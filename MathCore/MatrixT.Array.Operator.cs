#if NET5_0_OR_GREATER

// ReSharper disable InconsistentNaming
namespace MathCore;

public partial class Matrix<T>
{
    public static partial class Array
    {
        public static partial class Operator
        {
            /// <summary>Скалярное произведение векторов</summary>
            /// <param name="v1">Первый множитель скалярного произведения</param>
            /// <param name="v2">Второй множитель скалярного произведения</param>
            /// <returns>Скалярное произведение векторов</returns>
            public static T Multiply(T[] v1, T[] v2)
            {
                if (v1 is null) throw new ArgumentNullException(nameof(v1));
                if (v2 is null) throw new ArgumentNullException(nameof(v2));
                if (v1.Length != v2.Length) throw new ArgumentException(@"Длины векторов не совпадают", nameof(v2));

                var s = default(T);
                var N = v1.Length;
                for (var i = 0; i < N; i++)
                    s += v1[i] * v2[i];
                return s;
            }

            /// <summary>Длина вектора</summary>
            /// <param name="v">Вектор элементов</param>
            /// <returns>Длина вектора</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v"/> is <see langword="null"/></exception>
            public static T VectorLength(T[] v)
            {
                if (v is null) throw new ArgumentNullException(nameof(v));

                var s = default(T);
                for (var i = 0; i < v.Length; i++)
                    s += v[i] * v[i];
                return T.Sqrt(s);
            }

            /// <summary>Умножение вектора на число</summary>
            /// <param name="v1">Первый сомножитель - вектор элементов</param>
            /// <param name="v2">Второй сомножитель - число, на которое должны быть умножены все элементы вектора</param>
            /// <returns>Вектор произведений элементов входного вектора и числа</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v1"/> is <see langword="null"/></exception>
            public static T[] Multiply(T[] v1, T v2)
            {
                if (v1 is null) throw new ArgumentNullException(nameof(v1));

                var s = new T[v1.Length];
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
            public static T[] Divide(T[] v1, T v2)
            {
                if (v1 is null)
                    throw new ArgumentNullException(nameof(v1));

                var s = new T[v1.Length];
                var N = s.Length;
                for (var i = 0; i < N; i++)
                    s[i] = v1[i] / v2;
                return s;
            }

            /// <summary>Проекция вектора на вектор</summary>
            /// <returns>Вектор - произведение компонентов исходных векторов</returns>
            /// <exception cref="ArgumentNullException"><paramref name="v1"/> or <paramref name="v2"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Длины векторов не совпадают</exception>
            public static T[] Projection(T[] v1, T[] v2)
            {
                if (v1 is null)
                    throw new ArgumentNullException(nameof(v1));
                if (v2 is null)
                    throw new ArgumentNullException(nameof(v2));
                if (v1.Length != v2.Length)
                    throw new ArgumentException(@"Длины векторов не совпадают", nameof(v2));

                var result = new T[v2.Length];
                var m = default(T);
                var v2_length2 = default(T);
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
            public static T[,] Add(T[,] matrix, T x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new T[N, M];
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
            public static T[] Add(T[] a, T[] b)
            {
                if (a is null)
                    throw new ArgumentNullException(nameof(a));
                if (b is null)
                    throw new ArgumentNullException(nameof(b));

                var result = new T[a.Length];
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
            public static T[] Subtract(T[] a, T[] b)
            {
                if (a is null)
                    throw new ArgumentNullException(nameof(a));
                if (b is null)
                    throw new ArgumentNullException(nameof(b));
                if (a.Length != b.Length)
                    throw new InvalidOperationException(@"Размеры векторов не совпадают");

                var result = new T[a.Length];
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
            public static T[] MultiplyElements(T[] a, T[] b)
            {
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (b is null) throw new ArgumentNullException(nameof(b));
                if (a.Length != b.Length) throw new InvalidOperationException(@"Размеры векторов не совпадают");

                var result = new T[a.Length];
                for (var i = 0; i < a.Length; i++)
                    result[i] = a[i] * b[i];
                return result;
            }

            /// <summary>Оператор вычисления поэлементного деления двух векторов</summary>
            /// <param name="a">Вектор - делимое</param>
            /// <param name="b">Вектор - делитель</param>
            /// <returns>Вектор, составленный из поэлементного частного элементов векторов делимого и делителя</returns>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> or <paramref name="b"/> is <see langword="null"/></exception>
            public static T[] DivideElements(T[] a, T[] b)
            {
                if (a is null)
                    throw new ArgumentNullException(nameof(a));
                if (b is null)
                    throw new ArgumentNullException(nameof(b));

                var result = new T[a.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = a[i] / b[i];
                return result;
            }

            public static T[,] MultiplyElements(T[,] A, T[,] B)
            {
                GetLength(A, out var rows_a, out var cols_a);
                GetLength(B, out var rows_b, out var cols_b);

                if (rows_a != rows_b) throw new InvalidOperationException("Число строк матриц не совпадает");
                if (cols_a != cols_b) throw new InvalidOperationException("Число столбцов матриц не совпадает");

                var result = new T[rows_a, cols_a];
                for (var i = 0; i < rows_a; i++)
                    for (var j = 0; j < cols_a; j++)
                        result[i, j] = A[i, j] * B[i, j];
                return result;
            }

            public static T[,] DivideElements(T[,] A, T[,] B)
            {
                GetLength(A, out var rows_a, out var cols_a);
                GetLength(B, out var rows_b, out var cols_b);

                if (rows_a != rows_b) throw new InvalidOperationException("Число строк матриц не совпадает");
                if (cols_a != cols_b) throw new InvalidOperationException("Число столбцов матриц не совпадает");

                var result = new T[rows_a, cols_a];
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
            public static T[,] Subtract(T[,] matrix, T x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new T[N, M];
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
            public static T[,] Subtract(T x, T[,] matrix)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new T[N, M];
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
            public static T[,] Multiply(T[,] matrix, T x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new T[N, M];
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
            public static T[,] Divide(T[,] matrix, T x)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new T[N, M];
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
            public static T[,] Divide(T x, T[,] matrix)
            {
                if (matrix is null)
                    throw new ArgumentNullException(nameof(matrix));

                GetLength(matrix, out var N, out var M);
                var result = new T[N, M];
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
            public static T[,] Add(T[,] A, T[,] B)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (B is null) throw new ArgumentNullException(nameof(B));

                GetLength(A, out var N, out var M);
                if (N != B.GetLength(0) || M != B.GetLength(1))
                    throw new ArgumentException(@"Размеры матриц не равны.", nameof(B));
                var result = new T[N, M];
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
            public static T[,] Subtract(T[,] A, T[,] B)
            {
                GetLength(A, out var N, out var M);
                if (B is null) throw new ArgumentNullException(nameof(B));

                if (N != B.GetLength(0) || M != B.GetLength(1))
                    throw new ArgumentException(@"Размеры матриц не равны.", nameof(B));
                var result = new T[N, M];
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
            public static T[] MultiplyCol(T[,] A, T[] col)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (col is null) throw new ArgumentNullException(nameof(col));

                GetLength(A, out var N, out var M);
                if (M != col.Length)
                    throw new ArgumentException(@"Число столбцов матрицы А не равно числу элементов массива col", nameof(col));
                var result = new T[N];
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
            public static T[] MultiplyRow(T[] row, T[,] B)
            {
                if (B is null) throw new ArgumentNullException(nameof(B));
                if (row is null) throw new ArgumentNullException(nameof(row));

                GetLength(B, out var N, out var M);
                if (B.Length != N)
                    throw new ArgumentException(@"Число столбцов матрицы B не равно числу элементов массива row", nameof(row));
                var result = new T[M];
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
            public static T MultiplyRowToCol(T[] row, T[] col)
            {
                if (row is null) throw new ArgumentNullException(nameof(row));
                if (col is null) throw new ArgumentNullException(nameof(col));
                if (col.Length != row.Length) throw new ArgumentException(@"Число столбцов элементов строки не равно числу элементов столбца", nameof(row));

                var result = default(T);
                for (var i = 0; i < row.Length; i++)
                    result += row[i] * col[i];
                return result;
            }

            public static T[,] MultiplyRowToColMatrix(T[] row, T[] col) => MultiplyRowToColMatrix(row, col, new T[col.Length, row.Length]);

            public static T[,] MultiplyRowToColMatrix(T[] row, T[] col, T[,] matrix)
            {
                if (row is null) throw new ArgumentNullException(nameof(row));
                if (col is null) throw new ArgumentNullException(nameof(col));
                var N = col.Length;
                var M = row.Length;
                GetLength(matrix, out var matrix_N, out var matrix_M);
                if (matrix_N != N) throw new InvalidOperationException("Число строк матрицы не равно длине вектора col");
                if (matrix_M != M) throw new InvalidOperationException("Число столбцов матрицы не равно длине вектора row");

                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        matrix[i, j] = col[i] * row[j];

                return matrix;
            }

            /// <summary>Оператор вычисления произведения двух матриц</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="B">Массив элементов второй матрицы</param>
            /// <returns>Массив произведения двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            public static T[,] Multiply(T[,] A, T[,] B)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (B is null) throw new ArgumentNullException(nameof(B));
                if (A.GetLength(1) != B.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(B), @"Число столбцов матрицы А не равно числу строк матрицы B");

                GetLength(A, out var A_N, out var A_M);
                GetColsCount(B, out var B_M);

                var result = new T[A_N, B_M];
                for (var i = 0; i < A_N; i++)
                    for (var j = 0; j < B_M; j++)
                    {
                        var s = default(T);
                        for (var k = 0; k < A_M; k++)
                            s += A[i, k] * B[k, j];
                        result[i, j] = s;
                    }

                return result;
            }

            public static T[] MultiplyAtb(T[,] At, T[] x) => MultiplyAtb(At, x, new T[At.GetLength(1)]);

            public static T[] MultiplyAtb(T[,] At, T[] x, T[] y)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (y is null) throw new ArgumentNullException(nameof(y));
                GetLength(At, out var A_M, out var A_N);

                if (A_M != x.Length) throw new InvalidOperationException($"Число строк ({A_M}) матрицы A[{A_M}, {A_N}] не равно размеру вектора y.Length={y.Length}");
                if (y.Length != A_N) throw new InvalidOperationException($"Длина вектора y.Length={y.Length} не равна числу столбцов ({A_N}) матрицы A[{A_M}, {A_N}]");

                for (var j = 0; j < A_N; j++)
                {
                    var s = T.Zero;
                    for (var i = 0; i < A_M; i++)
                        s += At[i, j] * x[i];
                    y[j] = s;
                }

                return y;
            }

            /// <summary>Оператор вычисления произведения двух матриц (первая - транспонированная)</summary>
            /// <param name="At">Массив элементов первой матрицы (транспонированной)</param>
            /// <param name="B">Массив элементов второй матрицы</param>
            /// <returns>Массив произведения двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="At"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            public static T[,] MultiplyAtB(T[,] At, T[,] B) => MultiplyAtB(At, B, new T[At.GetLength(1), B.GetLength(1)]);

            public static T[,] MultiplyAtB(T[,] At, T[,] B, T[,] C)
            {
                GetLength(At, out var A_M, out var A_N);
                GetLength(B, out var B_N, out var B_M);
                GetLength(C, out var C_N, out var C_M);

                if (A_M != B_N) throw new InvalidOperationException($"Число строк ({A_M} матрицы AT[{A_M}, {A_N}] не равно числу строк ({B_N}) матрицы B[{B_N}, {B_M}])");
                if (C_N != A_N || C_M != B_M) throw new InvalidOperationException($"Размер матрицы результата C[{C_N}, {C_M}] не соответствует размерам матриц A и B");

                for (var i = 0; i < A_N; i++)
                    for (var j = 0; j < B_M; j++)
                    {
                        var s = default(T);
                        for (var k = 0; k < A_M; k++)
                            s += At[k, i] * B[k, j];

                        C[i, j] = s;
                    }

                return C;
            }

            /// <summary>Оператор вычисления произведения двух матриц</summary>
            /// <param name="A">Массив элементов первой матрицы</param>
            /// <param name="Bt">Массив элементов второй матрицы</param>
            /// <returns>Массив произведения двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="Bt"/> не определена</exception>
            public static T[,] MultiplyABt(T[,] A, T[,] Bt)
            {
                GetLength(A, out var A_N, out var A_M);
                GetLength(Bt, out var B_M, out var B_N);

                if (A_M != B_N) throw new InvalidOperationException($"Число строк ({A_M} матрицы AT[{A_M}, {A_N}] не равно числу строк ({B_N}) матрицы B[{B_N}, {B_M}])");

                var result = new T[A_N, B_M];
                for (var i = 0; i < A_N; i++)
                    for (var j = 0; j < B_M; j++)
                    {
                        var s = default(T);
                        for (var k = 0; k < A_M; k++)
                            s += A[i, k] * Bt[j, k];
                        result[i, j] = s;
                    }

                return result;
            }

            public static T[] Multiply(T[,] A, T[] X, T[] Y)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (Y is null) throw new ArgumentNullException(nameof(Y));
                GetLength(A, out var rows_count, out var cols_count);
                if (rows_count != Y.Length) throw new ArgumentException($"Число строк матрицы ({rows_count}) не равно длине вектора результата Y ({Y.Length})");
                if (cols_count != X.Length) throw new ArgumentException($"Число столбцов матрицы ({cols_count}) не равно длине вектора X ({X.Length})");

                for (var i = 0; i < rows_count; i++)
                {
                    var sum = T.Zero;
                    for (var j = 0; j < cols_count; j++)
                        sum += A[i, j] * X[j];
                    Y[i] = sum;
                }

                return Y;
            }

            /// <summary>Умножение матрицы на вектор</summary>
            /// <param name="A">Матрица</param>
            /// <param name="X">Вектор</param>
            /// <returns>Вектор - результат умножения</returns>
            /// <exception cref="ArgumentNullException">Если <paramref name="A"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException">Если <paramref name="X"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Если число столбцов матрицы не равно длине вектора X</exception>
            public static T[] Multiply(T[,] A, T[] X)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (A.GetLength(1) != X.Length) throw new ArgumentException($"Число столбцов матрицы ({A.GetLength(1)}) не равно длине вектора X ({X.Length})");
                return Multiply(A, X, new T[A.GetLength(0)]);
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
            public static T[,] Multiply(T[,] A, T[,] B, T[,] result)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (B is null) throw new ArgumentNullException(nameof(B));
                if (result is null) throw new ArgumentNullException(nameof(result));
                if (A.GetLength(1) != B.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(B), @"Матрицы несогласованных порядков.");
                if (result.GetLength(0) != A.GetLength(0)) throw new ArgumentException(@"Число строк матрицы результата не равно числу строк первой матрицы", nameof(result));
                if (result.GetLength(1) != B.GetLength(1)) throw new ArgumentException(@"Число столбцов матрицы результата не равно числу строк второй матрицы", nameof(result));

                GetLength(A, out var A_N, out var A_M);
                GetColsCount(B, out var B_M);
                for (var i = 0; i < A_N; i++)
                    for (var j = 0; j < B_M; j++)
                    {
                        result[i, j] = default;
                        for (var k = 0; k < A_M; k++)
                            result[i, j] += A[i, k] * B[k, j];
                    }

                return result;
            }

            /// <summary>Оператор деления двух матриц</summary>
            /// <param name="A">Делимое</param>
            /// <param name="B">Делитель</param>
            /// <returns>Частное двух матриц</returns>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="A"/> не определена</exception>
            /// <exception cref="ArgumentNullException">В случае если <paramref name="B"/> не определена</exception>
            /// <exception cref="ArgumentException">В случае если размерности матриц не согласованы</exception>
            public static T[,] Divide(T[,] A, T[,] B) => Multiply(A, Inverse(B, out _));

            /// <summary>Объединение матриц по строкам, либо столбцам</summary>
            /// <returns>Двумерный массив, содержащий объединение элементов исходных массивов по строкам, либо столбцам</returns>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> or <paramref name="B"/> is <see langword="null"/></exception>
            public static T[,] Concatenate(T[,] A, T[,] B)
            {
                GetLength(A, out var A_N, out var A_M);
                GetLength(B, out var B_N, out var B_M);

                T[,] result;
                if (A_M == B_M) // Конкатенация по строкам
                {
                    result = new T[A_N + B_N, A_M];
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
                    result = new T[A_N, A_M + B_M];
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

            /// <summary>Оператор вычисления билинейной формы с векторными операндами b = <paramref name="x"/>*<paramref name="A"/>*<paramref name="y"/></summary>
            /// <param name="x">Массив компонент левой строки билинейной формы</param>
            /// <param name="A">Матрица билинейной формы</param>
            /// <param name="y">Массив компонент правого столбца билинейной формы</param>
            /// <returns>Результат вычисления билинейной формы</returns>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="x"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="y"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Если длина строки <paramref name="x"/> не равна числу строк матрицы <paramref name="A"/></exception>
            /// <exception cref="ArgumentException">Если длина столбца <paramref name="y"/> не равна числу столбцов матрицы <paramref name="A"/></exception>
            public static T BiliniarMultiply(T[] x, T[,] A, T[] y)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (y is null) throw new ArgumentNullException(nameof(y));
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (x.Length != A.GetLength(0)) throw new ArgumentException($@"Длина вектора {nameof(x)} не равна числу строк матрицы {nameof(A)}", nameof(x));
                if (y.Length != A.GetLength(1)) throw new ArgumentException($@"Длина вектора {nameof(y)} не равна числу столбцов матрицы {nameof(A)}", nameof(y));

                var result = default(T);

                GetLength(A, out var N, out var M);

                if (N == 0 || M == 0) return T.NaN;

                for (var i = 0; i < N; i++)
                {
                    var s = default(T);
                    for (var j = 0; j < M; j++)
                        s += A[i, j] * y[j];
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
            public static T[,] BiliniarMultiply(T[,] x, T[,] a, T[,] y)
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

                var result = new T[x_N, y_M];

                if (x_N == 0 || y_M == 0)
                    return result;

                for (var i0 = 0; i0 < x_N; i0++)
                    for (var j0 = 0; j0 < y_M; j0++)
                    {
                        var s0 = default(T);
                        for (var i = 0; i < N; i++)
                        {
                            var s = default(T);
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
            public static T BiliniarMultiplyAuto(T[] x, T[,] a)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (a.GetLength(0) != a.GetLength(1))
                    throw new ArgumentException($@"Матрица {nameof(a)} билинейной формы X*A*X^T не квадратная", nameof(a));
                if (x.Length != a.GetLength(0))
                    throw new ArgumentException($@"Длина вектора {nameof(x)} не равна числу строк матрицы {nameof(a)}", nameof(x));

                if (x.Length == 0)
                    return T.NaN;

                var result = default(T);

                GetLength(a, out var N, out var M);

                for (var i = 0; i < N; i++)
                {
                    var s = default(T);
                    for (var j = 0; j < M; j++)
                        s += a[i, j] * x[j];
                    result += x[i] * s;
                }

                return result;
            }

            /// <summary>Вычисление матрицы билинейной формы X*A*X^T</summary>
            /// <param name="x">Матрица элементов массива вектора операнда оператора билинейной формы</param>
            /// <param name="a">Матрица элементов двумерного массива матрицы оператора билинейной формы</param>
            /// <returns>Матрица результата вычисления билинейной формы</returns>
            /// <exception cref="ArgumentNullException"><paramref name="x"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="a"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Если матрица элементов массива <paramref name="a"/> не квадратная</exception>
            /// <exception cref="ArgumentException">Если число столбцов матрицы элементов массива <paramref name="x"/> не равно числу строк матрицы элементов массива <paramref name="a"/></exception>
            public static T[,] BiliniarMultiplyAuto(T[,] x, T[,] a)
            {
                if (x is null) throw new ArgumentNullException(nameof(x));
                if (a is null) throw new ArgumentNullException(nameof(a));
                if (a.GetLength(0) != a.GetLength(1))
                    throw new ArgumentException($@"Матрица {nameof(a)} билинейной формы X*A*X^T не квадратная", nameof(a));
                if (x.GetLength(1) != a.GetLength(0))
                    throw new ArgumentException(
                        $@"Число столбцов матрицы аргумента {nameof(x)} не равно числу столбцов (строк) матрицы билинейной формы {nameof(a)}", nameof(x));

                GetRowsCount(x, out var x_N);
                GetLength(a, out var N, out var M);

                var result = new T[x_N, x_N];

                if (x_N == 0)
                    return result;

                for (var i0 = 0; i0 < x_N; i0++)
                    for (var j0 = 0; j0 < x_N; j0++)
                    {
                        var s0 = default(T);
                        for (var i = 0; i < N; i++)
                        {
                            var s = default(T);
                            for (var j = 0; j < M; j++)
                                s += a[i, j] * x[j0, j];
                            s0 += x[i0, i] * s;
                        }

                        result[i0, j0] = s0;
                    }

                return result;
            }

            /// <summary>Выполнение умножения матриц A*X*A^T (Y = A*X*A^T)</summary>
            /// <param name="A">Матрица элементов двумерного массива матрицы оператора билинейной формы</param>
            /// <param name="X">Матрица элементов двумерного массива операнда</param>
            /// <param name="Y">Матрица результата вычисления билинейной формы</param>
            /// <param name="N">Число строк матрицы <paramref name="A"/></param>
            /// <param name="M">Число столбцов матрицы <paramref name="A"/></param>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="X"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="Y"/> is <see langword="null"/></exception>
            private static void ExecuteAXAt(T[,] A, T[,] X, T[,] Y, int N, int M)
            {
                for (var n = 0; n < N; n++)
                    for (var m = 0; m < N; m++)
                    {
                        Y[n, m] = T.Zero;
                        for (var i = 0; i < M; i++)
                        {
                            var s = T.Zero;
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

            /// <summary>Выполнение умножения матриц A*X*A^T (Y = A*X*A^T)</summary>
            /// <param name="A">Матрица элементов двумерного массива матрицы оператора билинейной формы</param>
            /// <param name="X">Матрица элементов двумерного массива операнда</param>
            /// <param name="Y">Матрица результата вычисления билинейной формы</param>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="X"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="Y"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Число столбцов матриц не совпадает</exception>
            /// <exception cref="ArgumentException">Размерность матрицы результата не равна числу строк матрицы A</exception>
            public static void AXAt(T[,] A, T[,] X, T[,] Y)
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

            /// <summary>Вычисление билинейной формы Y = AX * XT</summary>
            /// <param name="A">Матрица элементов двумерного массива оператора билинейной формы</param>
            /// <param name="X">Матрица элементов двумерного массива операнда</param>
            /// <returns>Матрица результата вычисления билинейной формы</returns>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="X"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Число столбцов матриц не совпадает</exception>
            public static T[,] AXAt(T[,] A, T[,] X)
            {
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (X is null) throw new ArgumentNullException(nameof(X));

                var rows_x = GetSquareLength(X);

                GetLength(A, out var N, out var M);

                if (M != X.GetLength(0)) throw new ArgumentException("Число столбцов матриц не совпадает", nameof(X));
                if (M != rows_x) throw new ArgumentException("Число столбцов матриц не совпадает", nameof(X));

                var Y = new T[N, N];

                ExecuteAXAt(A, X, Y, N, M);

                return Y;
            }

            /// <summary>Вычисление билинейной формы Xt * A * Y</summary>
            /// <param name="X">Вектор элементов операнда</param>
            /// <param name="A">Матрица элементов двумерного массива оператора билинейной формы</param>
            /// <param name="Y">Вектор элементов операнда</param>
            /// <returns>Скалярное значение - результат вычисления билинейной формы</returns>
            /// <exception cref="ArgumentNullException"><paramref name="X"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="A"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentNullException"><paramref name="Y"/> is <see langword="null"/></exception>
            /// <exception cref="InvalidOperationException">Число строк матрицы A не равно длине вектора X</exception>
            /// <exception cref="InvalidOperationException">Число столбцов матрицы A не равно длине вектора Y</exception>
            public static T XtAY(T[] X, T[,] A, T[] Y)
            {
                if (X is null) throw new ArgumentNullException(nameof(X));
                if (A is null) throw new ArgumentNullException(nameof(A));
                if (Y is null) throw new ArgumentNullException(nameof(Y));

                GetLength(A, out var N, out var M);
                if (X.Length != N) throw new InvalidOperationException($"Число строк ({N}) матрицы A[{N},{M}] не равно длине вектора X.Length = {X.Length}");
                if (Y.Length != M) throw new InvalidOperationException($"Число столбцов ({M}) матрицы A[{N},{M}] не равно длине вектора Y.Length = {Y.Length}");

                var result = T.Zero;
                for (var i = 0; i < N; i++)
                {
                    var s = T.Zero;
                    for (var j = 0; j < M; j++)
                        s += A[i, j] * Y[j];

                    result += s * X[i];
                }

                return result;
            }
        }
    }
}


#endif