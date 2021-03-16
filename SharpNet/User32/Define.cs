using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.User32
{
    public struct RECT
    {
        public int left;

        public int top;

        public int right;

        public int bottom;
    }

    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }
        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }
        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    public class Define
    {
        public const int WM_KEYDOWN = 256;
        public const int WM_KEYUP = 257;
        
    }
}
