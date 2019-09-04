using System;
using System.Threading.Tasks;
using MathCore.Annotations;

namespace MathCore.DifferencialEquations.Numerical
{
    public static partial class Solover
    {
        public static class Differential
        {
            static Differential() => __MethodsCount = diff_b.Length;

            private static readonly int __MethodsCount;

            internal static readonly int[] diff_b = { 1, 2, 6, 12, 60 };

            internal static readonly int[,] diff_a = {
                {-1, 1, 0, 0, 0, 0},
                {-3, 4, -1, 0, 0, 0},
                {-11, 18, -9, 2, 0, 0},
                {-25, 48, -36, 16, -3, 0},
                {-137, 300, -300, 200, -75, 12}
            };

            private static readonly int[][] dif_a = {
                new[] {-1, 1},
                new[] {-3, 4, -1},
                new[] {-11, 18, -9, 2},
                new[] {-25, 48, -36, 16, -3},
                new[] {-137, 300, -300, 200, -75, 12}
            };

            public static int MethodsCount => __MethodsCount;

            [NotNull]
            public static double[] GetDifferential([NotNull] double[] X, int n = -1)
            {
                var N = X.Length;
                var Y = new double[N];
                if(N == 0) return Y;
                if(n >= 0)
                {
                    var a = dif_a[n];
                    var a_len = a.Length;
                    var b = diff_b[n];
                    for(var i = 0; i < N; i++)
                    {
                        var y = 0d;
                        for(var j = 0; j < a_len && (i + j) < N; j++)
                            y += a[j] * X[i + j];
                        Y[i] = y / b;
                    }
                    return Y;
                }

                for(var i = 0; i < N; i++)
                {
                    n = N - i - 2;
                    if(n < 0) break;
                    if(n >= __MethodsCount) n = __MethodsCount - 1;

                    var a = dif_a[n];
                    var a_len = a.Length;
                    var b = diff_b[n];

                    var y = 0d;
                    for(var j = 0; j < a_len && (i + j) < N; j++)
                        y += a[j] * X[i + j];
                    Y[i] = y / b;
                }

                return Y;
            }

            [NotNull]
            public static Task<double[]> GetDifferentialAsync([NotNull] double[] X, int N = -1) => (X, N).Async(v => GetDifferential(v.X, v.N));
        }
    }
}