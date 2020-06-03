using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.User32
{
    public class Define
    {
        public const int WM_KEYDOWN = 256;
        public const int WM_KEYUP = 257;
        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        public struct POINT
        {
            public int x;

            public int y;
        }
    }
}
