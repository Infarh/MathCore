// ReSharper disable UnusedType.Global
namespace MathCore.Statistic;

public static partial class Distributions
{
    public static class Erlang
    {
        //public static double ModelA(double )

        private static double modelB_(double A, int n)
        {
            switch (n)
            {
                case 0: return 1;
                case 1: return A;
            }

            var result = A;
            for (var i = 2; i <= n; i++)
            {
                result *= A;
                result /= i;
            }

            return result;
        }

        public static double ModelB(double A, int N, int n)
        {
            double P = 1, q = 1, Q = 0;

            for(var i = 1; i <= N; i++)
            {
                Q += q       *= A / i;
                if(i == n) P =  q;
            }

            return P / Q;

            //return modelB_(A, N) / Enumerable.Range(0, N).Aggregate(0d, (S, i) => S + modelB_(A, i));
        }
        public static double ModelB(double A, int N) => ModelB(A, N, N);

        public static double[] GetDistribution(double A, int N) => GetDistribution(A, N, N);
        public static double[] GetDistribution(double A, int N, int n_max)
        {
            var p = new double[n_max + 1];

            var Q = p[0] = 1;
                
            for(int i = 1, nn = Math.Max(n_max, N); i <= nn; i++)
            {
                if(i <= n_max) p[i] = p[i-1] * A / i;
                if(i <= N) Q       += p[i];
            }

            return p.DivideItself(Q);
        }

        private static double modelC_(double A, int N) => Math.Pow(A, N) * N / (N.Factorial() * (N - A));
        public static double ModelC_OC(double A, int N) => 1 / (modelC_(A, N) + Enumerable.Range(0, N - 1).Aggregate(0d, (_, i) => modelB_(A, i)));
        public static double ModelC(double A, int N) => ModelC_OC(A, N) * modelC_(A, N);
    }
}