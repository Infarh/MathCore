﻿using System;

namespace MathCore
{
    public static class Numeric
    {
        public static int HiBit(int x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x - (x >> 1);
        }

        public static uint HiBit(uint x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x - (x >> 1);
        }

        public static byte HiBit(byte x)
        {
            x |= (byte)(x >> 1);
            x |= (byte)(x >> 2);
            x |= (byte)(x >> 4);
            return (byte)(x - (x >> 1));
        }

        public static sbyte HiBit(sbyte x)
        {
            x |= (sbyte)(x >> 1);
            x |= (sbyte)(x >> 2);
            x |= (sbyte)(x >> 4);
            return (sbyte)(x - (x >> 1));
        }

        public static short HiBit(short x)
        {
            x |= (short)(x >> 1);
            x |= (short)(x >> 2);
            x |= (short)(x >> 4);
            x |= (short)(x >> 8);
            return (short)(x - (x >> 1));
        }

        public static ushort HiBit(ushort x)
        {
            x |= (ushort)(x >> 1);
            x |= (ushort)(x >> 2);
            x |= (ushort)(x >> 4);
            x |= (ushort)(x >> 8);
            return (ushort)(x - (x >> 1));
        }

        public static long HiBit(long x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;
            return x - (x >> 1);
        }

        public static ulong HiBit(ulong x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;
            return x - (x >> 1);
        }

        public static int SignedBitCount(int x)
        {
            x = (x & 0x5555_5555) + ((x >> 1) & 0x5555_5555);
            x = (x & 0x3333_3333) + ((x >> 2) & 0x3333_3333);
            x = (x & 0x0F0F_0F0F) + ((x >> 4) & 0x0F0F_0F0F);
            x = (x & 0x00FF_00FF) + ((x >> 8) & 0x00FF_00FF);
            return (x & 0x0000_FFFF) + (x >> 16);
        }

        public static int SignedBitCount(uint x)
        {
            x = (x & 0x5555_5555) + ((x >> 1) & 0x5555_5555);
            x = (x & 0x3333_3333) + ((x >> 2) & 0x3333_3333);
            x = (x & 0x0F0F_0F0F) + ((x >> 4) & 0x0F0F_0F0F);
            x = (x & 0x00FF_00FF) + ((x >> 8) & 0x00FF_00FF);
            return (int) ((x & 0x0000_FFFF) + (x >> 16));
        }

        public static int SignedBitCount(byte x)
        {
            x = (byte)((x & 0x55) + ((x >> 1) & 0x55));
            x = (byte)((x & 0x33) + ((x >> 2) & 0x33));
            return (x & 0x0F) + (x >> 4);
        }

        public static int SignedBitCount(sbyte x)
        {
            x = (sbyte)((x & 0x55) + ((x >> 1) & 0x55));
            x = (sbyte)((x & 0x33) + ((x >> 2) & 0x33));
            return (x & 0x0F) + (x >> 4);
        }

        public static int SignedBitCount(short x)
        {
            x = (short) ((x & 0x5555) + ((x >> 1) & 0x5555));
            x = (short) ((x & 0x3333) + ((x >> 2) & 0x3333));
            x = (short) ((x & 0x0F0F) + ((x >> 4) & 0x0F0F));
            return (x & 0x00FF) + (x >> 8);
        }

        public static int SignedBitCount(ushort x)
        {
            x = (ushort)((x & 0x5555) + ((x >> 1) & 0x5555));
            x = (ushort)((x & 0x3333) + ((x >> 2) & 0x3333));
            x = (ushort)((x & 0x0F0F) + ((x >> 4) & 0x0F0F));
            return (x & 0x00FF) + (x >> 8);
        }

        public static int SignedBitCount(long x)
        {
            x = (x & 0x5555_5555__5555_5555) + ((x >> 01) & 0x5555_5555__5555_5555);
            x = (x & 0x3333_3333__3333_3333) + ((x >> 02) & 0x3333_3333__3333_3333);
            x = (x & 0x0F0F_0F0F__0F0F_0F0F) + ((x >> 04) & 0x0F0F_0F0F__0F0F_0F0F);
            x = (x & 0x00FF_00FF__00FF_00FF) + ((x >> 08) & 0x00FF_00FF__00FF_00FF);
            x = (x & 0x0000_FFFF__0000_FFFF) + ((x >> 16) & 0x0000_FFFF__0000_FFFF);
            return (int)((x & 0x0000_0000__FFFF_FFFF) + (x >> 32));
        }

        public static int SignedBitCount(ulong x)
        {
            x = (x & 0x5555_5555__5555_5555) + ((x >> 01) & 0x5555_5555__5555_5555);
            x = (x & 0x3333_3333__3333_3333) + ((x >> 02) & 0x3333_3333__3333_3333);
            x = (x & 0x0F0F_0F0F__0F0F_0F0F) + ((x >> 04) & 0x0F0F_0F0F__0F0F_0F0F);
            x = (x & 0x00FF_00FF__00FF_00FF) + ((x >> 08) & 0x00FF_00FF__00FF_00FF);
            x = (x & 0x0000_FFFF__0000_FFFF) + ((x >> 16) & 0x0000_FFFF__0000_FFFF);
            return (int)((x & 0x0000_0000__FFFF_FFFF) + (x >> 32));
        }

        [Copyright("GoldNotch@habr.com", url = "https://habr.com/ru/post/522788/")]
        public static int Log2(int x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(uint x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(short x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(ushort x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(byte x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(sbyte x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(long x) => SignedBitCount(HiBit(x) - 1);
        
        public static int Log2(ulong x) => SignedBitCount(HiBit(x) - 1);

        private static decimal Sqrt(decimal x, decimal eps = 0.0M)
        {
            var current = (decimal)Math.Sqrt((double)x);
            decimal last;
            do
            {
                last = current;
                if (last == 0.0M) return 0m;
                current = (last + x / last) * 0.5m;
            }
            while (Math.Abs(last - current) > eps);
            return current;
        }

        #region Radius

        public static double Radius(double X, double Y)
        {
            if (double.IsInfinity(X) || double.IsInfinity(Y))
                return double.PositiveInfinity;

            // |value| == sqrt(a^2 + b^2)
            // sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2) = a * sqrt(1 + b^2/a^2)
            // Using the above we can factor out the square of the larger component to dodge overflow.


            var x = Math.Abs(X);
            var y = Math.Abs(Y);

            if (x > y)
            {
                var ir = y / x;
                return x * Math.Sqrt(1d + ir * ir);
            }
            if (y.Equals(0d))
                return x; // re is either 0.0 or NaN
            var r = x / y;
            return y * Math.Sqrt(1d + r * r);
        }

        public static float Radius(float X, float Y)
        {
            if (float.IsInfinity(X) || float.IsInfinity(Y))
                return float.PositiveInfinity;

            // |value| == sqrt(a^2 + b^2)
            // sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2) = a * sqrt(1 + b^2/a^2)
            // Using the above we can factor out the square of the larger component to dodge overflow.


            var x = Math.Abs(X);
            var y = Math.Abs(Y);

            if (x > y)
            {
                var ir = y / x;
                return (float)(x * Math.Sqrt(1d + ir * ir));
            }
            if (y.Equals(0f))
                return x; // re is either 0.0 or NaN
            var r = x / y;
            return (float)(y * Math.Sqrt(1d + r * r));
        }

        public static decimal Radius(decimal X, decimal Y)
        {
            // |value| == sqrt(a^2 + b^2)
            // sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2) = a * sqrt(1 + b^2/a^2)
            // Using the above we can factor out the square of the larger component to dodge overflow.
            var x = Math.Abs(X);
            var y = Math.Abs(Y);

            if (x > y)
            {
                var ir = y / x;
                return x * Sqrt(1m + ir * ir);
            }
            if (y.Equals(0m))
                return x; // re is either 0.0 or NaN
            var r = x / y;
            return y * Sqrt(1m + r * r);
        }

        #endregion

        public static double Angle(double X, double Y) =>
            X.Equals(0d)
                ? Y.Equals(0d)
                    ? 0
                    : Math.Sign(Y) * Consts.pi05
                : Y.Equals(0d)
                    ? Math.Sign(X) > 0
                        ? 0d
                        : Consts.pi
                    : Math.Atan2(Y, X);

        public static float Angle(float X, float Y) => (float)
             (X.Equals(0f)
                ? Y.Equals(0f)
                    ? 0
                    : Math.Sign(Y) * Consts.pi05
                : Y.Equals(0f)
                    ? Math.Sign(X) > 0
                        ? 0f
                        : Consts.pi
                    : Math.Atan2(Y, X));
    }
}
