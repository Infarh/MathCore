namespace MathCore.Functions.Differentiable
{
    public static class Functions
    {
        public static Zero Zero => new Zero();
        public static One One => new One();
        public static Identity Identity => new Identity();
        public static Exponent Exponent => new Exponent();

        public static Sinus Sinus => new Sinus();
        public static Cousins Cousins => new Cousins();
        public static Tangent Tangent => new Tangent();
        public static Cotangent Cotangent => new Cotangent();

        public static HyperbolicSinus HyperbolicSinus => new HyperbolicSinus();
        public static HyperbolicCosines HyperbolicCosines => new HyperbolicCosines();
        public static HyperbolicCotangent HyperbolicCotangent => new HyperbolicCotangent();
        public static HyperbolicTangent HyperbolicTangent => new HyperbolicTangent();
    }
}
