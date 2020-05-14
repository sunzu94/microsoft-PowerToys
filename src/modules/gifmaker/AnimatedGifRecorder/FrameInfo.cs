using System;
using System.Collections.Generic;
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
        /// Path to bitmap
        /// </summary>
        public string Path;
    }
}
