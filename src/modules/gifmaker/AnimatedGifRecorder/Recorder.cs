using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //Texture that is used to recieve the pixel data from the GPU.
            BackingTexture = new Texture2D(Device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Conf.Width,
                Height = Conf.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
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

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Pause()
        {

        }

        private void Capture()
        {

        }

        /// <summary>
        /// The current device being duplicated.
        /// </summary>
        protected internal Device Device;

        /// <summary>
        /// The desktop duplication interface.
        /// </summary>
        protected internal OutputDuplication DuplicatedOutput;

        /// <summary>
        /// The texture used exclusively to be a backing texture when capturing the cursor shape.
        /// This texture will always hold only the desktop texture, without the cursor.
        /// </summary>
        protected internal Texture2D BackingTexture;

        /// <summary>
        /// Texture used to merge the cursor with the background image (desktop).
        /// </summary>
        protected internal Texture2D CursorStagingTexture;

        /// <summary>
        /// The buffer that holds all pixel data of the cursor.
        /// </summary>
        protected internal byte[] CursorShapeBuffer;

        /// <summary>
        /// The details of the cursor.
        /// </summary>
        protected internal OutputDuplicatePointerShapeInformation CursorShapeInfo;

        /// <summary>
        /// The previous position of the mouse cursor.
        /// </summary>
        protected internal OutputDuplicatePointerPosition PreviousPosition;

        /// <summary>
        /// Summary current recording session configurations
        /// </summary>
        protected internal RecorderConf Conf;

        protected internal int OffsetLeft { get; set; }
        protected internal int OffsetTop { get; set; }

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
    }
}
