using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimatedGifRecorder
{
    public class FrameInfo
    {
        /// <summary>
        /// Frame index
        /// </summary>
        public int Index;

        /// <summary>
        /// Delay to next frame
        /// </summary>
        public int Delay;

        /// <summary>
        /// Path to saved image file
        /// </summary>
        public string Path;

        /// <summary>
        /// Bitmap image
        /// Will be disposed off once image is saved to path
        /// </summary>
        public Image Image;
    }
}
