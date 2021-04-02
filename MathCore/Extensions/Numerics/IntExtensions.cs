﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using MathCore;
using MathCore.Annotations;

using Complex = MathCore.Complex;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов-расширений для класса целых 4-х-байтовых чисел со знаком</summary>
    public static class IntExtensions
    {
        public static int HiBit(this int x) => Numeric.HiBit(x);

        public static int Log2(this int x) => Numeric.Log2(x);

        /// <summary>Возведение целого числа в целую степень</summary>
        /// <param name="x">Целое основание</param>
        /// <param name="N">Целый показатель степени</param>
        /// <returns>Результат возведения целого основания в целую степень</returns>
        [DST]
        public static int Power(this int x, int N) => (int)Math.Pow(x, N);

        /// <summary>Возведение целого числа в вещественную степень</summary>
        /// <param name="x">Целое основание</param>
        /// <param name="q">Вещественный показатель степени</param>
        /// <returns>Результат возведения целого основания в вещественную степень</returns>
        [DST]
        public static double Power(this int x, double q) => Math.Pow(x, q);

        /// <summary>Возведение целого числа в комплексную степень</summary>
        /// <param name="x">Целое основание</param>
        /// <param name="z">Комплексный показатель степени</param>
        /// <returns>Результат возведения целого основания в комплексную степень</returns>
        [DST]
        public static Complex Power(this int x, Complex z) => x ^ z;

        /// <summary>Факторизация целого числа</summary>
        /// <param name="n">Раскладываемое число</param>
        /// <returns>Последовательность простых чисел составляющих раскладываемое число</returns>
        public static IEnumerable<int> FactorizationEnum(this int n)
        {
            if(n < 0) n = -n;
            if(n <= 1) yield break;

            // пока число четное
            while((n & 1) == 0)
            {
                n >>= 1;
                yield return 2;
            }
            //как только кончились двойки в числе, переходим к 3 и далее по списку простых чисел
            var d = 3; // текущий делитель
            while(n != 1)
            {
                if(n % d == 0)
                {
                    yield return d;
                    n /= d;
                    continue;
                }
                d++;
            }
        }

        /// <summary>Разложение числа на простые множители</summary>
        /// <param name="n">Раскладываемое число</param>
        /// <returns>Массив простых множителей</returns>
        //[Copyright("Alexandr A Alexeev 2011", url = "http://eax.me")]
        [NotNull]
        public static int[] FactorizationList(this int n)
        {
            if(n < 0) n = -n;
            if(n <= 1) return Array.Empty<int>();

            var result = new List<int>();
            // пока число четное
            while((n & 1) == 0)
            {
                n >>= 1;
                result.Add(2);
            }

            var d = 3; // текущий делитель
            while(n != 1)
            {
                if(n % d == 0)
                {
                    result.Add(d);
                    n /= d;
                    continue;
                }
                d++;
            }
            return result.ToArray();
        }

        /// <summary>Разложение числа на простые множители</summary>
        /// <param name="n">Раскладываемое число</param>
        /// <returns>Словарь с делителями числа - значение элементов словаря - кратность делителя</returns>
        [NotNull]
        public static Dictionary<int, int> Factorization(this int n)
        {
            var result = new Dictionary<int, int>();

            if(n <= 1) return result;

            result.Add(2, 0);
            // пока число четное
            while((n & 1) == 0)
            {
                n >>= 1;
                result[2]++;
            }

            var d = 3; // текущий делитель
            while(n != 1)
            {
                if(n % d == 0)
                {
                    if(!result.ContainsKey(d))
                        result.Add(d, 1);
                    else
                        result[d]++;

                    n /= d;
                    continue;
                }
                d++;
            }
            return result;
        }

        /// <summary>Проверка - является ли число простым?</summary>
        /// <param name="n">Проверяемое число</param>
        /// <returns>Истина, если число простое</returns>
        [DST]
        public static bool IsPrime(this int n)
        {
            if(n < 0) n = -n;
            if(n % 2 == 0) return n == 2;

            var max = (int)Math.Sqrt(n);

            for(var i = 3; i <= max; i += 2) if(n % i == 0) return false;
            return true;
        }


        /// <summary>Является ли число степенью двойки?</summary>
        /// <param name="n">Проверяемое число</param>
        /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
        [DST]
        public static bool IsPowerOf2(this int n)
        {
            if(n < 0) n = -n;
            return (n & (n - 1)) == 0 || n == 1;
        }

        /// <summary>Определяет номер старшего бита в числе (индексация с 1)</summary>
        /// <param name="n">Исходное число</param>
        /// <returns>Число бит всех числа (включая нули)</returns>
        [DST]
        public static int BitCount(this int n) => n.GetNumberOfDigits(2);

        /// <summary>Получить число разрядов в указанной системе счисления</summary>
        /// <param name="n">Рассматриваемое число</param>
        /// <param name="Base">Основание системы счисления. По умолчанию = 10</param>
        /// <returns>Количество разрядов в указанной системе счисления</returns>
        [DST]
        public static int GetNumberOfDigits(this int n, int Base = 10) => (int)Math.Log(n < 0 ? -n : n, Base) + 1;

        /// <summary>Получить битовый массив из числа</summary>
        /// <param name="Value">Преобразуемое число</param>
        /// <param name="Length">Длина результирующего массива бит. По умолчанию = 32 битам</param>
        /// <returns>Битовый массив числа</returns>
        [DST, NotNull]
        public static BitArray GetBitArray(this int Value, int Length = 32)
        {
            var array = new BitArray(Length);
            for(var i = 0; i < Length; i++)
            {
                array[i] = (Value & 1) == 1;
                Value >>= 1;
            }
            return array;
        }

        /// <summary>Реверсирование бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <param name="N">Число реверсируемых бит</param>
        /// <returns>Реверсированное число</returns>
        [DST]
        public static int BitReversing(this int x, int N)
        {
            var Result = 0;
            for(var i = 0; i < N; i++)
            {
                Result <<= 1;
                Result += x & 1;
                x >>= 1;
            }
            return Result;
        }

        /// <summary>Реверсирование всех 32 бит числа</summary>
        /// <param name="x">исходное число</param>
        /// <returns>Реверсированное число</returns>
        [DST]
        public static int BitReversing(this int x) => x.BitReversing(sizeof(int) * 8);

        /// <summary>Проверка делимости числа на делитель</summary>
        /// <param name="x">Делимое</param>
        /// <param name="y">Делитель</param>
        /// <returns>Истина, если остаток от целочисленного деления равен 0</returns>
        [DST]
        public static bool IsDeviatedTo(this int x, int y) => x % y == 0;

        /// <summary>Положительный остаток от деления</summary>
        /// <param name="x">Делимое</param>
        /// <param name="mod">Модуль</param>
        /// <returns>Остаток от деления</returns>
        [DST]
        public static int GetAbsMod(this int x, int mod) => (x % mod) + (x < 0 ? mod : 0);

        /// <summary>Получить абсолютное значение числа</summary>
        /// <param name="x">Вещественное число</param>
        /// <returns>Модуль числа</returns>
        [DST]
        public static int GetAbs(this int x) => Math.Abs(x);

        /// <summary>Наибольший общий делитель</summary>
        /// <param name="Y">Первое число</param>
        /// <param name="X">Второе число</param>
        /// <returns>Наибольший общий делитель</returns>
        [DST]
        public static int GetNOD(this int Y, int X)
        {
            while(X != Y) if(X < Y) Y -= X; else X -= Y;
            return X;
        }

        /// <summary>Является ли число нечётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число нечётное</returns>
        [DST]
        public static bool IsOdd(this int x) => !x.IsEven();

        /// <summary>Является ли число чётным</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число чётное</returns>
        [DST]
        public static bool IsEven(this int x) => x.IsDeviatedTo(2);

        /// <summary>Факториал целого числа >= 0 и значение Г-функции для отрицательных значений</summary>
        /// <param name="n">Исходное число</param>
        /// <returns>Факториал числа</returns>
        [DST]
        public static long Factorial(this int n)
        {
            if(n < 0) return (long)SpecialFunctions.Gamma.G(n);

            if(n > 40)
            {
                var sqrt = Math.Sqrt(Consts.pi2 * n);
                var truncate = Math.Pow(n / Math.E, n);

                var r1 = new[]
                {
                    1,
                    1d / (12 * n),
                    1d / (288 * n * n),
                    139d / (51840 * n * n * n)
                };

                return (long)(sqrt * truncate * (r1[0] + r1[1] + r1[2] + r1[3]));
            }

            long result = n == 0 ? 1 : n;
            while(n > 1) result *= --n;
            return result;
        }

        /// <summary>Факториал целого числа >= 0 и значение Г-функции для отрицательных значений</summary>
        /// <param name="n">Исходное число</param>
        /// <returns>Факториал числа</returns>
        [DST]
        public static BigInteger FactorialBigInt(this int n)
        {
            if (n < 0) return (long)SpecialFunctions.Gamma.G(n);

            if (n <= 20) return n.Factorial();

            var result = n == 0 ? 1 : n;
            while (n > 1) result *= --n;
            return result;
        }


        /// <summary>Приведение целого числа в 10 системе счисления к виду системы счисления по основанию 8</summary>
        /// <param name="n">Число в 10-ой системе счисления</param>
        /// <returns>Представление числа в 8-ричной системе счисления</returns>
        [DST]
        public static int ToOctBase(this int n)
        {
            var num = 0;
            for(var i = 1; n != 0; i *= 10)
            {
                num += (n % 8) * i;
                n >>= 3;
            }
            return num;
        }

        /// <summary>Приведение целого числа в 8 системе счисления к виду системы счисления по основанию 10</summary>
        /// <param name="x">Число в 8-ой системе счисления</param>
        /// <returns>Представление числа в 10-ричной системе счисления</returns>
        [DST]
        public static int FromOctalBase(this int x)
        {
            var num = x % 10;
            x /= 10;
            var b = 8;
            while(x != 0)
            {
                num += (x % 10) * b;
                b *= 8;
                x /= 10;
            }
            return num;
        }

        [NotNull]
        public static int[] ToBase(this int x, int Base = 16)
        {
            var result = new int[GetNumberOfDigits(x, Base)];

            for(var i = 0; i < result.Length; i++)
            {
                result[i] = x % Base;
                x /= Base;
            }

            return result;
        }

        //[Obsolete("Используйте метод ToBase(Base:10)"), DST, NotNull]
        //public static int[] GetDigits(this int x)
        //{
        //    var result = new List<int>(20);
        //    while(x != 0)
        //    {
        //        result.Add(x % 10);
        //        x /= 10;
        //    }
        //    return result.ToArray();
        //}

        [DST]
        public static int GetFlags(this int Value, int Mask) => Value & Mask;

        [DST]
        public static int SetFlag(this int Value, int Flag, int Mask) => (Value & ~Mask) | (Flag & Mask);

        public static int ToInteger([NotNull] this byte[] data, int Offset = 0, bool IsMsbFirst = true)
        {
            if(data.Length == 0) return 0;
            var result = 0;
            bool sign;

            if(IsMsbFirst)
            {
                sign = (data[0] & 0x80) == 0x80;
                for(var i = Offset; i < data.Length; i++)
                {
                    result <<= 8;
                    result += data[i];
                }
            }
            else
            {
                sign = (data[data.Length - 1] & 0x80) == 0x80;
                for(var i = 0; (i + Offset) < data.Length; i++)
                    result += data[i + Offset] << (i * 8);
            }

            return sign ? -(((~result) & ((1 << (8 * data.Length)) - 1)) + 1) : result;
        }

        public static int ToInteger(this byte[] data, int Offset, int Length, bool IsMsbFirst = true) => 
            IsMsbFirst ? data.ToIntegerMSB(Offset, Length) : data.ToIntegerLSB(Offset, Length);

        public static int ToIntegerMSB([NotNull] this byte[] data, int Offset, int Length)
        {
            var result = 0;

            for(var i = Offset; i < data.Length && i - Offset < Length; i++)
                unchecked
                {
                    result = (result << 8) + data[i];
                }

            return result;
        }

        public static uint ToUIntegerMSB([NotNull] this byte[] data, int Offset, int Length)
        {
            var result = 0u;

            for(var i = Offset; i < data.Length && i - Offset < Length; i++)
                unchecked
                {
                    result = (result << 8) + data[i];
                }

            return result;
        }

        public static int ToIntegerLSB([NotNull] this byte[] data, int Offset, int Length)
        {
            var result = 0;

            for(var i = 0; i + Offset < data.Length && i < Length; i++)
                unchecked
                {
                    result += data[i + Offset] << (i << 3);
                }


            return result;
        }

        public static uint ToUIntegerLSB([NotNull] this byte[] data, int Offset, int Length)
        {
            var result = 0u;

            for(var i = 0; i + Offset < data.Length && i < Length; i++)
                unchecked
                {
                    result += (uint)(data[i + Offset] << (i << 3));
                }


            return result;
        }

        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
        public static int FromComplementBinary(this int I, int BitCount = 16)
        {
            var sign = (I >> (BitCount - 1)) == 1;
            var mask = (1 << (BitCount - 1)) - 1;
            var x = ((~((long)(I & mask) - 1)) & mask) * (sign ? -1 : 1);
            return (int)x;
        }
    }
}