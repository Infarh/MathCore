﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore;

public partial class Matrix
{
    public partial class Array
    {
        /// <summary>Преобразовать двумерный массив элементов матрицы в массив массивов-столбцов</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <returns>Массив столбцов</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        public static double[][] MatrixToColsArray(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            var result = new double[M][];
            for (var j = 0; j < M; j++)
            {
                var col = new double[N];
                for (var i = 0; i < N; i++) col[i] = matrix[i, j];
                result[j] = col;
            }
            return result;
        }

        /// <summary>Преобразовать двумерный массив элементов матрицы в массив массивов-строк</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <returns>Массив строк</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        public static double[][] MatrixToRowsArray(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            var result = new double[N][];
            for (var i = 0; i < N; i++)
            {
                var row = new double[M];
                for (var j = 0; j < M; j++) row[j] = matrix[i, j];
                result[i] = row;
            }
            return result;
        }

        /// <summary>Создать двумерный массив массив матрицы из массива столбцов</summary>
        /// <param name="cols">Массив столбцов матрицы</param>
        /// <returns>Двумерный массив элементов матрицы</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cols"/> is <see langword="null"/></exception>
        public static double[,] ColsArrayToMatrix(params double[][] cols)
        {
            if (cols is null) throw new ArgumentNullException(nameof(cols));

            var M = cols.Length;
            var N = cols[0].Length;
            var result = new double[N, M];
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) result[i, j] = cols[j][i];
            return result;
        }

        /// <summary>Создать двумерный массив массив матрицы из массива строк</summary>
        /// <param name="rows">Массив строк матрицы</param>
        /// <returns>Двумерный массив элементов матрицы</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rows"/> is <see langword="null"/></exception>
        public static double[,] RowsArrayToMatrix(params double[][] rows)
        {
            if (rows is null) throw new ArgumentNullException(nameof(rows));

            var N = rows.Length;
            var M = rows[0].Length;
            var result = new double[N, M];
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) result[i, j] = rows[i][j];
            return result;
        }

        /// <summary>Проверка - является ли матрица вырожденной</summary>
        /// <param name="matrix">Проверяемая матрица</param>
        /// <returns>Истина, если определитель матрицы равен нулю</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица не содержит элементов, или если матрица не квадратная</exception>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static bool IsMatrixSingular(double[,] matrix)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.Length == 0) throw new ArgumentException(@"Матрица не содержит элементов", nameof(matrix));
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));


            return Rank(matrix) != matrix.GetLength(0);
        }

        /// <summary>Определение ранга матрицы</summary>
        /// <param name="matrix">Матрица, ранг которой требуется определить</param>
        /// <returns>Ранг матрицы</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица не содержит элементов</exception>
        public static int Rank(double[,] matrix)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.GetLength(0) == 0 || matrix.GetLength(1) == 0) throw new ArgumentException(@"Матрица не содержит элементов", nameof(matrix));

            GetTriangle(matrix, out var rank, out _);
            return rank;
        }

        /// <summary>Создать диагональную матрицу</summary>
        /// <param name="elements">Элементы диагонали матрицы</param>
        /// <returns>Двумерный массив, содержащий на главной диагонали элементы диагональной матрицы</returns>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Массив не содержит элементов</exception>
        public static double[,] CreateDiagonal(params double[] elements)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));
            if (elements.Length == 0)
                throw new ArgumentException(@"Массив не содержит элементов", nameof(elements));

            var N = elements.Length;
            var result = new double[N, N];
            for (var i = 0; i < N; i++) result[i, i] = elements[i];
            return result;
        }

        /// <summary>Получить массив элементов тени (главной диагонали) матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <returns>Массив элементов тени матрицы</returns>
        /// <exception cref="ArgumentException">Массив не содержит элементов</exception>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        public static double[] GetMatrixShadow(double[,] matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException(nameof(matrix));
            if (matrix.GetLength(0) == 0 || matrix.GetLength(1) == 0)
                throw new ArgumentException(@"Массив не содержит элементов", nameof(matrix));

            GetLength(matrix, out var N, out var M);
            var result = new double[Math.Min(M, N)];
            for (var i = 0; i < result.Length; i++) result[i] = matrix[i, i];
            return result;
        }

        /// <summary>Перечислить элементы тени (главной диагонали) матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <returns>Перечисление элементов тени матрицы</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Массив не содержит элементов</exception>
        public static IEnumerable<double> EnumerateMatrixShadow(double[,] matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException(nameof(matrix));
            if (matrix.GetLength(0) == 0 || matrix.GetLength(1) == 0)
                throw new ArgumentException(@"Массив не содержит элементов", nameof(matrix));

            GetLength(matrix, out var N, out var M);
            for (int i = 0, length = Math.Min(M, N); i < length; i++) yield return matrix[i, i];
        }

        /// <summary>Применение матрицы перестановок слева (перестановка строк)</summary>
        /// <param name="matrix">Матрица, подвергаемая перестановке строк</param>
        /// <param name="p">Матрица перестановок (строк)</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="p"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица перестановок не квадратная</exception>
        /// <exception cref="ArgumentException">Число строк матрицы не равно числу столбцов матрицы перестановок</exception>
        public static void Permutation_Left(double[,] matrix, double[,] p)
        {
            if (matrix is null)
                throw new ArgumentNullException(nameof(matrix));
            if (p is null)
                throw new ArgumentNullException(nameof(p));
            if (p.GetLength(0) != p.GetLength(1))
                throw new ArgumentException(@"Матрица перестановок не квадратная", nameof(p));
            if (matrix.GetLength(0) != p.GetLength(1))
                throw new ArgumentException(@"Число строк матрицы не равно числу столбцов матрицы перестановок", nameof(matrix));

            GetRowsCount(p, out var N);
            if (N == 1) return;
            var m = matrix.CloneObject();
            // ReSharper disable CompareOfFloatsByEqualityOperator
            for (var i = 0; i < N; i++)
                if ((int)p[i, i] != 1)
                {
                    var j = 0;
                    while (j < N && (int)p[i, j] != 1) j++;
                    if (j == N) continue;
                    if (p[i, j] != p[j, i]) throw new InvalidOperationException($@"Ошибка в матрице перестановок: элемент p[{i},{j}] не соответствует элементу p[{j},{i}]");
                    if (j >= i) continue;
                    m.SwapRows(i, j);
                }
            // ReSharper restore CompareOfFloatsByEqualityOperator
            System.Array.Copy(m, matrix, matrix.Length);
        }

        /// <summary>Применение матрицы перестановок справа (перестановка столбцов)</summary>
        /// <param name="matrix">Матрица, подвергаемая перестановке столбцов</param>
        /// <param name="p">Матрица перестановок (столбцов)</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="p"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица перестановок не квадратная</exception>
        /// <exception cref="ArgumentException">Число строк матрицы не равно числу столбцов матрицы перестановок</exception>
        public static void Permutation_Right(double[,] matrix, double[,] p)
        {
            if (matrix is null)
                throw new ArgumentNullException(nameof(matrix));
            if (p is null)
                throw new ArgumentNullException(nameof(p));
            if (p.GetLength(0) != p.GetLength(1))
                throw new ArgumentException(@"Матрица перестановок не квадратная", nameof(p));
            if (matrix.GetLength(0) != p.GetLength(1))
                throw new ArgumentException(@"Число строк матрицы не равно числу столбцов матрицы перестановок", nameof(matrix));

            GetRowsCount(p, out var N);
            if (N == 1) return;
            var m = matrix.CloneObject();
            for (var i = 0; i < N; i++)
                if ((int)p[i, i] != 1)
                {
                    var j = 0;
                    while (j < N && (int)p[i, j] != 1) j++;
                    if (j == N) continue;
                    if (!p[i, j].Equals(p[j, i])) throw new InvalidOperationException($@"Ошибка в матрице перестановок: элемент p[{i},{j}] не соответствует элементу p[{j},{i}]");
                    if (j >= i) continue;
                    m.SwapCols(i, j);
                }
            System.Array.Copy(m, matrix, matrix.Length);
        }

        /// <summary>Применение матрицы перестановок слева (перестановка строк) без проверок</summary>
        /// <param name="matrix">Матрица, подвергаемая перестановке строк</param>
        /// <param name="p">Матрица перестановок (строк)</param>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="p"/></exception>
        private static void Permutation_Left_Internal(double[,] matrix, double[,] p)
        {
            GetRowsCount(p, out var N);
            if (N == 1) return;
            for (var i = 0; i < N; i++)
                if ((int)p[i, i] != 1)
                {
                    var j = 0;
                    while (j < N && (int)p[i, j] != 1) j++;
                    if (j == N || j >= i) continue;
                    matrix.SwapRows(i, j);
                }
        }

        /// <summary>Применение матрицы перестановок справа (перестановка столбцов) без проверок</summary>
        /// <param name="matrix">Матрица, подвергаемая перестановке столбцов</param>
        /// <param name="p">Матрица перестановок (столбцов)</param>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="p"/></exception>
        private static void Permutation_Right_Internal(double[,] matrix, double[,] p)
        {
            GetRowsCount(p, out var N);
            if (N == 1) return;
            for (var i = 0; i < N; i++)
                if ((int)p[i, i] != 1)
                {
                    var j = 0;
                    while (j < N && (int)p[i, j] != 1) j++;
                    if (j == N || j >= i) continue;
                    matrix.SwapCols(i, j);
                }
        }

        /// <summary>Создать и инициализировать двумерный массив-матрицу</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        /// <param name="Creator">Функция, принимающая номер строки и номер столбца и возвращающая значение элемента матрицы</param>
        /// <returns>Массив элементов матрицы</returns>
        public static double[,] Create(int N, int M, Func<int, int, double> Creator)
        {
            var result = new double[N, M];
            for (var n = 0; n < N; n++)
                for (var m = 0; m < M; m++)
                    result[n, m] = Creator(n, m);
            return result;
        }

        /// <summary>Создать двумерный массив элементов матрицы-столбца</summary>
        /// <param name="data">Элементы массива матрицы-столбца</param>
        /// <returns>Двумерный массив элементов матрицы столбца</returns>
        /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
        /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
        public static double[,] CreateColArray(params double[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)
                throw new ArgumentException(@"Длина массива должна быть больше 0", nameof(data));

            var result = new double[data.Length, 1];
            for (var i = 0; i < data.Length; i++) result[i, 0] = data[i];
            return result;
        }

        /// <summary>Создать двумерный массив элементов матрицы-строки</summary>
        /// <param name="data">Элементы массива матрицы-строки</param>
        /// <returns>Двумерный массив элементов матрицы строки</returns>
        /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
        /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
        public static double[,] CreateRowArray(params double[] data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) throw new ArgumentException(@"Длина массива должна быть больше 0", nameof(data));

            var result = new double[1, data.Length];
            for (var j = 0; j < data.Length; j++) result[0, j] = data[j];
            return result;
        }

        /// <summary>Получить размерность массива матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы, размеры которого требуется получить</param>
        /// <param name="N">Число строк матрицы</param>
        /// <param name="M">Число столбцов (элементов строки) матрицы</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        [DST]
        public static void GetLength(
            double[,] matrix,
            out int N,
            out int M,
            [CallerArgumentExpression("matrix")] string? MatrixArgumentName = null)
        {
            if (matrix is null)
                throw new ArgumentNullException(MatrixArgumentName ?? nameof(matrix));

            N = matrix.GetLength(0);
            M = matrix.GetLength(1);
        }

        /// <summary>Определение размера квадратной матрицы</summary>
        /// <param name="matrix">Квадратная матрица, размер которой надо определить</param>
        /// <returns>Возвращает число строк матрицы</returns>
        /// <exception cref="ArgumentNullException">Если передана пустая ссылка на массив</exception>
        /// <exception cref="InvalidOperationException">Если матрица не является квадратной</exception>
        public static int GetSquareLength(double[,] matrix, [CallerArgumentExpression("matrix")] string? MatrixArgumentName = null)
        {
            if (matrix is null) throw new ArgumentNullException(MatrixArgumentName ?? nameof(matrix));
            var n = matrix.GetLength(0);
            var m = matrix.GetLength(1);
            if (m != n) throw new InvalidOperationException($"Матрица [{n}x{m}] не является квадратной");
            return n;
        }

        /// <summary>Получить число строк массива матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы, размеры которого требуется получить</param>
        /// <param name="N">Число строк матрицы</param>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="matrix"/></exception>
        [DST]
        public static void GetRowsCount(
            double[,] matrix,
            out int N,
            [CallerArgumentExpression("matrix")] string? MatrixArgumentName = null)
        {
            if (matrix is null) throw new ArgumentNullException(MatrixArgumentName ?? nameof(matrix));

            N = matrix.GetLength(0);
        }

        /// <summary>Получить число столбцов (элементов строки) массива матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы, размеры которого требуется получить</param>
        /// <param name="M">Число столбцов (элементов строки) матрицы</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        [DST]
        public static void GetColsCount(
            double[,] matrix,
            out int M,
            [CallerArgumentExpression("matrix")] string? MatrixArgumentName = null)
        {
            if (matrix is null) throw new ArgumentNullException(MatrixArgumentName ?? nameof(matrix));

            M = matrix.GetLength(1);
        }

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="N">Размерность матрицы</param>
        /// <returns>Квадратный двумерный массив размерности NxN с 1 на главной диагонали</returns>
        /// <exception cref="ArgumentOutOfRangeException">В случае если размерность матрицы <paramref name="N"/> меньше 1</exception>
        [DST]
        public static double[,] GetUnitaryArrayMatrix(int N)
        {
            if (N < 1) throw new ArgumentOutOfRangeException(nameof(N), @"Размерность матрицы должна быть больше 0");

            var result = new double[N, N];
            for (var i = 0; i < N; i++)
                result[i, i] = 1;
            return result;
        }

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <returns>Квадратный двумерный массив размерности NxN с 1 на главной диагонали</returns>
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="matrix"/> не квадратная</exception>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        [DST]
        public static void InitializeUnitaryMatrix(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            if (N != M)
                throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));

            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    matrix[i, j] = i == j ? 1 : 0;
        }

        /// <summary>Трансвекция матрицы</summary>
        /// <param name="A">Трансвецируемая матрица</param>
        /// <param name="i0">Опорная строка</param>
        /// <returns>Трансвекция матрицы А</returns>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="A"/></exception>
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="A"/> не квадратная</exception>
        /// <exception cref="ArgumentException">В случае если опорная строка <paramref name="i0"/> матрицы <paramref name="A"/> &lt; 0 и &gt; числа строк матрицы</exception>
        public static double[,] GetTransvection(double[,] A, int i0)
        {
            GetRowsCount(A, out var N);
            if (N != A.GetLength(1))
                throw new ArgumentException(@"Трансвекция неквадратной матрицы невозможна", nameof(A));
            if (i0 < 0 || i0 >= N)
                throw new ArgumentException(@"Номер опорной строки выходит за пределы индексов строк матрицы", nameof(i0));

            var result = GetUnitaryArrayMatrix(N);
            for (var i = 0; i < N; i++)
                result[i, i0] = i == i0
                    ? 1 / A[i0, i0]
                    : -A[i, i0] / A[i0, i0];
            return result;
        }

        /// <summary>Трансвекция матрицы</summary>
        /// <param name="A">Трансвецируемая матрица</param>
        /// <param name="j0">Опорный столбец</param>
        /// <param name="result">Двумерный массив элементов матрицы результата</param>
        /// <returns>Трансвекция матрицы А</returns>
        /// <exception cref="ArgumentNullException">В случае если матрицы <paramref name="A"/> и <paramref name="result"/> не заданы</exception>  
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="A"/> не квадратная</exception>
        /// <exception cref="ArgumentException">В случае если опорный столбец <paramref name="j0"/> матрицы <paramref name="A"/> меньше 0 или больше числа столбцов матрицы</exception>
        /// <exception cref="ArgumentException">В случае если размер матрицы <paramref name="result"/> не совпадает с размером матрицы <paramref name="A"/></exception>
        public static void Transvection(double[,] A, int j0, double[,] result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            GetRowsCount(A, out var N);
            if (N != A.GetLength(1)) throw new ArgumentException(@"Трансвекция неквадратной матрицы невозможна", nameof(A));
            if (N != result.GetLength(0) || A.GetLength(1) != result.GetLength(1))
                throw new ArgumentException(@"Размер матрицы результата не соответствует размеру исходной матрицы", nameof(result));
            if (j0 < 0 || j0 >= N) throw new ArgumentException(@"Номер опорного столбца выходит за пределы индексов столбцов матрицы", nameof(j0));

            InitializeUnitaryMatrix(result);
            for (var i = 0; i < N; i++)
                result[i, j0] = i == j0
                    ? 1 / A[j0, j0]
                    : -A[i, j0] / A[j0, j0];
        }

        /// <summary>Получить столбец матрицы в виде матрицы</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="j">Номер столбца</param>
        /// <returns>Матрица-столбец, составленная из элементов столбца матрицы c индексом j</returns>
        /// <exception cref="ArgumentOutOfRangeException">В случае если указанный номер столбца <paramref name="j"/> матрицы <paramref name="matrix"/> меньше 0, либо больше числа столбцов матрицы</exception>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="matrix"/></exception>
        [DST]
        public static double[,] GetCol(double[,] matrix, int j)
        {
            GetRowsCount(matrix, out var N);
            if (j < 0 || j >= N) throw new ArgumentOutOfRangeException(nameof(j), j, @"Указанный номер столбца матрицы выходит за границы массива");

            var result = new double[N, 1];
            for (var i = 0; i < N; i++)
                result[i, 0] = matrix[i, j];
            return result;
        }

        /// <summary>Получить столбец матрицы в виде массива</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="j">Номер столбца</param>
        /// <returns>Массив, составленная из элементов столбца матрицы c индексом <paramref name="j"/></returns>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если указанный номер столбца <paramref name="j"/> матрицы <paramref name="matrix"/> меньше 0, либо больше числа столбцов матрицы</exception>
        [DST]
        public static double[] GetCol_Array(double[,] matrix, int j)
        {
            GetRowsCount(matrix, out var N);
            if (j < 0 || j >= N) throw new ArgumentOutOfRangeException(nameof(j), j, @"Указанный номер столбца матрицы выходит за границы массива");

            var result = new double[N];
            for (var i = 0; i < N; i++)
                result[i] = matrix[i, j];
            return result;
        }

        /// <summary>Получить столбец матрицы в виде массива</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="j">Номер столбца</param>
        /// <param name="result">Массив, составленная из элементов столбца матрицы c индексом <paramref name="j"/></param>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если массив <paramref name="result"/> не задан</exception>
        /// <exception cref="ArgumentException">В случае если размер массива <paramref name="result"/> не соответствует числу строк матрицы</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если указанный номер столбца <paramref name="j"/> матрицы <paramref name="matrix"/> меньше 0, либо больше числа столбцов матрицы</exception>
        [DST]
        public static void GetCol_Array(double[,] matrix, int j, double[] result)
        {
            GetRowsCount(matrix, out var N);

            if (j < 0 || j >= N) throw new ArgumentOutOfRangeException(nameof(j), j, @"Указанный номер столбца матрицы выходит за границы массива");
            if (result is null) throw new ArgumentNullException(nameof(result));
            if (result.Length != N) throw new ArgumentException(@"Размер массива результата не соответствует числу строк исходной матрицы", nameof(result));

            for (var i = 0; i < N; i++)
                result[i] = matrix[i, j];
        }

        /// <summary>Получить строку матрицы</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="i">Номер строки</param>
        /// <returns>Матрица-строка, составленная из элементов строки матрицы с индексом <paramref name="i"/></returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если указанный номер строки <paramref name="i"/> матрицы <paramref name="matrix"/> меньше 0, либо больше числа строк матрицы</exception>
        [DST]
        public static double[,] GetRow(double[,] matrix, int i)
        {
            GetColsCount(matrix, out var M);

            if (i < 0 || i >= M) throw new ArgumentOutOfRangeException(nameof(i), i, @"Указанный номер строки матрицы выходит за границы массива");

            var result = new double[1, M];
            for (var j = 0; j < M; j++)
                result[0, j] = matrix[i, j];
            return result;
        }

        /// <summary>Получить строку матрицы</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="i">Номер строки</param>
        /// <returns>Массив, составленный из элементов строки матрицы с индексом <paramref name="i"/></returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если указанный номер строки <paramref name="i"/> матрицы <paramref name="matrix"/> меньше 0, либо больше числа строк матрицы</exception>
        [DST]
        public static double[] GetRow_Array(double[,] matrix, int i)
        {
            GetColsCount(matrix, out var M);

            if (i < 0 || i >= M) throw new ArgumentOutOfRangeException(nameof(i), i, @"Указанный номер строки матрицы выходит за границы массива");

            var result = new double[M];
            for (var j = 0; j < M; j++)
                result[j] = matrix[i, j];
            return result;
        }

        /// <summary>Получить строку матрицы</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="i">Номер строки</param>
        /// <param name="result">Массив, составленный из элементов строки матрицы с индексом i</param>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если массив <paramref name="result"/> не задан</exception>
        /// <exception cref="ArgumentException">В случае если размер массива <paramref name="result"/> не соответствует числу столбцов матрицы</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если указанный номер строки <paramref name="i"/> матрицы <paramref name="matrix"/> меньше 0, либо больше числа строк матрицы</exception>
        [DST]
        public static void GetRow_Array(double[,] matrix, int i, double[] result)
        {
            GetColsCount(matrix, out var M);

            if (i < 0 || i >= M) throw new ArgumentOutOfRangeException(nameof(i), i, @"Указанный номер строки матрицы выходит за границы массива");
            if (result is null) throw new ArgumentNullException(nameof(result));
            if (result.Length != M) throw new ArgumentException(@"Размер массива результата не соответствует числу столбцов исходной матрицы", nameof(result));

            for (var j = 0; j < M; j++)
                result[j] = matrix[i, j];
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <param name="matrix">Обращаемая матрица</param>
        /// <returns>Обратная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="matrix"/> не квадратная</exception>
        /// <exception cref="InvalidOperationException">Невозможно найти обратную матрицу для вырожденной матрицы</exception>
        public static double[,] Inverse(double[,] matrix)
        {
            var result = Inverse(matrix, out var p);
            Permutation_Left_Internal(result, p);
            return result;
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <param name="matrix">Обращаемая матрица</param>
        /// <param name="p">Матрица перестановок</param>
        /// <returns>Обратная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="matrix"/> не квадратная</exception>
        /// <exception cref="InvalidOperationException">Невозможно найти обратную матрицу для вырожденной матрицы</exception>
        /// <exception cref="ArgumentOutOfRangeException">Размерность массива 0х0</exception>
        public static double[,] Inverse(double[,] matrix, out double[,] p)
        {
            GetLength(matrix, out var N, out var M);

            if (N == 0 || M == 0) throw new ArgumentOutOfRangeException(nameof(matrix), @"Матрица пуста");
            if (N != M) throw new ArgumentException(@"Обратная матрица существует только для квадратной матрицы", nameof(matrix));

            var result = GetUnitaryArrayMatrix(N);
            return TrySolve(matrix, ref result, out p)
                ? result
                : throw new InvalidOperationException(@"Невозможно найти обратную матрицу для вырожденной матрицы");
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <param name="matrix">Матрица, подлежащая обращению</param>
        /// <param name="result">Обратная матрица</param>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="result"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="matrix"/> не квадратная</exception>
        public static void Inverse(double[,] matrix, double[,] result)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));
            if (result is null) throw new ArgumentNullException(nameof(result));
            if (result.GetLength(0) != matrix.GetLength(0) || result.GetLength(1) != matrix.GetLength(1))
                throw new ArgumentException(@"Размерность матрицы результата не соответствует размерности исходной матрицы", nameof(result));

            InitializeUnitaryMatrix(result);

            if (!TrySolve(matrix, ref result, out _))
                throw new InvalidOperationException(@"Невозможно найти обратную матрицу для вырожденной матрицы");
        }

        /// <summary>Метод решения СЛАУ A*X=B -&gt; X</summary>
        /// <param name="matrix">Матрица СЛАУ</param>
        /// <param name="b">Правая часть СЛАУ</param>
        /// <param name="p">Матрица перестановок</param>
        /// <returns>Матрица решения уравнения A*X=B -&gt; X</returns>
        /// <exception cref="InvalidOperationException">Невозможно найти обратную матрицу для вырожденной матрицы</exception>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentException">В случае если матрица системы <paramref name="matrix"/> не квадратная</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static double[,] GetSolve(double[,] matrix, double[,] b, out double[,] p)
        {
            var x = b;
            return TrySolve(matrix, ref x, out p, true)
                ? x
                : throw new InvalidOperationException(@"Невозможно найти обратную матрицу для вырожденной матрицы");
        }

        /// <summary>Метод решения СЛАУ</summary>
        /// <param name="matrix">Матрица СЛАУ</param>
        /// <param name="b">Правая часть СЛАУ</param>
        /// <param name="p">Матрица перестановок</param>
        /// <param name="clone_b">Работать с копией <paramref name="b"/></param>
        /// <exception cref="InvalidOperationException">Невозможно найти обратную матрицу для вырожденной матрицы</exception>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentException">В случае если матрица системы <paramref name="matrix"/> не квадратная</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static void Solve(double[,] matrix, ref double[,] b, out double[,] p, bool clone_b = false)
        {
            if (!TrySolve(matrix, ref b, out p, clone_b))
                throw new InvalidOperationException("Невозможно найти обратную матрицу для вырожденной матрицы");
        }

        /// <summary>Попытаться решить СЛАУ</summary>
        /// <param name="matrix">Матрица СЛАУ</param>
        /// <param name="b">Правая часть СЛАУ</param>
        /// <param name="p">Матрица перестановок</param>
        /// <param name="clone_b">Работать с копией <paramref name="b"/></param>
        /// <returns>Истина, если решение СЛАУ получено; ложь - если матрица СЛАУ <paramref name="matrix"/> вырождена</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="b"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentException">В случае если матрица системы <paramref name="matrix"/> не квадратная</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static bool TrySolve(double[,] matrix, ref double[,] b, out double[,] p, bool clone_b = false)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (b is null) throw new ArgumentNullException(nameof(b));
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new ArgumentException(@"Матрица системы не квадратная", nameof(matrix));
            if (matrix.GetLength(0) != b.GetLength(0)) throw new ArgumentException(@"Число строк матрицы правой части не равно числу строк матрицы системы", nameof(b));

            var temp_b = b.CloneObject();
            Triangulate(ref matrix, ref temp_b, out p, out var d);
            if (d == 0) return false;
            GetColsCount(b, out var b_M);

            GetLength(matrix, out var N, out var M);
            for (var i0 = Math.Min(N, M) - 1; i0 >= 0; i0--)
            {
                var m = matrix[i0, i0];
                if (m != 1) for (var j = 0; j < b_M; j++) temp_b[i0, j] /= m;
                for (var i = i0 - 1; i >= 0; i--)
                    if (matrix[i, i0] != 0)
                    {
                        var k = matrix[i, i0];
                        matrix[i, i0] = 0d;
                        for (var j = 0; j < b_M; j++) temp_b[i, j] -= temp_b[i0, j] * k;
                    }
            }
            if (clone_b)
                b = temp_b;
            else
                System.Array.Copy(temp_b, b, b.Length);
            return true;
        }

        /// <summary>Транспонирование матрицы</summary>
        /// <returns>Транспонированная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        [DST]
        public static double[,] Transpose(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            var result = new double[M, N];
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    result[j, i] = matrix[i, j];
            return result;
        }

        /// <summary>Транспонирование матрицы</summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <param name="result">Транспонированная матрица</param>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="result"/> не задана</exception>
        [DST]
        public static void Transpose(double[,] matrix, double[,] result)
        {
            GetLength(matrix, out var N, out var M);
            if (result is null) throw new ArgumentNullException(nameof(result));
            if (result.GetLength(0) != M) throw new ArgumentException(@"Число строк матрицы результата не равно числу столбцов исходной матрицы", nameof(result));
            if (result.GetLength(1) != N) throw new ArgumentException(@"Число столбцов матрицы результата не равно числу строк исходной матрицы", nameof(result));

            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    result[j, i] = matrix[i, j];
        }

        /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <param name="n">Номер строки</param>
        /// <param name="m">Номер столбца</param>
        /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если номер строки <paramref name="n"/> меньше 0, или больше, либо равен числу строк матрицы <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если номер столбца <paramref name="m"/> меньше 0, или больше, либо равен числу столбцов матрицы <paramref name="matrix"/></exception>
        public static double GetAdjunct(double[,] matrix, int n, int m)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (n < 0 || n >= matrix.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(n), n, @"Указанный номер строки вышел за пределы границ массива");
            if (m < 0 || m >= matrix.GetLength(1)) throw new ArgumentOutOfRangeException(nameof(m), m, @"Указанный номер столбца вышел за пределы границ массива");

            return (n + m) % 2 == 0
                ? GetDeterminant(GetMinor(matrix, n, m))
                : -GetDeterminant(GetMinor(matrix, n, m));
        }

        /// <summary>Скопировать минор из матрицы в матрицу результата</summary>
        /// <param name="matrix">Массив элементов исходной матрицы</param>
        /// <param name="n">Номер строки</param>
        /// <param name="m">Номер столбца</param>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        /// <param name="result">Минор матрицы</param>
        private static void CopyMinor(double[,] matrix, int n, int m, int N, int M, double[,] result)
        {
            var i0 = 0;
            for (var i = 0; i < N; i++)
                if (i != n)
                {
                    var j0 = 0;
                    for (var j = 0; j < M; j++)
                        if (j != m)
                            result[i0, j0++] = matrix[i, j];
                    i0++;
                }
        }

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <param name="n">Номер строки</param>
        /// <param name="m">Номер столбца</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если номер строки <paramref name="n"/> меньше 0, или больше, либо равен числу строк матрицы <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если номер столбца <paramref name="m"/> меньше 0, или больше, либо равен числу столбцов матрицы <paramref name="matrix"/></exception>
        public static double[,] GetMinor(double[,] matrix, int n, int m)
        {
            GetLength(matrix, out var N, out var M);
            if (n < 0 || n >= N) throw new ArgumentOutOfRangeException(nameof(n), n, @"Указанный номер строки вышел за пределы границ массива");
            if (m < 0 || m >= M) throw new ArgumentOutOfRangeException(nameof(m), m, @"Указанный номер столбца вышел за пределы границ массива");

            var result = new double[N - 1, M - 1];
            CopyMinor(matrix, n, m, N, M, result);
            return result;
        }

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <param name="n">Номер строки</param>
        /// <param name="m">Номер столбца</param>
        /// <param name="result">Минор элемента матрицы [n,m]</param>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если номер строки <paramref name="n"/> меньше 0, или больше, либо равен числу строк матрицы <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если номер столбца <paramref name="m"/> меньше 0, или больше, либо равен числу столбцов матрицы <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentException">В случае если число строк матрицы результата <paramref name="result"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentException">В случае если число столбцов матрицы результата <paramref name="result"/> не равно числу столбцов исходной матрицы <paramref name="matrix"/></exception>
        public static void GetMinor(double[,] matrix, int n, int m, double[,] result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            GetLength(matrix, out var N, out var M);

            if (n < 0 || n >= N) throw new ArgumentOutOfRangeException(nameof(n), n, @"Указанный номер строки вышел за пределы границ массива");
            if (m < 0 || m >= M) throw new ArgumentOutOfRangeException(nameof(m), m, @"Указанный номер столбца вышел за пределы границ массива");

            if (result.GetLength(0) != N - 1) throw new ArgumentException(@"Число строк матрицы результата не равно числу строк исходной матрицы - 1", nameof(result));
            if (result.GetLength(1) != M - 1) throw new ArgumentException(@"Число столбцов матрицы результата не равно числу столбцов исходной матрицы - 1", nameof(result));

            CopyMinor(matrix, n, m, N, M, result);
        }

        /// <summary>Определитель матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <returns>Определитель матрицы</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если матрица <paramref name="matrix"/> не квадратная</exception>
        public static double GetDeterminant(double[,] matrix)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));

            GetRowsCount(matrix, out var N);

            switch (N)
            {
                case 0: return double.NaN;
                case 1: return matrix[0, 0];
                case 2:
                    return matrix[0, 0] * matrix[1, 1] 
                         - matrix[0, 1] * matrix[1, 0];
                case 3:
                    return matrix[0, 0] * matrix[1, 1] * matrix[2, 2]
                         + matrix[0, 1] * matrix[1, 2] * matrix[2, 0]
                         + matrix[0, 2] * matrix[1, 0] * matrix[2, 1]
                         - matrix[0, 2] * matrix[1, 1] * matrix[2, 0]
                         - matrix[0, 0] * matrix[1, 2] * matrix[2, 1]
                         - matrix[0, 1] * matrix[1, 0] * matrix[2, 2];
                default:
                    Triangulate(matrix.CloneObject(), out var d);
                    return d;
            }
        }

        /// <summary>Поменять значения местами</summary>
        /// <typeparam name="T">Тип значения</typeparam>
        [DST] private static void Swap<T>(ref T v1, ref T v2) { var t = v1; v1 = v2; v2 = t; }

        /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
        /// <remarks>
        /// This method is based on the 'LU Decomposition and Its Applications' 
        /// section of Numerical Recipes in C by William H. Press, Saul A. Teukolsky, William T. 
        /// Vetterling and Brian P. Flannery,  University of Cambridge Press 1992.  
        /// </remarks>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <param name="l">Нижне-треугольная матрица</param>
        /// <param name="u">Верхне-треугольная матрица</param>
        /// <param name="p">Матрица преобразований P*X = L*U</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Истина, если процедура декомпозиции прошла успешно. Ложь, если матрица вырождена</returns>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу <paramref name="matrix"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если размерность матрицы <paramref name="matrix"/> меньше 1</exception>
        public static bool GetLUPDecomposition
        (
            double[,] matrix,
            out double[,]? l,
            out double[,]? u,
            out double[,]? p,
            out double d
        )
        {
            GetRowsCount(matrix, out var N);

            l = GetUnitaryArrayMatrix(N);  // L - изначально единичная матрица
            u = matrix.CloneObject();      // U - изначально клон исходной матрицы
            d = 1d;                         // Начальное значение определителя матрицы - единичный элемент относительно операции умножения

            var p_index = new int[N]; for (var i = 0; i < N; i++) p_index[i] = i; // Создаём вектор коммутации

            for (var j = 0; j < N; j++)
            {
                var max = 0d;
                var max_index = -1;
                for (var i = j; i < N; i++)      // Ищем строку с максимальным ведущим элементом
                {
                    var abs = Math.Abs(u[i, j]); // Ищем элемент в строке, максимальный по модулю
                    if (abs <= max) continue;
                    max = abs;
                    max_index = i;
                }

                if (max_index == -1)             // Если индекс ведущего элемента не изменился, то матрица вырождена
                {
                    l = null;                   // Очищаем выходные переменные
                    u = null;
                    p = null;
                    d = 0d;                      // Приравниваем определитель к нулю
                    return false;               // Возвращаем ложь - операция не может быть выполнена
                }

                if (max_index != j)              // Если ведущий элемент был найден
                {
                    Swap(ref p_index[j], ref p_index[max_index]); // Переставляем строки для ведущего элемента в векторе коммутации
                    u.SwapRows(max_index, j);
                    d = -d;
                }

                var main = u[j, j];             // Определяем ведущий элемент строки
                d *= main;                      // Умножаем определитель на ведущий элемент
                for (var i = j + 1; i < N; i++)  // Для всех строк ниже текущей
                {
                    l[i, j] = u[i, j] / main; // Проводим операции над присоединённой матрицей
                    for (var k = 0; k <= j; k++) u[i, k] = 0d; // Очищаем начало очередной строки
                    if (l[i, j] == 0) continue; // Если очередной ведущий элемент строки уже ноль, то пропускаем её
                    for (var k = j + 1; k < N; k++) // Вычитаем из элементов строки ...
                        u[i, k] -= l[i, j] * u[j, k];
                }
            }
            p = CreatePermutationMatrix(p_index); // Создаём матрицу коммутации из вектора коммутации
            return true;
        }

        /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
        /// <remarks>
        /// This method is based on the 'LU Decomposition and Its Applications' 
        /// section of Numerical Recipes in C by William H. Press, Saul A. Teukolsky, William T. 
        /// Vetterling and Brian P. Flannery,  University of Cambridge Press 1992.  
        /// </remarks>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <param name="l">Нижне-треугольная матрица</param>
        /// <param name="u">Верхне-треугольная матрица</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Истина, если процедура декомпозиции прошла успешно. Ложь, если матрица вырождена</returns>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу matrix</exception>
        /// <exception cref="ArgumentOutOfRangeException">В случае если размерность матрицы N меньше 1</exception>
        public static bool GetLUDecomposition(double[,] matrix, out double[,]? l, out double[,]? u, out double d)
        {
            GetRowsCount(matrix, out var N);

            l = GetUnitaryArrayMatrix(N);
            u = matrix.CloneObject();
            d = 1d;

            for (var j = 0; j < N; j++)
            {
                if (u[j, j] == 0)
                {
                    l = null;
                    u = null;
                    d = 0d;
                    return false;
                }
                var main = u[j, j];
                d *= main;
                for (var i = j + 1; i < N; i++)
                {
                    l[i, j] = u[i, j] / main;
                    for (var k = 0; k <= j; k++) u[i, k] = 0d;
                    if (l[i, j] == 0) continue;
                    for (var k = j + 1; k < N; k++) u[i, k] -= l[i, j] * u[j, k];
                }
            }
            return true;
        }


        /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
        /// <remarks>
        /// This method is based on the 'LU Decomposition and Its Applications' 
        /// section of Numerical Recipes in C by William H. Press, Saul A. Teukolsky, William T. 
        /// Vetterling and Brian P. Flannery,  University of Cambridge Press 1992.  
        /// </remarks>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <param name="c">Матрица с результатами разложения: элементы ниже главной диагонали - матрица L, элементы выше - матрица U</param>
        /// <param name="p">Массив матрицы перестановок</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Истина, если операция выполнена успешно</returns>
        /// <exception cref="ArgumentException">Матрица не квадратная</exception>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу matrix</exception>
        public static bool GetLUPDecomposition(
            double[,] matrix,
            out double[,]? c,
            out double[,]? p,
            out double d)
        {
            GetRowsCount(matrix, out var N);
            if (N != matrix.GetLength(1))
                throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));

            c = matrix.CloneObject();
            d = 1d;

            var p_index = new int[N];
            for (var i = 0; i < N; i++) p_index[i] = i;

            for (var j = 0; j < N; j++)
            {
                //поиск опорного элемента
                var max = 0d;
                var max_index = -1;
                for (var i = j; i < N; i++)
                {
                    var abs = Math.Abs(c[i, j]);
                    if (abs <= max) continue;
                    max = abs;
                    max_index = i;
                }
                if (max_index < 0)
                {
                    p = null;
                    c = null;
                    d = 0d;
                    return false;
                }
                if (max_index != j)
                {
                    c.SwapRows(max_index, j);
                    Swap(ref p_index[max_index], ref p_index[j]);
                    d = -d;
                }
                var main = c[j, j];
                d *= main;
                for (var i = j + 1; i < N; i++)
                {
                    c[i, j] /= main;
                    if (c[i, j] == 0) continue;
                    for (var k = j + 1; k < N; k++) c[i, k] -= c[i, j] * c[j, k];
                }
            }
            p = CreatePermutationMatrix(p_index);
            return true;
        }

        /// <summary>LU-разложение матрицы</summary>
        /// <param name="matrix">Разлагаемая матрица</param>
        /// <param name="c">Матрица с результатами разложения: элементы ниже главной диагонали - матрица L, элементы выше - матрица U</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Истина, если процедура выполнена успешно</returns>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу matrix</exception>
        /// <exception cref="ArgumentException">Матрица не квадратная</exception>
        public static bool GetLUPDecomposition(double[,] matrix, out double[,]? c, out double d)
        {
            GetRowsCount(matrix, out var N);
            if (N != matrix.GetLength(1))
                throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));

            c = matrix.CloneObject();
            d = 1d;

            for (var j = 0; j < N; j++)
            {
                //поиск опорного элемента
                var max = 0d;
                var max_index = -1;
                for (var i = j; i < N; i++)
                {
                    var abs = Math.Abs(c[i, j]);
                    if (abs <= max) continue;
                    max = abs;
                    max_index = i;
                }
                if (max_index < 0) { c = null; d = 0d; return false; }
                if (max_index != j) { c.SwapRows(max_index, j); d = -d; }
                var main = c[j, j];
                d *= main;
                for (var i = j + 1; i < N; i++)
                {
                    c[i, j] /= main;
                    if (c[i, j] == 0) continue;
                    for (var k = j + 1; k < N; k++) c[i, k] -= c[i, j] * c[j, k];
                }
            }

            return true;
        }

        /// <summary>LU-разложение матрицы</summary>
        /// <param name="matrix">Разлагаемая матрица</param>
        /// <param name="c">Матрица с результатами разложения: элементы ниже главной диагонали - матрица L, элементы выше - матрица U</param>
        /// <returns>Истина, если разложение выполнено успешно</returns>
        /// <exception cref="ArgumentNullException">В случае если отсутствует ссылка на матрицу matrix</exception>
        /// <exception cref="ArgumentException">Матрица не квадратная</exception>
        public static bool GetLUDecomposition(double[,] matrix, out double[,]? c)
        {
            GetRowsCount(matrix, out var N);
            if (N != matrix.GetLength(1)) 
                throw new ArgumentException(@"Матрица не квадратная", nameof(matrix));

            c = matrix.CloneObject();

            for (var j = 0; j < N; j++)
            {
                if (c[j, j] == 0) { c = null; return false; }
                for (var i = j + 1; i < N; i++)
                {
                    c[i, j] /= c[j, j];
                    if (c[i, j] == 0) continue;
                    for (var k = j + 1; k < N; k++) c[i, k] -= c[i, j] * c[j, k];
                }
            }
            return true;
        }

        /// <summary>Создать матрицу перестановок из массива индексов</summary>
        /// <param name="indexes">Массив индексов элементов столбцов</param>
        /// <returns>Матрица перестановок</returns>
        // ReSharper disable once SuggestBaseTypeForParameter
        private static double[,] CreatePermutationMatrix(int[] indexes)
        {
            var N = indexes.Length;
            var P = new double[N, N];
            for (var i = 0; i < N; i++)
                P[i, indexes[i]] = 1;
            return P;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="p">Матрица перестановок</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Треугольная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        public static double[,] GetTriangle(double[,] matrix, out double[,] p, out int rank, out double d)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));

            var result = matrix.CloneObject();
            rank = Triangulate(result, out p, out d);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Треугольная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        public static double[,] GetTriangle(double[,] matrix, out int rank, out double d)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));

            var result = matrix.CloneObject();
            rank = Triangulate(result, out d);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="b">Матрица правой части</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="d">Определитель матрицы</param>
        /// <returns>Треугольная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="b"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static double[,] GetTriangle(double[,] matrix, double[,] b, out int rank, out double d)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (b is null) throw new ArgumentNullException(nameof(b));

            var result = matrix.CloneObject();
            rank = Triangulate(result, b, out d);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
        /// <param name="matrix">Двумерный массив элементов матрицы</param>
        /// <param name="b">Матрица правой части</param>
        /// <param name="p">Матрица перестановок</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="d">Определитель матрицы</param>
        /// <param name="clone_b">Клонировать матрицу правых частей</param>
        /// <returns>Треугольная матрица</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="b"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static double[,] GetTriangle
        (
            double[,] matrix,
            ref double[,] b,
            out double[,] p,
            out int rank,
            out double d,
            bool clone_b = true
        )
        {
            rank = Triangulate(ref matrix, ref b, out p, out d, false, clone_b);
            return matrix;
        }

        /// <summary>Приведение матрицы к треугольному виду</summary>
        /// <param name="matrix">Матрица, приводимая к треугольному виду</param>
        /// <param name="p">Матрица перестановок</param>
        /// <param name="d">Определитель матрицы (произведение диагональных элементов)</param>
        /// <returns>Ранг матрицы (число ненулевых строк)</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        public static int Triangulate(double[,] matrix, out double[,] p, out double d)
        {
            GetLength(matrix, out var N, out var M);
            d = 1d;

            var p_index = new int[N];
            for (var i = 0; i < N; i++)
                p_index[i] = i;

            var N1 = Math.Min(N, M);
            for (var i0 = 0; i0 < N1; i0++)
            {
                if (matrix[i0, i0] == 0)
                {
                    var nonzero_index = i0 + 1;
                    while (nonzero_index < N && matrix[nonzero_index, i0] == 0) nonzero_index++;
                    if (nonzero_index == N)
                    {
                        p = CreatePermutationMatrix(p_index);
                        for (var i = i0; i < N; i++)
                            for (var j = i0; j < M; j++)
                                matrix[i, j] = 0d;

                        d = 0d;
                        return i0;
                    }
                    matrix.SwapRows(i0, nonzero_index);
                    Swap(ref p_index[i0], ref p_index[nonzero_index]);
                    d = -d;
                }

                var main = matrix[i0, i0]; // Ведущий элемент строки
                d *= main;

                //Нормируем строку основной матрицы по первому элементу
                for (var i = i0 + 1; i < N; i++)
                    if (matrix[i, i0] != 0)
                    {
                        var k = matrix[i, i0] / main;
                        matrix[i, i0] = 0d;

                        for (var j = i0 + 1; j < M; j++)
                            matrix[i, j] -= matrix[i0, j] * k;
                    }
            }

            p = CreatePermutationMatrix(p_index);
            return N1;
        }

        /// <summary>Приведение матрицы к треугольному виду</summary>
        /// <param name="matrix">Матрица, приводимая к треугольному виду</param>
        /// <param name="d">Определитель матрицы (произведение диагональных элементов)</param>
        /// <returns>Ранг матрицы (число ненулевых строк)</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        public static int Triangulate(double[,] matrix, out double d)
        {
            GetLength(matrix, out var N, out var M);
            d = 1d;
            var rank = Math.Min(N, M);

            for (var i0 = 0; i0 < rank; i0++)
            {
                if (matrix[i0, i0] == 0)
                {
                    var max = 0d;
                    var max_index = -1;
                    for (var i1 = i0 + 1; i1 < N; i1++)
                    {
                        var abs = Math.Abs(matrix[i1, i0]);
                        if (abs > max)
                        {
                            max = abs;
                            max_index = i1;
                        }
                    }

                    if (max_index < 0) // матрица вырождена?
                    {
                        for (var i = i0; i < N; i++)
                            for (var j = i0; j < M; j++)
                                matrix[i, j] = 0d;
                        d = 0d;
                        return i0;
                    }

                    matrix.SwapRows(i0, max_index);
                    d = -d;
                }

                var main = matrix[i0, i0]; // Ведущий элемент строки
                d *= main;

                //Нормируем строку основной матрицы по первому элементу
                for (var i = i0 + 1; i < N; i++)
                    if (matrix[i, i0] != 0)
                    {
                        var k = matrix[i, i0] / main;
                        matrix[i, i0] = 0d;
                        for (var j = i0 + 1; j < M; j++)
                            matrix[i, j] -= matrix[i0, j] * k;
                    }
            }

            return rank;
        }

        /// <summary>Приведение матрицы к треугольному виду</summary>
        /// <param name="matrix">Матрица, приводимая к треугольному виду</param>
        /// <param name="b">Присоединённая матрица, над которой выполняются те же операции, что и над <paramref name="matrix"/></param>
        /// <param name="d">Определитель матрицы (произведение диагональных элементов)</param>
        /// <returns>Ранг матрицы (число ненулевых строк)</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="b"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static int Triangulate(double[,] matrix, double[,] b, out double d)
        {
            GetLength(matrix, out var N, out var M);
            if (b is null) throw new ArgumentNullException(nameof(b));
            GetLength(b, out var B_N, out var B_M);
            if (B_N != N) throw new ArgumentException(@"Число строк присоединённой матрицы не равно числу строк исходной матрицы");

            d = 1d;
            var rank = Math.Min(N, M);
            for (var i0 = 0; i0 < rank; i0++)
            {
                if (matrix[i0, i0] == 0)
                {
                    var max = 0d;
                    var max_index = -1;
                    for (var i1 = i0 + 1; i1 < N; i1++)
                    {
                        var abs = Math.Abs(matrix[i1, i0]);
                        if (abs > max)
                        {
                            max = abs;
                            max_index = i1;
                        }
                    }

                    if (max_index < 0)
                    {
                        for (var i = i0; i < N; i++)
                        {
                            for (var j = i0; j < M; j++) matrix[i, j] = 0d;
                            for (var j = 0; j < B_M; j++) b[i, j] = 0d;
                        }
                        d = 0d;
                        return i0;
                    }
                    matrix.SwapRows(i0, max_index);
                    b.SwapRows(i0, max_index);
                    d = -d;
                }

                var main = matrix[i0, i0]; // Ведущий элемент строки
                d *= main;
                //Нормируем строку основной матрицы по первому элементу
                for (var i = i0 + 1; i < N; i++)
                    if (matrix[i, i0] != 0)
                    {
                        var k = matrix[i, i0] / main;
                        matrix[i, i0] = 0d;

                        for (var j = i0 + 1; j < M; j++)
                            matrix[i, j] -= matrix[i0, j] * k;
                        for (var j = 0; j < B_M; j++)
                            b[i, j] -= b[i0, j] * k;
                    }
            }
            if (rank >= N)
                return rank;

            for (var i = rank; i < N; i++)
                for (var j = 0; j < B_M; j++)
                    b[i, j] = 0d;
            return rank;
        }

        /// <summary>Приведение матрицы к треугольному виду</summary>
        /// <param name="matrix">Матрица, приводимая к треугольному виду</param>
        /// <param name="b">Присоединённая матрица, над которой выполняются те же операции, что и над <paramref name="matrix"/></param>
        /// <param name="d">Определитель матрицы (произведение диагональных элементов)</param>
        /// <param name="clone">Клонировать исходную матрицу</param>
        /// <returns>Ранг матрицы (число ненулевых строк)</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="b"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static int Triangulate(ref double[,] matrix, double[,] b, out double d, bool clone = true)
        {
            GetRowsCount(matrix, out var N);
            if (b is null) throw new ArgumentNullException(nameof(b));
            GetRowsCount(b, out var B_N);
            if (B_N != N) throw new ArgumentException(@"Число строк присоединённой матрицы не равно числу строк исходной матрицы");

            return Triangulate(clone ? matrix.CloneObject() : matrix, b, out d);
        }

        /// <summary>Приведение матрицы к треугольному виду</summary>
        /// <param name="matrix">Матрица, приводимая к треугольному виду</param>
        /// <param name="b">Присоединённая матрица, над которой выполняются те же операции, что и над <paramref name="matrix"/></param>
        /// <param name="p">Матрица перестановок</param>
        /// <param name="d">Определитель матрицы (произведение диагональных элементов)</param>
        /// <param name="clone_matrix">Клонировать исходную матрицу</param>
        /// <param name="clone_b">Клонировать присоединённую матрицу</param>
        /// <returns>Ранг матрицы (число ненулевых строк)</returns>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="matrix"/> не задана</exception>
        /// <exception cref="ArgumentNullException">В случае если матрица <paramref name="b"/> не задана</exception>
        /// <exception cref="ArgumentException">В случае если число строк присоединённой матрицы <paramref name="b"/> не равно числу строк исходной матрицы <paramref name="matrix"/></exception>
        public static int Triangulate
        (
            ref double[,] matrix,
            ref double[,] b,
            out double[,] p,
            out double d,
            bool clone_matrix = true,
            bool clone_b = true
        )
        {
            GetLength(matrix, out var N, out var M);
            if (b is null) throw new ArgumentNullException(nameof(b));
            GetLength(b, out var B_N, out var B_M);
            if (B_N != N) throw new ArgumentException(@"Число строк присоединённой матрицы не равно числу строк исходной матрицы");

            if (clone_matrix)
                matrix = matrix.CloneObject();
            if (clone_b)
                b = b.CloneObject();

            d = 1d;
            var p_index = new int[N];
            for (var i = 0; i < N; i++)
                p_index[i] = i;
            var N1 = Math.Min(N, M);
            for (var i0 = 0; i0 < N1; i0++)
            {
                if (matrix[i0, i0] == 0)
                {
                    var max = 0d;
                    var max_index = -1;
                    for (var i1 = i0 + 1; i1 < N; i1++)
                    {
                        var abs = Math.Abs(matrix[i1, i0]);
                        if (abs > max)
                        {
                            max = abs;
                            max_index = i1;
                        }
                    }

                    if (max_index < 0)
                    {
                        p = CreatePermutationMatrix(p_index);
                        for (var i = i0; i < N; i++)
                        {
                            for (var j = i0; j < M; j++)
                                matrix[i, j] = 0d;
                            for (var j = 0; j < B_M; j++)
                                b[i, j] = 0d;
                        }
                        d = 0d;
                        return i0;
                    }
                    matrix.SwapRows(i0, max_index);
                    b.SwapRows(i0, max_index);
                    Swap(ref p_index[i0], ref p_index[max_index]);
                    d = -d;
                }
                var main = matrix[i0, i0]; // Ведущий элемент строки
                d *= main;

                //Нормируем строку основной матрицы по первому элементу
                for (var i = i0 + 1; i < N; i++)
                    if (matrix[i, i0] != 0)
                    {
                        var k = matrix[i, i0] / main;
                        matrix[i, i0] = 0d;
                        for (var j = i0 + 1; j < M; j++)
                            matrix[i, j] -= matrix[i0, j] * k;
                        for (var j = 0; j < B_M; j++)
                            b[i, j] -= b[i0, j] * k;
                    }
            }
            p = CreatePermutationMatrix(p_index);
            if (N1 >= N) return N1;
            for (var i = N1; i < N; i++)
                for (var j = 0; j < B_M; j++)
                    b[i, j] = 0d;
            return N1;
        }

        /// <summary>Сравнение двух двумерных массивов элементов матриц</summary>
        /// <param name="A">Первый массив</param>
        /// <param name="B">Второй массив</param>
        /// <returns>Истина, если оба массивы не определены, либо если оба массивы - один и тот же массив, либо если элементы массивов идентичны</returns>
        /// <exception cref="ArgumentNullException">matrix is <see langword="null"/></exception>
        public static bool AreEquals(double[,]? A, double[,]? B)
        {
            if (ReferenceEquals(A, B)) return true;
            if (B is null || A is null) return false;

            GetLength(A, out var N, out var M);
            if (N != B.GetLength(0) || M != B.GetLength(1)) return false;

            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    if (!A[i, j].Equals(B[i, j]))
                        return false;
            return true;
        }

        /// <summary>Сравнение двух двумерных массивов элементов матриц</summary>
        /// <param name="A">Первый массив</param>
        /// <param name="B">Второй массив</param>
        /// <param name="eps">Точность сравнения</param>
        /// <returns>Истина, если оба массивы не определены, либо если оба массивы - один и тот же массив, либо если элементы массивов идентичны</returns>
        /// <exception cref="ArgumentNullException">matrix is <see langword="null"/></exception>
        public static bool AreEquals(double[,]? A, double[,]? B, double eps)
        {
            if (ReferenceEquals(A, B)) return true;
            if (B is null || A is null) return false;

            GetLength(A, out var N, out var M);
            if (N != B.GetLength(0) || M != B.GetLength(1)) return false;

            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++)
                    if (Math.Abs(A[i, j] - B[i, j]) > eps)
                        return false;
            return true;
        }

        /// <summary>Вычисление максимума от сумм абсолютных значений по элементам строк</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <returns>Максимальная из сумм абсолютных значений элементов строк</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        public static double GetMaxRowAbsSum(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            var max = 0d;
            for (var i = 0; i < N; i++)
            {
                var row_sum = 0d;
                for (var j = 0; j < M; j++)
                    row_sum += Math.Abs(matrix[i, j]);
                if (row_sum > max)
                    max = row_sum;
            }
            return max;
        }

        /// <summary>Вычисление максимума от сумм абсолютных значений по элементам столбцов</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <returns>Максимальная из сумм абсолютных значений элементов столбцов</returns>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        public static double GetMaxColAbsSum(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            var max = 0d;
            for (var j = 0; j < M; j++)
            {
                var col_sum = 0d;
                for (var i = 0; i < N; i++)
                    col_sum += Math.Abs(matrix[i, j]);
                if (col_sum > max)
                    max = col_sum;
            }
            return max;
        }

        /// <summary>Вычисление среднеквадратичного значения элементов матрицы</summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        /// <returns>Среднеквадратичное значение элементов матрицы</returns>
        /// <exception cref="ArgumentNullException">matrix is <see langword="null"/></exception>
        public static double GetRMS(double[,] matrix)
        {
            GetLength(matrix, out var N, out var M);
            var s = 0d;
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    s += matrix[i, j] * matrix[i, j];
            return Math.Sqrt(s);
        }

        /// <summary>Квадрат числа</summary>
        /// <param name="x">Значение, квадрат которого требуется получить</param>
        /// <returns>Квадрат указанного числа</returns>
        private static double Sqr(double x) => x * x;

        /// <summary>Метод проверки сходимости метода Метод Гаусса — Зейделя</summary>
        /// <remarks>Метод меняет местами матрицы решения текущего и прошлого шагов, если метод Гаусса — Зейделя не сошёлся на текущем шаге</remarks>
        /// <param name="new_x">Новое полученное решение</param>
        /// <param name="last_x">Решение, полученное на прошлом шаге метода</param>
        /// <param name="eps">Требуемая точность решения</param>
        /// <param name="N">Число строк матрицы решения</param>
        /// <param name="M">Число столбцов матрицы решения</param>
        /// <returns>Истина, если метод сошёлся Метод Гаусса — Зейделя</returns>
        private static bool GaussSeidelConverge
        (
            ref double[,] new_x,
            ref double[,] last_x,
            [GreaterOrEqual(double.Epsilon)] double eps,
            int N,
            int M
        )
        {
            var sum = 0d;
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    sum += Sqr(new_x[i, j] - last_x[i, j]);
            if (sum < eps * eps)
                return true;
            Swap(ref new_x, ref last_x);
            return false;
        }

        /// <summary>Метод Гаусса — Зейделя решения системы линейных уравнений</summary>
        /// <param name="matrix">Матрица коэффициентов</param>
        /// <param name="x">Матрица неизвестных</param>
        /// <param name="b">Матрица правых частей</param>
        /// <param name="eps">Требуемая точность</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="x"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="b"/> == <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица системы не содержит элементов</exception>
        /// <exception cref="ArgumentException">Матрица системы не квадратная</exception>
        /// <exception cref="ArgumentException">Число строк массива неизвестных не совпадает с числом строк матрицы системы</exception>
        /// <exception cref="ArgumentException">Число строк массива правой части СЛАУ не совпадает с числом строк матрицы системы</exception>
        /// <exception cref="ArgumentException">Число столбцов массива правых частей не совпадает с числом столбцов массива неизвестных</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="eps"/> &lt;= <see cref="double"/>.<see cref="double.Epsilon"/></exception> Solve
        public static void GaussSeidelSolve
        (
            double[,] matrix,
            double[,] x,
            double[,] b,
            [GreaterOrEqual(double.Epsilon)] double eps = double.Epsilon
        )
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.Length == 0) throw new ArgumentException(@"Матрица системы не содержит элементов", nameof(matrix));
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new ArgumentException(@"Матрица системы не квадратная", nameof(matrix));
            if (x is null) throw new ArgumentNullException(nameof(x));
            if (b is null) throw new ArgumentNullException(nameof(b));
            if (eps < double.Epsilon) throw new ArgumentOutOfRangeException(nameof(eps), eps, @"Точность должна быть больше 0");
            if (x.GetLength(0) != matrix.GetLength(0)) throw new ArgumentException(@"Число строк массива неизвестных не совпадает с числом строк матрицы системы", nameof(x));
            if (b.GetLength(0) != matrix.GetLength(0)) throw new ArgumentException(@"Число строк массива правой части СЛАУ не совпадает с числом строк матрицы системы", nameof(x));
            if (b.GetLength(1) != x.GetLength(1)) throw new ArgumentException(@"Число столбцов массива правых частей не совпадает с числом столбцов массива неизвестных", nameof(b));

            var x1 = x;
            var x0 = x1.CloneObject();
            GetRowsCount(matrix, out var N);
            GetLength(x1, out var x_N, out var x_M);

            do
                for (var j_x = 0; j_x < x_M; j_x++)
                    for (var i = 0; i < N; i++)
                    {
                        var d = 0d;
                        for (var j = 0; j < i; j++)
                            d += matrix[i, j] * x1[j, j_x];
                        for (var j = i + 1; j < N; j++)
                            d += matrix[i, j] * x0[j, j_x];
                        x1[i, j_x] = (b[i, j_x] - d) / matrix[i, i];
                    }
            while (!GaussSeidelConverge(ref x1, ref x0, eps, x_N, x_M));
            if (ReferenceEquals(x1, x))
                return;
            System.Array.Copy(x1, x, x.Length);
        }

        /// <summary>QR-разложение матрицы</summary>
        /// <param name="matrix">Разлагаемая матрица</param>
        /// <param name="q">Унитарная матрица (ортогональная) - должна быть передана квадратная матрица nxn != null</param>
        /// <param name="r">Верхне-треугольная матрица - должна быть передана квадратная матрица nxn != null</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица не содержит элементов</exception>
        public static void QRDecomposition(double[,] matrix, double[,] q, double[,] r)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.Length == 0) throw new ArgumentException(@"Матрица не содержит элементов", nameof(matrix));

            GetLength(matrix, out var N, out var M);

            var u = matrix.CloneObject();

            // По столбцам матрицы matrix (j - номер обрабатываемого столбца)
            for (var j = 0; j < M; j++)
            {
                double n;
                // По предыдущим столбцам матрицы u (j - номер обрабатываемого столбца)
                for (var k = 0; k < j; k++)
                {
                    var s = default(double);
                    n = default; // скалярное произведение столбца на самого себя
                    // Вычисление скалярного произведения v*u и квадрата длины u
                    for (var i = 0; i < N; i++)
                    {
                        var v = u[i, k];
                        s += matrix[i, j] * v;
                        n += v * v;
                    }
                    s /= n;
                    // Вычитание проекции
                    for (var i = 0; i < N; i++)
                        u[i, j] -= u[i, k] * s;
                }
                // Вычисление нормированного вектора
                n = default;
                for (var i = 0; i < N; i++)
                    n += u[i, j] * u[i, j];
                n = Math.Sqrt(n);
                for (var i = 0; i < N; i++)
                    q[i, j] = u[i, j] / n;
            }

            for (var i = 0; i < N; i++)  // Произведение qT*matrix
                for (var j = i; j < M; j++)
                {
                    var s = default(double);
                    for (var k = 0; k < N; k++)
                        s += q[k, i] * matrix[k, j];
                    r[i, j] = s;
                }
        }

        /// <summary>QR-разложение матрицы</summary>
        /// <param name="matrix">Разлагаемая матрица</param>
        /// <param name="q">Унитарная матрица (ортогональная) - создаётся квадратная матрица nxn != null</param>
        /// <param name="r">Верхне-треугольная матрица - создаётся квадратная матрица nxn != null</param>
        /// <exception cref="ArgumentNullException"><paramref name="matrix"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Матрица не содержит элементов</exception>
        public static void QRDecomposition(double[,] matrix, out double[,] q, out double[,] r)
        {
            if (matrix is null) throw new ArgumentNullException(nameof(matrix));
            if (matrix.Length == 0) throw new ArgumentException(@"Матрица не содержит элементов", nameof(matrix));

            GetLength(matrix, out var N, out var M);
            q = new double[N, M];
            r = new double[N, M];
            QRDecomposition(matrix, q, r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double PyThag(double a, double b)
        {
            var abs_a = Math.Abs(a);
            var abs_b = Math.Abs(b);
            return abs_a > abs_b ? abs_a * Math.Sqrt(1d + Sqr(abs_b / abs_a)) :
                abs_b == 0 ? 0d : abs_b * Math.Sqrt(1d + Sqr(abs_a / abs_b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Sign(double a, double b) => b >= 0d ? Math.Abs(a) : -Math.Abs(a);

        /// <summary>SVD-разложение</summary>
        /// <param name="matrix">Разлагаемая матрица</param>
        /// <param name="u">Матрица левых сингулярных векторов</param>
        /// <param name="w">Вектор собственных чисел</param>
        /// <param name="v">Матрица правых сингулярных векторов</param>
        /// <exception cref="ArgumentNullException">matrix is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Метод не сошёлся за 30 итераций</exception>
        // ReSharper disable once FunctionComplexityOverflow
        // ReSharper disable once CyclomaticComplexity
        public static void SVD(double[,] matrix, out double[,] u, out double[] w, out double[,] v)
        {
            GetLength(matrix, out var N, out var M);

            if (M > N)
            {
                SVD(Transpose(matrix), out v, out w, out u);
                return;
            }

            u = matrix.CloneObject();
            w = new double[Math.Min(N, M)];
            v = new double[w.Length, w.Length];

            var a_norm = 0d;
            var scale = 0d;
            var g = 0d;

            var rv1 = new double[M];
            /* Householder reduction to bidiagonal form */
            for (var i = 0; i < M; i++)
            {
                var l = i + 1;
                rv1[i] = scale * g;
                g = scale = 0d;
                if (i < N)
                {
                    for (var k = i; k < N; k++)
                        scale += Math.Abs(u[k, i]);
                    if (scale != 0)
                    {
                        var s = 0d;
                        for (var k = i; k < N; k++)
                        {
                            u[k, i] /= scale;
                            s += u[k, i] * u[k, i];
                        }
                        var f = u[i, i];
                        g = -Sign(Math.Sqrt(s), f);
                        var h = f * g - s;
                        u[i, i] = f - g;
                        for (var j = l; j < M; j++)
                        {
                            s = 0d;
                            for (var k = i; k < N; k++)
                                s += u[k, i] * u[k, j];
                            f = s / h;
                            for (var k = i; k < N; k++)
                                u[k, j] += f * u[k, i];
                        }
                        for (var k = i; k < N; k++)
                            u[k, i] *= scale;
                    }
                }
                w[i] = scale * g;
                g = scale = 0d;
                if (i < N && i != M)  // i != M-1 ?
                {
                    for (var k = l; k < M; k++)
                        scale += Math.Abs(u[i, k]);
                    if (scale != 0)
                    {
                        var s = 0d;
                        for (var k = l; k < M; k++)
                        {
                            u[i, k] /= scale;
                            s += u[i, k] * u[i, k];
                        }
                        var f = u[i, l];
                        g = -Sign(Math.Sqrt(s), f);
                        var h = f * g - s;
                        u[i, l] = f - g;
                        for (var k = l; k < M; k++)
                            rv1[k] = u[i, k] / h;
                        for (var j = l; j < N; j++)
                        {
                            s = 0d;
                            for (var k = l; k < M; k++)
                                s += u[j, k] * u[i, k];
                            for (var k = l; k < M; k++)
                                u[j, k] += s * rv1[k];
                        }
                        for (var k = l; k < M; k++)
                            u[i, k] *= scale;
                    }
                }
                a_norm = Math.Max(a_norm, Math.Abs(w[i]) + Math.Abs(rv1[i]));
            }
            /* Accumulation of right-hand transformations. */
            for (var i = M - 1; i >= 0; i--)
            {
                var l = i + 1;
                if (i < M - 1)
                {
                    if (g != 0)
                    {
                        /* Double division to avoid possible underflow. */
                        for (var j = l; j < M; j++)
                            v[j, i] = u[i, j] / u[i, l] / g;
                        for (var j = l; j < M; j++)
                        {
                            var s = 0d;
                            for (var k = l; k < M; k++)
                                s += u[i, k] * v[k, j];
                            for (var k = l; k < M; k++)
                                v[k, j] += s * v[k, i];
                        }
                    }
                    for (var j = l; j < M; j++)
                        v[i, j] = v[j, i] = 0d;
                }
                v[i, i] = 1d;
                g = rv1[i];
            }
            /* Accumulation of left-hand transformations. */
            for (var i = Math.Min(N, M) - 1; i >= 0; i--)
            {
                var l = i + 1;
                g = w[i];
                for (var j = l; j < M; j++)
                    u[i, j] = 0d;
                if (g != 0)
                {
                    g = 1d / g;
                    for (var j = l; j < M; j++)
                    {
                        var s = 0d;
                        for (var k = l; k < N; k++)
                            s += u[k, i] * u[k, j];
                        var f = s / u[i, i] * g;
                        for (var k = i; k < N; k++)
                            u[k, j] += f * u[k, i];
                    }
                    for (var j = i; j < N; j++)
                        u[j, i] *= g;
                }
                else for (var j = i; j < N; j++)
                        u[j, i] = 0d;
                u[i, i]++;
            }
            /* Diagonalization of the bidiagonal form. */
            for (var k = M; k >= 1; k--)
                for (var its = 1; its <= 30; its++)
                {
                    int l;
                    var flag = true;
                    var nm = 0;
                    /* Test for splitting. */
                    for (l = k; l >= 1; l--)
                    {
                        nm = l - 1;
                        /* Note that rv1[0] is always zero. */
                        if ((Math.Abs(rv1[l - 1]) + a_norm).Equals(a_norm))
                        {
                            flag = false;
                            break;
                        }
                        if ((Math.Abs(w[nm - 1]) + a_norm).Equals(a_norm))
                            break;
                    }
                    double c;
                    double y;
                    double z;
                    double s;
                    double f;
                    double h;
                    if (flag)
                    {
                        c = 0d; /* Cancellation of rv1[l-1], if l > 1. */
                        s = 1d;
                        for (var i = l; i <= k; i++)
                        {
                            f = s * rv1[i - 1];
                            rv1[i - 1] = c * rv1[i - 1];
                            if ((Math.Abs(f) + a_norm).Equals(a_norm))
                                break;
                            g = w[i - 1];
                            h = PyThag(f, g);
                            w[i - 1] = h;
                            h = 1d / h;
                            c = g * h;
                            s = -f * h;
                            for (var j = 0; j < N; j++)
                            {
                                y = u[j, nm - 1];
                                z = u[j, i - 1];
                                u[j, nm - 1] = y * c + z * s;
                                u[j, i - 1] = z * c - y * s;
                            }
                        }
                    }
                    z = w[k - 1];
                    /* Convergence. */
                    if (l == k)
                    {
                        /* Singular value is made nonnegative. */
                        if (z < 0d)
                        {
                            w[k - 1] = -z;
                            for (var j = 0; j < M; j++)
                                v[j, k - 1] = -v[j, k - 1];
                        }
                        break;
                    }
                    if (its == 30)
                        throw new InvalidOperationException("Метод не сошёлся за 30 итераций");
                    var x = w[l - 1];
                    nm = k - 1;
                    y = w[nm - 1];
                    g = rv1[nm - 1];
                    h = rv1[k - 1];
                    f = ((y - z) * (y + z) + (g - h) * (g + h)) / (2 * h * y);
                    g = PyThag(f, 1d);
                    f = ((x - z) * (x + z) + h * (y / (f + Sign(g, f)) - h)) / x;
                    c = 1d;
                    s = 1d;
                    /* Next QR transformation: */
                    for (var j0 = l - 1; j0 < nm; j0++)
                    {
                        var i = j0 + 1;
                        g = rv1[i];
                        y = w[i];
                        h = s * g;
                        g = c * g;
                        z = PyThag(f, h);
                        rv1[j0] = z;
                        c = f / z;
                        s = h / z;
                        f = x * c + g * s;
                        g = g * c - x * s;
                        h = y * s;
                        y *= c;
                        for (var j = 0; j < M; j++)
                        {
                            x = v[j, j0];
                            z = v[j, i];
                            v[j, j0] = x * c + z * s;
                            v[j, i] = z * c - x * s;
                        }
                        z = PyThag(f, h);
                        w[j0] = z;
                        /* Rotation can be arbitrary if z = 0. */
                        if (z != 0)
                        {
                            z = 1 / z;
                            c = f * z;
                            s = h * z;
                        }
                        f = c * g + s * y;
                        x = c * y - s * g;
                        for (var j = 0; j < N; j++)
                        {
                            y = u[j, j0];
                            z = u[j, i];
                            u[j, j0] = y * c + z * s;
                            u[j, i] = z * c - y * s;
                        }
                    }
                    rv1[l - 1] = 0d;
                    rv1[k - 1] = f;
                    w[k - 1] = x;
                }

            M = w.Length;
            if (M <= 2) return;
            var is_correct = true;
            for (var i = 0; i < M - 1 && is_correct; i++)
                if (w[i] < w[i + 1])
                    is_correct = false;
            if (is_correct) return;

            var M05 = (M - 1) >> 1;
            double t;
            for (var i = 1; i <= M05; i++)
            {
                t = w[i];
                w[i] = w[M - i];
                w[M - i] = t;
            }
            GetLength(u, out N, out M);
            M05 = (M - 1) >> 1;
            for (var i = 0; i < N; i++)
                for (var j = 1; j <= M05; j++)
                {
                    t = u[i, j];
                    u[i, j] = u[i, M - j];
                    u[i, M - j] = t;
                }
            GetLength(v, out N, out M);
            M05 = (M - 1) >> 1;
            for (var i = 0; i < N; i++)
                for (var j = 1; j <= M05; j++)
                {
                    t = v[i, j];
                    v[i, j] = v[i, M - j];
                    v[i, M - j] = t;
                }
        }

        public static partial class Operator
        {
            /// <summary>Скалярное произведение векторов</summary>
            /// <param name="v1">Первый множитель скалярного произведения</param>
            /// <param name="v2">второй множитель скалярного произведения</param>
            /// <returns>Скалярное произведение векторов</returns>
            public static double Multiply(double[] v1, double[] v2)
            {
                if (v1 is null) throw new ArgumentNullException(nameof(v1));
                if (v2 is null) throw new ArgumentNullException(nameof(v2));
                if (v1.Length != v2.Length) throw new ArgumentException(@"Длины векторов не совпадают", nameof(v2));

                var s = default(double);
                var N = v1.Length;
                for (var i = 0; i < N; i++)
                    s += v1[i] * v2[i];
                return s;
            }

            /// <summary>Длина вектора</summary>
            /// <param name="v">Вектор элементов</param>
            /// <returns>Длина вектора</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v"/> is <see langword="null"/></exception>
            public static double VectorLength(double[] v)
            {
                if (v is null) throw new ArgumentNullException(nameof(v));

                var s = default(double);
                for (var i = 0; i < v.Length; i++)
                    s += v[i] * v[i];
                return Math.Sqrt(s);
            }

            /// <summary>Умножение вектора на число</summary>
            /// <param name="v1">Первый сомножитель - вектор элементов</param>
            /// <param name="v2">Второй сомножитель - число, на которое должны быть умножены все элементы вектора</param>
            /// <returns>Вектор произведений элементов входного вектора и числа</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v1"/> is <see langword="null"/></exception>
            public static double[] Multiply(double[] v1, double v2)
            {
                if (v1 is null) throw new ArgumentNullException(nameof(v1));

                var s = new double[v1.Length];
                var N = s.Length;
                for (var i = 0; i < N; i++)
                    s[i] = v1[i] * v2;
                return s;
            }

            /// <summary>Деление вектора элементов на число</summary>
            /// <param name="v1">Вектор-делимое</param>
            /// <param name="v2">Число-делитель</param>
            /// <returns>Вектор, составленный из частного элементов вектора-делимого и числового делителя</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v1"/> is <see langword="null"/></exception>
            public static double[] Divide(double[] v1, double v2)
            {
                if (v1 is null)
                    throw new ArgumentNullException(nameof(v1));

                var s = new double[v1.Length];
                var N = s.Length;
                for (var i = 0; i < N; i++)
                    s[i] = v1[i] / v2;
                return s;
            }

            /// <summary>Проекция вектора на вектор</summary>
            /// <returns>Вектор - произведение компонентов исходных векторов</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v1"/> or <paramref name="v2"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Длины векторов не совпадают</exception>
            public static double[] Projection(double[] v1, double[] v2)
            {
                if (v1 is null)
                    throw new ArgumentNullException(nameof(v1));
                if (v2 is null)
                    throw new ArgumentNullException(nameof(v2));
                if (v1.Length != v2.Length)
                    throw new ArgumentException(@"Длины векторов не совпадают", nameof(v2));

                var result = new double[v2.Length];
                var m = default(double);
                var v2_length2 = default(double);
                var N = v1.Length;
                for (var i = 0; i < N; i++)
                {
                    m += v1[i] * v2[i];
                    v2_length2 += v2[i] * v2[i];
                }
                for (var i = 0; i < N; i++)
                    result[i] = v2[i] * m / v2_length2;
                return v2;
            }

            /// <summary>Оператор вычисления суммы двумерного массива элементов матрицы с числом</summary>
            /// <param name="matrix">Массив элементов матрицы</param>
            /// <param name="x">Число</param>
            /// <returns>Массив суммы элементов матрицы с числом</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="matrix"/> не определена</exception>
            public static double[,] Add(double[,] matrix, double x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = matrix[i, j] + x;
                return result;
            }

            /// <summary>Поэлементное сложение двух матриц</summary>
            /// <param name="a">Матрица - первое слагаемое</param>
            /// <param name="b">Матрица - второе слагаемое</param>
            /// <returns>Матрица, составленная из элементов - сумм элементов исходных матриц</returns>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> or <paramref name="b"/> is <see langword="null"/></exception>
            public static double[] Add(double[] a, double[] b)
            {
                if (a is null)
                    throw new ArgumentNullException(nameof(a));
                if (b is null)
                    throw new ArgumentNullException(nameof(b));

                var result = new double[a.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = a[i] + b[i];
                return a;
            }

            /// <summary>Оператор вычитания между двумя столбцами</summary>
            /// <param name="a">Столбец уменьшаемого</param>
            /// <param name="b">Столбец вычитаемого</param>
            /// <returns>Вектор-столбец разности указанных векторов</returns>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> or <paramref name="b"/> is <see langword="null"/></exception>
            /// <exception cref="InvalidOperationException">Размеры векторов не совпадают</exception>
            public static double[] Subtract(double[] a, double[] b)
            {
                if (a is null)
                    throw new ArgumentNullException(nameof(a));
                if (b is null)
                    throw new ArgumentNullException(nameof(b));
                if (a.Length != b.Length)
                    throw new InvalidOperationException(@"Размеры векторов не совпадают");

                var result = new double[a.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = a[i] - b[i];
                return result;
            }

            /// <summary>Оператор вычисления поэлементного произведения двух векторов</summary>
            /// <param name="a">Вектор элементов первого множителя</param>
            /// <param name="b">Вектор элементов второго множителя</param>
            /// <returns>Вектор элементов произведения</returns>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> or <paramref name="b"/> is <see langword="null"/></exception>
            /// <exception cref="InvalidOperationException">Размеры векторов не совпадают</exception>
            public static double[] MultiplyElements(double[] a, double[] b)
            {
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (b is null) throw new ArgumentNullException(nameof(b));
                if (a.Length != b.Length) throw new InvalidOperationException(@"Размеры векторов не совпадают");

                var result = new double[a.Length];
                for (var i = 0; i < a.Length; i++)
                    result[i] = a[i] * b[i];
                return result;
            }

            /// <summary>Оператор вычисления поэлементного деления двух векторов</summary>
            /// <param name="a">Вектор - делимое</param>
            /// <param name="b">Вектор - делитель</param>
            /// <returns>Вектор, составленный из поэлементного частного элементов векторов делимого и делителя</returns>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> or <paramref name="b"/> is <see langword="null"/></exception>
            public static double[] DivideElements(double[] a, double[] b)
            {
                if (a is null)
                    throw new ArgumentNullException(nameof(a));
                if (b is null)
                    throw new ArgumentNullException(nameof(b));

                var result = new double[a.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = a[i] / b[i];
                return result;
            }

            public static double[,] MultiplyElements(double[,] A, double[,] B)
            {
                GetLength(A, out var rows_a, out var cols_a);
                GetLength(B, out var rows_b, out var cols_b);

                if (rows_a != rows_b) throw new InvalidOperationException("Число строк матриц не совпадает");
                if (cols_a != cols_b) throw new InvalidOperationException("Число столбцов матриц не совпадает");

                var result = new double[rows_a, cols_a];
                for (var i = 0; i < rows_a; i++)
                    for (var j = 0; j < cols_a; j++)
                        result[i, j] = A[i, j] * B[i, j];
                return result;
            }

            public static double[,] DivideElements(double[,] A, double[,] B)
            {
                GetLength(A, out var rows_a, out var cols_a);
                GetLength(B, out var rows_b, out var cols_b);

                if (rows_a != rows_b) throw new InvalidOperationException("Число строк матриц не совпадает");
                if (cols_a != cols_b) throw new InvalidOperationException("Число столбцов матриц не совпадает");

                var result = new double[rows_a, cols_a];
                for (var i = 0; i < rows_a; i++)
                    for (var j = 0; j < cols_a; j++)
                        result[i, j] = A[i, j] / B[i, j];
                return result;
            }

            /// <summary>Оператор вычисления разности двумерного массива элементов матрицы с числом</summary>
            /// <param name="matrix">Массив элементов матрицы</param>
            /// <param name="x">Число</param>
            /// <returns>Массив разности элементов матрицы с числом</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="matrix"/> не определена</exception>
            public static double[,] Subtract(double[,] matrix, double x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = matrix[i, j] - x;
                return result;
            }

            /// <summary>Оператор вычисления разности двумерного массива элементов матрицы с числом</summary>
            /// <param name="matrix">Массив элементов матрицы</param>
            /// <param name="x">Число</param>
            /// <returns>Массив разности элементов матрицы с числом</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="matrix"/> не определена</exception>
            public static double[,] Subtract(double x, double[,] matrix)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = matrix[i, j] - x;
                return result;
            }

            /// <summary>Оператор вычисления произведения двумерного массива элементов матрицы с числом</summary>
            /// <param name="matrix">Массив элементов матрицы</param>
            /// <param name="x">Число</param>
            /// <returns>Массив произведения элементов матрицы с числом</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="matrix"/> не определена</exception>
            public static double[,] Multiply(double[,] matrix, double x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = matrix[i, j] * x;
                return result;
            }

            /// <summary>Оператор вычисления произведения двумерного массива элементов матрицы с числом</summary>
            /// <param name="matrix">Массив элементов матрицы</param>
            /// <param name="x">Число</param>
            /// <returns>Массив произведения элементов матрицы с числом</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="matrix"/> не определена</exception>
            public static double[,] Divide(double[,] matrix, double x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = matrix[i, j] / x;
                return result;
            }

            /// <summary>Оператор вычисления частного двумерного массива элементов матрицы с числом</summary>
            /// <param name="matrix">Массив элементов матрицы</param>
            /// <param name="x">Число</param>
            /// <returns>Массив частного элементов матрицы с числом</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="matrix"/> не определена</exception>
            public static double[,] Divide(double x, double[,] matrix)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = x / matrix[i, j];
                return result;
            }

            /// <summary>Оператор вычисления суммы элементов двух матриц</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="B">Массив элементов второй матрицы</param>
            /// <returns>Массив суммы элементов двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матрицы не равны</exception>
            public static double[,] Add(double[,] A, double[,] B)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (B is null) throw new ArgumentNullException(nameof(B));

                GetLength(A, out var N, out var M);
                if (N != B.GetLength(0) || M != B.GetLength(1))
                    throw new ArgumentException(@"Размеры матриц не равны.", nameof(B));
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = A[i, j] + B[i, j];
                return result;
            }

            /// <summary>Оператор вычисления разности элементов двух матриц</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="B">Массив элементов второй матрицы</param>
            /// <returns>Массив разности элементов двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матрицы не равны</exception>
            public static double[,] Subtract(double[,] A, double[,] B)
            {
                GetLength(A, out var N, out var M);
                if (B is null) throw new ArgumentNullException(nameof(B));

                if (N != B.GetLength(0) || M != B.GetLength(1))
                    throw new ArgumentException(@"Размеры матриц не равны.", nameof(B));
                var result = new double[N, M];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i, j] = A[i, j] - B[i, j];
                return result;
            }

            /// <summary>Оператор вычисления произведения элементов матрицы на столбец</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="col">Массив элементов столбца</param>
            /// <returns>Массив произведения элементов матрицы и столбца</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="col"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матрицы и столбца не равны</exception>
            public static double[] MultiplyCol(double[,] A, double[] col)
            {
                if (A is null)
                    throw new ArgumentNullException(nameof(A));
                if (col is null)
                    throw new ArgumentNullException(nameof(col));

                GetLength(A, out var N, out var M);
                if (M != col.Length)
                    throw new ArgumentException(@"Число столбцов матрицы А не равно числу элементов массива col", nameof(col));
                var result = new double[N];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        result[i] += A[i, j] * col[j];
                return result;
            }

            /// <summary>Оператор вычисления произведения элементов строки и матрицы</summary>
            /// <param name="B">Массив элементов первой матрицы</param>
            /// <param name="row">Массив элементов строки</param>
            /// <returns>Массив произведения элементов матрицы и столбца</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="row"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матрицы и строки не равны</exception>
            public static double[] MultiplyRow(double[] row, double[,] B)
            {
                if (B is null)
                    throw new ArgumentNullException(nameof(B));
                if (row is null)
                    throw new ArgumentNullException(nameof(row));

                GetLength(B, out var N, out var M);
                if (B.Length != N)
                    throw new ArgumentException(@"Число столбцов матрицы B не равно числу элементов массива row", nameof(row));
                var result = new double[M];
                for (var j = 0; j < M; j++)
                    for (var i = 0; i < N; i++)
                        result[j] += row[i] * B[i, j];
                return result;
            }

            /// <summary>Оператор вычисления произведения элементов строки и элементов столбца</summary>
            /// <param name="col">Массив элементов столбца</param>
            /// <param name="row">Массив элементов строки</param>
            /// <returns>Массив произведения элементов строки и столбца</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="col"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="row"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности строки и столбца не равны</exception>
            public static double MultiplyRowToCol(double[] row, double[] col)
            {
                if (row is null)
                    throw new ArgumentNullException(nameof(row));
                if (col is null)
                    throw new ArgumentNullException(nameof(col));
                if (col.Length != row.Length)
                    throw new ArgumentException(@"Число столбцов элементов строки не равно числу элементов столбца", nameof(row));

                var result = default(double);
                for (var i = 0; i < row.Length; i++)
                    result += row[i] * col[i];
                return result;
            }

            /// <summary>Оператор вычисления произведения двух матриц</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="B">Массив элементов второй матрицы</param>
            /// <returns>Массив произведения двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            public static double[,] Multiply(double[,] A, double[,] B)
            {
                if (A is null)
                    throw new ArgumentNullException(nameof(A));
                if (B is null)
                    throw new ArgumentNullException(nameof(B));
                if (A.GetLength(1) != B.GetLength(0))
                    throw new ArgumentOutOfRangeException(nameof(B), @"Число столбцов матрицы А не равно числу строк матрицы B");

                GetLength(A, out var A_N, out var A_M);
                GetColsCount(B, out var B_M);

                var result = new double[A_N, B_M];
                for (var i = 0; i < A_N; i++)
                    for (var j = 0; j < B_M; j++)
                    {
                        var s = default(double);
                        for (var k = 0; k < A_M; k++)
                            s += A[i, k] * B[k, j];
                        result[i, j] = s;
                    }

                return result;
            }

            public static void Multiply(double[,] A, double[] X, double[] Y)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (Y is null) throw new ArgumentNullException(nameof(Y));
                GetLength(A, out var rows_count, out var cols_count);
                if (rows_count != Y.Length) throw new ArgumentException($"Число строк матрицы ({rows_count}) не равно длине вектора результата Y ({Y.Length})");
                if (cols_count != X.Length) throw new ArgumentException($"Число столбцов матрицы ({cols_count}) не равно длине вектора X ({X.Length})");

                for (var i = 0; i < rows_count; i++)
                {
                    var sum = 0d;
                    for (var j = 0; j < cols_count; j++)
                        sum += A[i, j] * X[j];
                    Y[i] = sum;
                }
            }

            public static double[] Multiply(double[,] A, double[] X)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (A.GetLength(1) != X.Length) throw new ArgumentException($"Число столбцов матрицы ({A.GetLength(1)}) не равно длине вектора X ({X.Length})");
                var result = new double[A.GetLength(0)];
                Multiply(A, X, result);
                return result;
            }

            /// <summary>Оператор вычисления произведения двух матриц</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="B">Массив элементов второй матрицы</param>
            /// <param name="result">Массив элементов произведения</param>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="result"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матриц не согласованы</exception>
            /// <exception cref="ArgumentException">В случае если число строк <paramref name="result"/> не равно числу строк <paramref name="A"/></exception>
            /// <exception cref="ArgumentException">В случае если число столбцов <paramref name="result"/> не равно числу строк <paramref name="B"/></exception>
            public static void Multiply(double[,] A, double[,] B, double[,] result)
            {
                if (A is null)
                    throw new ArgumentNullException(nameof(A));
                if (B is null)
                    throw new ArgumentNullException(nameof(B));
                if (result is null)
                    throw new ArgumentNullException(nameof(result));
                if (A.GetLength(1) != B.GetLength(0))
                    throw new ArgumentOutOfRangeException(nameof(B), @"Матрицы несогласованных порядков.");
                if (result.GetLength(0) != A.GetLength(0))
                    throw new ArgumentException(@"Число строк матрицы результата не равно числу строк первой матрицы", nameof(result));
                if (result.GetLength(1) != B.GetLength(1))
                    throw new ArgumentException(@"Число столбцов матрицы результата не равно числу строк второй матрицы", nameof(result));

                GetLength(A, out var A_N, out var A_M);
                GetColsCount(B, out var B_M);
                for (var i = 0; i < A_N; i++)
                    for (var j = 0; j < B_M; j++)
                    {
                        result[i, j] = default;
                        for (var k = 0; k < A_M; k++)
                            result[i, j] += A[i, k] * B[k, j];
                    }
            }

            /// <summary>Оператор деления двух матриц</summary>
            /// <param name="A">Делимое</param>
            /// <param name="B">Делитель</param>
            /// <returns>Частное двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матриц не согласованы</exception>
            public static double[,] Divide(double[,] A, double[,] B) => Multiply(A, Inverse(B, out _));

            /// <summary>Объединение матриц по строкам, либо столбцам</summary>
            /// <returns>Двумерный массив, содержащий объединение элементов исходных массивов по строкам, либо столбцам</returns>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> or <paramref name="B"/> is <see langword="null"/></exception>
            public static double[,] Concatenate(double[,] A, double[,] B)
            {
                GetLength(A, out var A_N, out var A_M);
                GetLength(B, out var B_N, out var B_M);

                double[,] result;
                if (A_M == B_M) // Конкатенация по строкам
                {
                    result = new double[A_N + B_N, A_M];
                    for (var i = 0; i < A_N; i++)
                        for (var j = 0; j < A_M; j++)
                            result[i, j] = A[i, j];
                    var i0 = A_N;
                    for (var i = 0; i < B_N; i++)
                        for (var j = 0; j < B_M; j++)
                            result[i + i0, j] = B[i, j];
                }
                else if (A_N == B_N) //Конкатенация по строкам
                {
                    result = new double[A_N, A_M + B_M];
                    for (var i = 0; i < A_N; i++)
                        for (var j = 0; j < A_M; j++)
                            result[i, j] = A[i, j];
                    var j0 = A_M;
                    for (var i = 0; i < B_N; i++)
                        for (var j = 0; j < B_M; j++)
                            result[i, j + j0] = B[i, j];
                }
                else
                    throw new InvalidOperationException(@"Конкатенация возможна только по строкам, или по столбцам");

                return result;
            }

            /// <summary>Оператор вычисления билинейной формы с векторными операндами b = <paramref name="x"/>*<paramref name="a"/>*<paramref name="y"/></summary>
            /// <param name="x">Массив компонент левой строки билинейной формы</param>
            /// <param name="a">Матрица билинейной формы</param>
            /// <param name="y">Массив компонент правого столбца билинейной формы</param>
            /// <returns>Результат вычисления билинейной формы</returns>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="x"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="y"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Если длина строки <paramref name="x"/> не равна числу строк матрицы <paramref name="a"/></exception>
            /// <exception cref="ArgumentException">Если длина столбца <paramref name="y"/> не равна числу столбцов матрицы <paramref name="a"/></exception>
            public static double BiliniarMultiply(double[] x, double[,] a, double[] y)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (y is null) throw new ArgumentNullException(nameof(y));
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (x.Length != a.GetLength(0))
                    throw new ArgumentException($@"Длина вектора {nameof(x)} не равна числу строк матрицы {nameof(a)}", nameof(x));
                if (y.Length != a.GetLength(1))
                    throw new ArgumentException($@"Длина вектора {nameof(y)} не равна числу столбцов матрицы {nameof(a)}", nameof(y));

                var result = default(double);

                GetLength(a, out var N, out var M);

                if (N == 0 || M == 0)
                    return double.NaN;

                for (var i = 0; i < N; i++)
                {
                    var s = default(double);
                    for (var j = 0; j < M; j++)
                        s += a[i, j] * y[j];
                    result += x[i] * s;
                }

                return result;
            }

            /// <summary>Оператор вычисления билинейной формы с векторными операндами b = <paramref name="x"/>*<paramref name="a"/>*<paramref name="y"/></summary>
            /// <param name="x">Двумерный массив компонент матрицы первого операнда билинейной формы</param>
            /// <param name="a">Двумерный массив компонент матрицы оператора билинейной нормы</param>
            /// <param name="y">Двумерный массив компонент матрицы второго операнда билинейной формы</param>
            /// <returns>
            /// Двумерный массив компонент матрицы результата вычисления билинейной формы, 
            /// число строк которого равно числу строк операнда <paramref name="x"/>, число столбцов - равно числу столбцов операнда <paramref name="y"/>
            /// </returns>
            /// <exception cref="ArgumentNullException"><paramref name="x"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="y"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Число столбцов <paramref name="x"/> не равно числу строк <paramref name="a"/></exception>
            /// <exception cref="ArgumentException">Число строк <paramref name="y"/> не равно числу столбцов <paramref name="a"/></exception>
            public static double[,] BiliniarMultiply(double[,] x, double[,] a, double[,] y)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (y is null) throw new ArgumentNullException(nameof(y));
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (x.GetLength(1) != a.GetLength(0))
                    throw new ArgumentException($@"Число столбцов матрицы {nameof(x)} не равно числу строк матрицы {nameof(a)}", nameof(x));
                if (y.GetLength(0) != a.GetLength(1))
                    throw new ArgumentException($@"Число строк матрицы {nameof(y)} не равно числу столбцов матрицы {nameof(a)}", nameof(y));

                GetRowsCount(x, out var x_N);
                GetLength(a, out var N, out var M);
                GetColsCount(y, out var y_M);

                var result = new double[x_N, y_M];

                if (x_N == 0 || y_M == 0)
                    return result;

                for (var i0 = 0; i0 < x_N; i0++)
                    for (var j0 = 0; j0 < y_M; j0++)
                    {
                        var s0 = default(double);
                        for (var i = 0; i < N; i++)
                        {
                            var s = default(double);
                            for (var j = 0; j < M; j++)
                                s += a[i, j] * y[j, j0];
                            s0 += x[i0, i] * s;
                        }
                        result[i0, j0] = s0;
                    }

                return result;
            }

            /// <summary>Вычисление оператора билинейной формы для одного операнда B = X*A*X^T</summary>
            /// <param name="x">Элементы массива вектора операнда оператора билинейной формы</param>
            /// <param name="a">Элементы двумерного массива матрицы оператора билинейной формы (должна быть квадратной с числом строк, равным числу элементов вектора операнда <paramref name="x"/>)</param>
            /// <returns>Численное значение результата вычисления билинейной формы</returns>
            /// <exception cref="ArgumentNullException"><paramref name="x"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Если массив элементов матрицы <paramref name="a"/> не квадратный</exception>
            /// <exception cref="ArgumentException">Если число элементов вектора <paramref name="x"/> не равно числу строк массива элементов матрицы <paramref name="a"/></exception>
            public static double BiliniarMultiplyAuto(double[] x, double[,] a)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (a.GetLength(0) != a.GetLength(1))
                    throw new ArgumentException($@"Матрица {nameof(a)} билинейной формы X*A*X^T не квадратная", nameof(a));
                if (x.Length != a.GetLength(0))
                    throw new ArgumentException($@"Длина вектора {nameof(x)} не равна числу строк матрицы {nameof(a)}", nameof(x));

                if (x.Length == 0)
                    return double.NaN;

                var result = default(double);

                GetLength(a, out var N, out var M);

                for (var i = 0; i < N; i++)
                {
                    var s = default(double);
                    for (var j = 0; j < M; j++)
                        s += a[i, j] * x[j];
                    result += x[i] * s;
                }

                return result;
            }

            public static double[,] BiliniarMultiplyAuto(double[,] x, double[,] a)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (a.GetLength(0) != a.GetLength(1))
                    throw new ArgumentException($@"Матрица {nameof(a)} билинейной формы X*A*X^T не квадратная", nameof(a));
                if (x.GetLength(1) != a.GetLength(0))
                    throw new ArgumentException($@"Число столбцов матрицы аргумента {nameof(x)} не равно числу столбцов (строк) матрицы билинейной формы {nameof(a)}", nameof(x));

                GetRowsCount(x, out var x_N);
                GetLength(a, out var N, out var M);

                var result = new double[x_N, x_N];

                if (x_N == 0)
                    return result;

                for (var i0 = 0; i0 < x_N; i0++)
                    for (var j0 = 0; j0 < x_N; j0++)
                    {
                        var s0 = default(double);
                        for (var i = 0; i < N; i++)
                        {
                            var s = default(double);
                            for (var j = 0; j < M; j++)
                                s += a[i, j] * x[j0, j];
                            s0 += x[i0, i] * s;
                        }
                        result[i0, j0] = s0;
                    }

                return result;
            }

            private static void ExecuteAXAt(double[,] A, double[,] X, double[,] Y, int N, int M)
            {
                for (var n = 0; n < N; n++)
                    for (var m = 0; m < N; m++)
                    {
                        Y[n, m] = 0;
                        for (var i = 0; i < M; i++)
                        {
                            var s = 0d;
                            for (var j = 0; j < M; j++)
                            {
                                var x = X[i, j];
                                var a = A[m, j];
                                s += x * a;
                            }

                            Y[n, m] += s * A[n, i];
                        }
                    }
            }

            /// <summary>Оператор вида Y = A * X * A^T</summary>
            public static void AXAt(double[,] A, double[,] X, double[,] Y)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (Y is null) throw new ArgumentNullException(nameof(Y));

                var rows_x = GetSquareLength(X);
                var rows_y = GetSquareLength(Y);

                GetLength(A, out var N, out var M);

                if (M != X.GetLength(0)) throw new ArgumentException("Число столбцов матриц не совпадает", nameof(X));
                if (M != rows_x) throw new ArgumentException("Число столбцов матриц не совпадает", nameof(X));
                if (N != rows_y) throw new ArgumentException("Размерность матрицы результата не равна числу строк матрицы A", nameof(Y));

                ExecuteAXAt(A, X, Y, N, M);
            }

            /// <summary>Оператор вида Y = A * X * A^T</summary>
            public static double[,] AXAt(double[,] A, double[,] X)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));

                var rows_x = GetSquareLength(X);

                GetLength(A, out var N, out var M);

                if (M != X.GetLength(0)) throw new ArgumentException("Число столбцов матриц не совпадает", nameof(X));
                if (M != rows_x) throw new ArgumentException("Число столбцов матриц не совпадает", nameof(X));

                var Y = new double[N, N];

                ExecuteAXAt(A, X, Y, N, M);

                return Y;
            }

            public static double XtAY(double[] X, double[,] A, double[] Y)
            {
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (Y is null) throw new ArgumentNullException(nameof(Y));

                GetLength(A, out var N, out var M);
                if (X.Length != N) throw new InvalidOperationException($"Число строк ({N}) матрицы A[{N},{M}] не равно длине вектора X.Length = {X.Length}");
                if (Y.Length != M) throw new InvalidOperationException($"Число столбцов ({M}) матрицы A[{N},{M}] не равно длине вектора Y.Length = {Y.Length}");

                var result = 0.0;
                for (var i = 0; i < N; i++)
                {
                    var s = 0.0;
                    for (var j = 0; j < M; j++)
                        s += A[i, j] * Y[j];

                    result += s * X[i];
                }

                return result;
            }
        }
    }

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="w"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix U, out double[] w, out Matrix V)
    {
        Array.SVD(_Data, out var u, out w, out var v);
        U = new Matrix(u);
        V = new Matrix(v);
    }

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="S"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix U, out Matrix S, out Matrix V)
    {
        SVD(out U, out double[] w, out V);
        S = CreateDiagonal(w);
    }
}