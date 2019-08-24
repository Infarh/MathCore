using System;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Vectors
{
    public class VectorND_double : Vector<double>
    {
        public double Length => Math.Sqrt(this.Sum());

        public VectorND_double(int Demention) : base(Demention) { }

        public VectorND_double(double[] Elements) : base(Elements) { }

        public VectorND_double(IEnumerable<double> Elements) : base(Elements.ToArray()) { }


        public VectorND_double GetProduction(VectorND_double b)
        {
            if(Demention != b.Demention) throw new ArgumentException("Размерности векторов не совпадают");
            return new VectorND_double(new double[Demention].Initialize(i => this[i] * b[i]));
        }

        public double GetScalarProduction(VectorND_double b)
        {
            if(Demention != b.Demention) throw new ArgumentException("Размерности векторов не совпадают");
            return GetProduction(b).Sum();
        }

        public static VectorND_double GetVectorProduction(VectorND_double[] Vectors)
        {
            if(Vectors == null || Vectors.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(Vectors),
                    "Векторное произведение не может быть рассчитано для пустого множества векторов");

            var N = Vectors[0].Demention;
            for(var i = 1; i < Vectors.Length; i++)
                if(Vectors[i].Demention != N)
                    throw new ArgumentOutOfRangeException(nameof(Vectors),
                                $"Размерность вектора {i}:{Vectors[i].Demention} не соответствует размерности первого вектора {N}");

            if(N - 1 != Vectors.Length)
                throw new ArgumentOutOfRangeException(nameof(Vectors),
                    "Количество векторов для векторного произведения не соответствует размерности векторного пространства");


            var M = new Matrix(N, (i, j) => i == 0 ? 0 : Vectors[i - 1][j]);
            var k = -1;
            return new VectorND_double(new double[N].Initialize(i => M.GetMinor(i, 0).GetDeterminant() * (k *= -1)));
        }

        public VectorND_double GetInversed() => new VectorND_double(new double[Demention].Initialize(i => 1 / this[i]));

        public static VectorND_double operator +(VectorND_double a, VectorND_double b)
        {
            if(a.Demention != b.Demention) throw new ArgumentException("Размерности векторов не совпадают");
            return new VectorND_double(new double[a.Demention].Initialize(i => a[i] + b[i]));
        }

        public static VectorND_double operator +(VectorND_double a, double b) => new VectorND_double(new double[a.Demention].Initialize(i => a[i] + b));

        public static VectorND_double operator +(double a, VectorND_double b) => new VectorND_double(new double[b.Demention].Initialize(i => a + b[i]));

        public static VectorND_double operator -(VectorND_double a, VectorND_double b)
        {
            if(a.Demention != b.Demention) throw new ArgumentException("Размерности векторов не совпадают");
            return new VectorND_double(new double[a.Demention].Initialize(i => a[i] - b[i]));
        }

        public static VectorND_double operator -(VectorND_double a, double b) => new VectorND_double(new double[a.Demention].Initialize(i => a[i] - b));

        public static VectorND_double operator -(double a, VectorND_double b) => new VectorND_double(new double[b.Demention].Initialize(i => a - b[i]));

        public static double operator *(VectorND_double a, VectorND_double b) => a.GetScalarProduction(b);

        public static VectorND_double operator *(VectorND_double a, double b) => new VectorND_double(new double[a.Demention].Initialize(i => a[i] * b));

        public static VectorND_double operator *(double a, VectorND_double b) => new VectorND_double(new double[b.Demention].Initialize(i => a * b[i]));

        public static double operator /(VectorND_double a, VectorND_double b) => a * b.GetInversed();

        public static VectorND_double operator /(VectorND_double a, double b) => new VectorND_double(new double[a.Demention].Initialize(i => a[i] / b));

        public static VectorND_double operator /(double a, VectorND_double b) => new VectorND_double(new double[b.Demention].Initialize(i => a / b[i]));
    }
}