using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore.Functions.Differentiable
{
    public static class Functions
    {
        public static Zero Zero => new Zero();
        public static One One => new One();
        public static Identity Identity => new Identity();
        public static Exponenta Exponenta => new Exponenta();

        public static Sinus Sinus => new Sinus();
        public static Cosinus Cosinus => new Cosinus();
        public static Tangens Tangens => new Tangens();
        public static Cotangens Cotangens => new Cotangens();

        public static HiperbolicSinus HiperbolicSinus => new HiperbolicSinus();
        public static HiperbolicCosinus HiperbolicCosinus => new HiperbolicCosinus();
        public static HiperbolicCotangens HiperbolicCotangens => new HiperbolicCotangens();
        public static HiperbolicTangens HiperbolicTangens => new HiperbolicTangens();
    }
}
