using System;
using System.Collections.Generic;
using System.Text;
using MathCore.Vectors;

namespace MathCore
{
    public static class Numeric
    {
        public static Matrix Sqr(Matrix x) => x * x;
        public static Complex Sqr(Complex x) => x * x;
        public static double Sqr(double x) => x * x;
        public static float Sqr(float x) => x * x;
        public static decimal Sqr(decimal x) => x * x;
        public static ulong Sqr(ulong x) => x * x;
        public static long Sqr(long x) => x * x;
        public static uint Sqr(uint x) => x * x;
        public static int Sqr(int x) => x * x;
        public static ushort Sqr(ushort x) => (ushort) (x * x);
        public static short Sqr(short x) => (short) (x * x);
        public static sbyte Sqr(sbyte x) => (sbyte) (x * x);
        public static byte Sqr(byte x) => (byte) (x * x);

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
