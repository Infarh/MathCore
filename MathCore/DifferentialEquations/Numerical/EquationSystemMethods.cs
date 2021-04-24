using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathCore.Annotations;
using MathCore.Vectors;
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable UnusedMember.Global

namespace MathCore.DifferentialEquations.Numerical
{
    public static class EquationSystemMethods
    {
        //public readonly struct SystemResultItem<TValue> : IEquatable<SystemResultItem<TValue>>
        //{
        //    public double t { get; }
        //    public IReadOnlyCollection<TValue> X { get; }

        //    public SystemResultItem(double t, IReadOnlyCollection<TValue> X)
        //    {
        //        this.t = t;
        //        this.X = X;
        //    }

        //    /// <inheritdoc />
        //    [NotNull] public override string ToString() => $"{t}:{{{string.Join(",", X)}}}";

        //    /// <inheritdoc />
        //    public bool Equals(SystemResultItem<TValue> other) => t.Equals(other.t) && Equals(X, other.X);

        //    /// <inheritdoc />
        //    public override bool Equals(object obj) => obj is SystemResultItem<TValue> other && Equals(other);

        //    /// <inheritdoc />
        //    public override int GetHashCode()
        //    {
        //        unchecked
        //        {
        //            return (t.GetHashCode() * 397) ^ (X != null ? X.GetHashCode() : 0);
        //        }
        //    }

        //    public static bool operator ==(SystemResultItem<TValue> left, SystemResultItem<TValue> right) => left.Equals(right);
        //    public static bool operator !=(SystemResultItem<TValue> left, SystemResultItem<TValue> right) => !left.Equals(right);
        //}

        [NotNull]
        private static double[] Add([NotNull] this double[] X, double[] Y, double k = 1)
        {
            var result = new double[X.Length];
            for(var i = 0; i < result.Length; i++)
                result[i] = X[i] + Y[i] * k;
            return result;
        }

        [NotNull]
        private static Complex[] Add([NotNull] this Complex[] X, Complex[] Y, double k = 1)
        {
            var result = new Complex[X.Length];
            for(var i = 0; i < result.Length; i++)
                result[i] = X[i] + Y[i] * k;
            return result;
        }

        [NotNull]
        private static Vector2D[] Add([NotNull] this Vector2D[] X, Vector2D[] Y, double k = 1)
        {
            var result = new Vector2D[X.Length];
            for(var i = 0; i < result.Length; i++)
                result[i] = X[i] + Y[i] * k;
            return result;
        }

        [NotNull]
        private static Vector3D[] Add([NotNull] this Vector3D[] X, Vector3D[] Y, double k = 1)
        {
            var result = new Vector3D[X.Length];
            for(var i = 0; i < result.Length; i++)
                result[i] = X[i] + Y[i] * k;
            return result;
        }

        private static double[] GetRungeKuttaResult([NotNull] this double[] Y, double[] K1, double[] K2, double[] K3, double[] K4, double dx)
        {
            for(var i = 0; i < Y.Length; i++)
            {
                K1[i] += 2 * K2[i] + 2 * K3[i] + K4[i];
                K1[i] *= dx / 6;
                K1[i] += Y[i];
            }
            return K1;
        }

        private static Vector2D[] GetRungeKuttaResult([NotNull] this Vector2D[] Y, Vector2D[] K1, Vector2D[] K2, Vector2D[] K3, Vector2D[] K4, double dx)
        {
            for(var i = 0; i < Y.Length; i++)
            {
                K1[i] += 2 * K2[i] + 2 * K3[i] + K4[i];
                K1[i] *= dx / 6;
                K1[i] += Y[i];
            }
            return K1;
        }

        private static Vector3D[] GetRungeKuttaResult([NotNull] this Vector3D[] Y, Vector3D[] K1, Vector3D[] K2, Vector3D[] K3, Vector3D[] K4, double dx)
        {
            for(var i = 0; i < Y.Length; i++)
            {
                K1[i] += 2 * K2[i] + 2 * K3[i] + K4[i];
                K1[i] *= dx / 6;
                K1[i] += Y[i];
            }
            return K1;
        }

        private static Complex[] GetRungeKuttaResult([NotNull] this Complex[] Y, Complex[] K1, Complex[] K2, Complex[] K3, Complex[] K4, double dx)
        {
            for(var i = 0; i < Y.Length; i++)
            {
                K1[i] += 2 * K2[i] + 2 * K3[i] + K4[i];
                K1[i] *= dx / 6;
                K1[i] += Y[i];
            }
            return K1;
        }

        public static IEnumerable<(double t, IReadOnlyList<double> X)> Compute_RungeKutta(this DiffEqs system, double x0, double x1, double dx, double[] Y0)
        {
            var x = x0;
            var y = Y0;
            yield return (x, y);
            var dx2 = dx / 2;

            while(x <= x1)
            {
                var k1 = system(x, y);
                var k2 = system(x + dx2, y.Add(k1, dx2));
                var k3 = system(x + dx2, y.Add(k2, dx2));
                var k4 = system(x + dx, y.Add(k3, dx));
                y = y.GetRungeKuttaResult(k1, k2, k3, k4, dx);
                x += dx;
                yield return (x, y);
            }
        }

        public static IEnumerable<(double t, IReadOnlyList<Complex> X)> Compute_RungeKutta(this DiffEqsComplex system, double x0, double x1, double dx, Complex[] Y0)
        {
            var x = x0;
            var y = Y0;
            yield return (x, y);
            var dx2 = dx / 2;

            while(x <= x1)
            {
                var k1 = system(x, y);
                var k2 = system(x + dx2, y.Add(k1, dx2));
                var k3 = system(x + dx2, y.Add(k2, dx2));
                var k4 = system(x + dx, y.Add(k3, dx));
                y = y.GetRungeKuttaResult(k1, k2, k3, k4, dx);
                x += dx;
                yield return (x, y);
            }
        }

        public static IEnumerable<(double t, IReadOnlyList<Vector2D> X)> Compute_RungeKutta(this DiffEqsVector2D system, double x0, double x1, double dx, Vector2D[] Y0)
        {
            var x = x0;
            var y = Y0;
            yield return (x, y);
            var dx2 = dx / 2;

            while(x <= x1)
            {
                var k1 = system(x, y);
                var k2 = system(x + dx2, y.Add(k1, dx2));
                var k3 = system(x + dx2, y.Add(k2, dx2));
                var k4 = system(x + dx, y.Add(k3, dx));
                y = y.GetRungeKuttaResult(k1, k2, k3, k4, dx);
                x += dx;
                yield return (x, y);
            }
        }

        public static IEnumerable<(double t, IReadOnlyList<Vector3D> X)> Compute_RungeKutta(this DiffEqsVector3D system, double x0, double x1, double dx, Vector3D[] Y0)
        {
            var x = x0;
            var y = Y0;
            yield return (x, y);
            var dx2 = dx / 2;

            while(x <= x1)
            {
                var k1 = system(x, y);
                var k2 = system(x + dx2, y.Add(k1, dx2));
                var k3 = system(x + dx2, y.Add(k2, dx2));
                var k4 = system(x + dx, y.Add(k3, dx));
                y = y.GetRungeKuttaResult(k1, k2, k3, k4, dx);
                x += dx;
                yield return (x, y);
            }
        }
    }
}