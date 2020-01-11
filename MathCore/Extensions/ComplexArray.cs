using System;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace MathCore
{
    public static class ComplexArray
    {
        public static void Deconstruct([NotNull] this Complex[] ComplexArray, [NotNull] out double[] Re, [NotNull] out double[] Im)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Re = new double[ComplexArray.Length];
            Im = new double[ComplexArray.Length];
            for (var i = 0; i < ComplexArray.Length; i++) (Re[i], Im[i]) = ComplexArray[i];
        }

        public static void ToAbsArg([NotNull] this Complex[] ComplexArray, [NotNull] out double[] Abs, [NotNull] out double[] Arg)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Abs = new double[ComplexArray.Length];
            Arg = new double[ComplexArray.Length];
            for (var i = 0; i < ComplexArray.Length; i++)
            {
                Abs[i] = ComplexArray[i].Abs;
                Arg[i] = ComplexArray[i].Arg;
            }
        }

        public static void ToAbsArgDeg([NotNull] this Complex[] ComplexArray, [NotNull] out double[] Abs, [NotNull] out double[] Arg)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Abs = new double[ComplexArray.Length];
            Arg = new double[ComplexArray.Length];
            for (var i = 0; i < ComplexArray.Length; i++)
            {
                Abs[i] = ComplexArray[i].Abs;
                Arg[i] = ComplexArray[i].Arg * Consts.ToDeg;
            }
        }
    }
}