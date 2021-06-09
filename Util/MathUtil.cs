using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Util
{
    public class MathUtil
    {
        public static Decimal Clamp(Decimal value, Decimal min, Decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static ulong Clamp(ulong value, ulong min, ulong max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static long Clamp(long value, long min, long max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static uint Clamp(uint value, uint min, uint max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static short Clamp(short value, short min, short max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static byte Clamp(byte value, byte min, byte max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }

}
