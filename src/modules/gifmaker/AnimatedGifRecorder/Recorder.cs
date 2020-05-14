using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AnimatedGifRecorder.Properties;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace AnimatedGifRecorder
{
    /// <summary>
    /// Minimized version of ScreenToGif DirectImageCapture
    /// https://github.com/NickeManarin/ScreenToGif
    /// </summary>
    public class Recorder
    {
        public Recorder(RecorderConf conf)
        {
            Device = new Device(DriverType.Hardware, DeviceCreationFlags.VideoSupport);

            using (var multiThread = Device.QueryInterface<Multithread>())
                multiThread.SetMultithreadProtected(true);

            //Texture used to copy contents from the GPU to be accesible by the CPU.
            StagingTexture = new Texture2D(Device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                Format = Format.B8G8R8A8_UNorm,
                Width = Conf.Width,
                Height = conf.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging
            });

            using (var factory = new Factory1())
            {
                //Get the Output1 based on the current capture region position.
                using (var output1 = GetOutput(factory))
                {
                    try
                    {
                        //Make sure to run with the integrated graphics adapter if using a Microsoft hybrid system. https://stackoverflow.com/a/54196789/1735672
                        DuplicatedOutput = output1.DuplicateOutput(Device);
                    }
                    catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.NotCurrentlyAvailable)
                    {
                        throw new Exception("Too many applications using the Desktop Duplication API. Please close one of the applications and try again.", e);
                    }
                    catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.Unsupported)
                    {
                        throw new NotSupportedException("The Desktop Duplication API is not supported on this computer. If you have multiple graphic cards, try running ScreenToGif on integrated graphics.", e);
                    }
                }
            }


        }

        /// <summary>
        /// To update configurations in the future
        /// </summary>
        /// <param name="conf"></param>
        public void Configure(RecorderConf conf)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (!_stopped && !_recording)
            {
                _recording = true;
            }

            //Spin up a Task to consume the BlockingCollection.
            Task.Run(() =>
            {
                try
                {
                    while (true)
                        Save(BlockingCollection.Take());
                }
                catch (InvalidOperationException)
                {
                    //It means that Take() was called on a completed collection.
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Exception: {e.Message}");
                }
            });

            Task.Run(async () =>
            {
                while(true)
                {
                    if (_recording)
                    {
                        Capture();
                    }
                    if (_stopped) return;
                    await Task.Delay(Interval);
                }
            });
        }


        public void Stop()
        {
            _stopped = true;
        }

        public void Pause()
        {
            _recording = false;
        }

        /// <summary>
        /// Get the correct Output1 based on region to be captured.
        /// TODO: Get the correct output when the user moves the capture region to other screen.
        /// TODO: Capture multiple screens at the same time.
        /// </summary>
        private Output1 GetOutput(Factory1 factory)
        {
            try
            {
                //Gets the output with the bigger area being intersected.
                var output = factory.Adapters1.SelectMany(s => s.Outputs).OrderByDescending(f =>
                {
                    var x = Math.Max(Conf.X, f.Description.DesktopBounds.Left);
                    var num1 = Math.Min(Conf.X + Conf.Width, f.Description.DesktopBounds.Right);
                    var y = Math.Max(Conf.Y, f.Description.DesktopBounds.Top);
                    var num2 = Math.Min(Conf.Y + Conf.Height, f.Description.DesktopBounds.Bottom);

                    if (num1 >= x && num2 >= y)
                        return num1 - x + num2 - y;

                    return 0;
                }).FirstOrDefault();

                if (output == null)
                    throw new Exception($"Could not find a proper output device for the area of L: {Conf.X}, T: {Conf.Y}, Width: {Conf.Width}, Height: {Conf.Height}.");

                //Position adjustments, so the correct region is captured.
                OffsetLeft = output.Description.DesktopBounds.Left;
                OffsetTop = output.Description.DesktopBounds.Top;

                return output.QueryInterface<Output1>();
            }
            catch (SharpDXException ex)
            {
                throw new Exception("Could not find the specified output device.", ex);
            }
        }

        /// <summary>
        /// Saves frame into separate file images;
        /// </summary>
        /// <param name="frameInfo"></param>
        private void Save(FrameInfo frame)
        {
            frame.Image?.Save(frame.Path);
            frame.Image?.Dispose();
            frame.Image = null;

            Frames.Add(frame);
        }

        private int Capture()
        {
            var res = new Result(-1);
            var frame = new FrameInfo();

            try
            {
                //Try to get the duplicated output frame within given time.
                res = DuplicatedOutput.TryAcquireNextFrame(0, out var info, out var resource);

                //Somehow, it was not possible to retrieve the resource or any frame.
                if (res.Failure || resource == null || info.AccumulatedFrames == 0)
                {
                    resource?.Dispose();
                    return FrameCount;
                }

                //Copy resource into memory that can be accessed by the CPU.
                using (var screenTexture = resource.QueryInterface<Texture2D>())
                {
                    //Copies from the screen texture only the area which the user wants to capture.
                    Device.ImmediateContext.CopySubresourceRegion(screenTexture, 0, new ResourceRegion(TrueLeft, TrueTop, 0, TrueRight, TrueBottom, 1), StagingTexture, 0);
                }

                //Get the desktop capture texture.
                var data = Device.ImmediateContext.MapSubresource(StagingTexture, 0, MapMode.Read, MapFlags.None);

                if (data.IsEmpty)
                {
                    Device.ImmediateContext.UnmapSubresource(StagingTexture, 0);
                    resource.Dispose();
                    return FrameCount;
                }

                #region Get image data

                var bitmap = new System.Drawing.Bitmap(Conf.Width, Conf.Height, PixelFormat.Format32bppArgb);
                var boundsRect = new System.Drawing.Rectangle(0, 0, Conf.Width, Conf.Height);

                //Copy pixels from screen capture Texture to the GDI bitmap.
                var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                var sourcePtr = data.DataPointer;
                var destPtr = mapDest.Scan0;

                for (var y = 0; y < Conf.Height; y++)
                {
                    //Copy a single line.
                    Utilities.CopyMemory(destPtr, sourcePtr, Conf.Width * 4);

                    //Advance pointers.
                    sourcePtr = IntPtr.Add(sourcePtr, data.RowPitch);
                    destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                }

                //Release source and dest locks.
                bitmap.UnlockBits(mapDest);

                //Set frame details.
                frame.Path = $"{System.IO.Path.GetTempPath()}/test_{FrameCount++}";
                frame.Delay = Interval;
                frame.Image = bitmap;
                BlockingCollection.Add(frame);

                #endregion

                Device.ImmediateContext.UnmapSubresource(StagingTexture, 0);

                resource.Dispose();
                return FrameCount;
            }
            catch (SharpDXException se) when (se.ResultCode.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
            {
                return FrameCount;
            }
            catch (SharpDXException se) when (se.ResultCode.Code == SharpDX.DXGI.ResultCode.DeviceRemoved.Result.Code || se.ResultCode.Code == SharpDX.DXGI.ResultCode.DeviceReset.Result.Code)
            {
                //When the device gets lost or reset, the resources should be instantiated again.
                throw new AccessViolationException("Device was lost. Session needs to be restarted");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to capture frame");
                Debug.WriteLine($"Error message: {e.Message}");
                return FrameCount;
            }
            finally
            {
                try
                {
                    //Only release the frame if there was a sucess in capturing it.
                    if (res.Success)
                        DuplicatedOutput.ReleaseFrame();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Unable to capture frame");
                    Debug.WriteLine($"Error message: {e.Message}");
                }
            }
        }
    

        /// <summary>
        /// The current device being duplicated.
        /// </summary>
        private Device Device;

        /// <summary>
        /// The desktop duplication interface.
        /// </summary>
        private OutputDuplication DuplicatedOutput;

        /// <summary>
        /// The texture used to copy the pixel data from the desktop to the destination image. 
        /// </summary>
        private Texture2D StagingTexture;

        /// <summary>
        /// Texture used to merge the cursor with the background image (desktop).
        /// </summary>
        private Texture2D CursorStagingTexture;

        /// <summary>
        /// The buffer that holds all pixel data of the cursor.
        /// </summary>
        private byte[] CursorShapeBuffer;

        /// <summary>
        /// The details of the cursor.
        /// </summary>
        private OutputDuplicatePointerShapeInformation CursorShapeInfo;

        /// <summary>
        /// The previous position of the mouse cursor.
        /// </summary>
        private OutputDuplicatePointerPosition PreviousPosition;

        /// <summary>
        /// Summary current recording session configurations
        /// </summary>
        private RecorderConf Conf;

        private int OffsetLeft { get; set; }
        private int OffsetTop { get; set; }
        private int TrueLeft => Conf.X + OffsetLeft;
        private int TrueRight => Conf.X + OffsetLeft + Conf.Width;
        private int TrueTop => Conf.Y + OffsetTop;
        private int TrueBottom => Conf.Y + OffsetTop + Conf.Height;
        private int Interval => (int)(1000 / Conf.FrameRate);

        /// <summary>
        /// The latest time in which a frame or metadata was captured.
        /// </summary>
        private long LastProcessTime = 0;

        /// <summary>
        /// Number of frames recorded
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Collection of frame metadata in the current session
        /// </summary>
        private List<FrameInfo> Frames { get; private set; } = new List<FrameInfo>();

        /// <summary>
        /// Frames in recording. 
        /// Using BlockingCollection for multithreaded saving.
        /// </summary>
        protected BlockingCollection<FrameInfo> BlockingCollection { get; private set; } = new BlockingCollection<FrameInfo>();

        private bool _recording;
        private bool _stopped;
    }
}
