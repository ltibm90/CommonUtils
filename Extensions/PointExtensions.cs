using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CommonUtils.Extensions
{
   public static class PointExtensions
    {
        public static bool Equals(this Point p, int x, int y)
        {
            return p.X == x && p.Y == y;
        }
        public static bool Equals(this PointF p, float x, float y)
        {
            return p.X == x && p.Y == y;
        }
    }
}
