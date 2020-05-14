using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimatedGifRecorder
{
    public class RecorderConf
    {
        /// <summary>
        /// Width of recording
        /// </summary>
        public int Width;
        
        /// <summary>
        /// Height of recording
        /// </summary>
        public int Height;

        /// <summary>
        /// Left most pixel of recording region
        /// </summary>
        public int X;

        /// <summary>
        /// Top most pixel of recording region
        /// </summary>
        public int Y;

        /// <summary>
        /// Determines the time interval between frames (1000ms / framerate)
        /// </summary>
        public double FrameRate;
    }
}
