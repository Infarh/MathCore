using System;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global

namespace MathCore.Functions
{
    // Функция
    using Function = Func<double, double>;

    /// <summary>Класс методов-расширений для функций</summary>
    public static class FunctionsExtensions
    {
        /// <summary>Структура значения функции {Аргумент - значение}</summary>
        public struct FuncValue
        {
            /// <summary>Аргумент функции</summary>
            public double Argument;

            /// <summary>Значение функции</summary>
            public double Value;

            /// <summary>Инициализация новой пары аргумент-значение функции</summary>
            /// <param name="arg">Аргумент функции</param>
            /// <param name="value">Значение функции</param>
            public FuncValue(double arg, double value)
            {
                Argument = arg;
                Value = value;
            }

            public static implicit operator double(FuncValue fv) => fv.Value;
        }

        /// <summary>Структура, содержащая максимальное и минимальное значение функции</summary>
        public struct FuncMinMaxValue
        {
            /// <summary>Минимальное значение функции</summary>
            public FuncValue Min;

            /// <summary>Максимальное значение функции</summary>
            public FuncValue Max;

            /// <summary>Инициализация нового максимального и минимального значения функции</summary>
            /// <param name="min">Минимум функции</param>
            /// <param name="max">Максимум функции</param>
            public FuncMinMaxValue(FuncValue min, FuncValue max)
            {
                Min = min;
                Max = max;
            }
        }

        /// <summary>Получить массив значений функции в указанном интервале с указанным шагом</summary>
        /// <param name="f">Функция, массив значений которой требуется получить</param>
        /// <param name="x1">Начало интервала</param>
        /// <param name="x2">Конец интервала</param>
        /// <param name="dx">Шаг сетки дискретизации</param>
        /// <returns>Массив значений функции</returns>
        [NotNull]
        public static double[] GetValues([NotNull] this Function f, double x1, double x2, double dx)
        {
            if (f == null)
                throw new ArgumentNullException(nameof(f));

            var X = x2 - x1;
            var N = (int)Math.Abs(X / dx);
            var values = new double[N];
            for (var i = 0; i < N; i++)
                values[i] = i * dx + x1;
            return values;
        }

        /// <summary>Определить минимум и максимум на интервале</summary>
        /// <param name="f">Функция, минимум и максимум которой требуется определить</param>
        /// <param name="x1">Начало интервала</param>
        /// <param name="x2">Конец интервала</param>
        /// <param name="dx">Шаг сетки разбиения</param>
        /// <returns>Структура, содержащая минимум и максимум функции</returns>
        public static FuncMinMaxValue GetMinMax([NotNull] this Function f, double x1, double x2, double dx = 0.0001)
        {
            var values = f.GetValues(x1, x2, dx);
            var N = values.Length;
            var MinMax = new FuncMinMaxValue
            {
                Min = new FuncValue { Value = double.PositiveInfinity },
                Max = new FuncValue { Value = double.NegativeInfinity }
            };

            for (var i = 0; i < N; i++)
            {
                var v = values[i];
                if (v < MinMax.Min.Value)
                {
                    MinMax.Min.Value = v;
                    MinMax.Min.Argument = i * dx + x1;
                }
                if (v > MinMax.Max.Value)
                {
                    MinMax.Max.Value = v;
                    MinMax.Max.Argument = i * dx + x1;
                }
            }

            return MinMax;
        }

        /// <summary>Определить минимум функции</summary>
        /// <param name="f">Исследуемая функция</param>
        /// <param name="x1">Начало интервала</param>
        /// <param name="x2">Конец интервала</param>
        /// <param name="dx">Шаг</param>
        /// <returns>Минимум функции</returns>
        public static FuncValue GetMinValue([NotNull] this Function f, double x1, double x2, double dx = 0.0001)
        {
            var values = f.GetValues(x1, x2, dx);
            var min = new FuncValue { Value = double.PositiveInfinity };
            for (int i = 0, N = values.Length; i < N; i++)
            {
                var v = values[i];
                if (v >= min.Value) continue;
                min.Value = v;
                min.Argument = i * dx + x1;
            }
            return min;
        }

        /// <summary>Определить максимум функции</summary>
        /// <param name="f">Исследуемая функция</param>
        /// <param name="x1">Начало интервала</param>
        /// <param name="x2">Конец интервала</param>
        /// <param name="dx">Шаг</param>
        /// <returns>Максимум функции</returns>
        public static FuncValue GetMaxValue([NotNull] this Function f, double x1, double x2, double dx = 0.0001)
        {
            var values = f.GetValues(x1, x2, dx);
            var max = new FuncValue { Value = double.NegativeInfinity };
            for (int i = 0, N = values.Length; i < N; i++)
            {
                var v = values[i];
                if (v <= max.Value) continue;
                max.Value = v;
                max.Argument = i * dx + x1;
            }
            return max;
        }
    }
}
