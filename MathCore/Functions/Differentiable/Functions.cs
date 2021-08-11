namespace MathCore.Functions.Differentiable
{
    public static class Functions
    {
        public static Zero Zero => new();
        public static One One => new();
        public static Identity Identity => new();
        public static Exponent Exponent => new();

        public static Sinus Sinus => new();
        public static Cousins Cousins => new();
        public static Tangent Tangent => new();
        public static Cotangent Cotangent => new();

        public static HyperbolicSinus HyperbolicSinus => new();
        public static HyperbolicCosines HyperbolicCosines => new();
        public static HyperbolicCotangent HyperbolicCotangent => new();
        public static HyperbolicTangent HyperbolicTangent => new();
    }
}
