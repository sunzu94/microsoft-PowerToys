using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace AnimatedGifRecorder.Encoders
{
    /// <summary>
    /// Taken from https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-encode-and-decode-a-gif-image
    /// </summary>
    public static class GifEncoder
    {
        /// <summary>
        /// Takes a sequence of frameinfos and merges them into a single GIF
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="filePath"></param>
        public static void Encode(List<FrameInfo> frames, string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Create);
            GifBitmapEncoder encoder = new GifBitmapEncoder();
            
            foreach(var frame in frames)
            {
                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(frame.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            }
            encoder.Save(stream);
        }
    }
}
