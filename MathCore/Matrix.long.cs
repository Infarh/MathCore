using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore
{
    /// <summary>Матрица NxM</summary>
    /// <remarks>
    /// ------------ j ---------->
    /// | a11 a12 a13 a14 a15 a16 a1M
    /// | a21........................
    /// | a31........................
    /// | a41.......aij..............
    /// i a51........................
    /// | a61........................
    /// | aN1.....................aNM
    /// \/
    /// </remarks>
    [Serializable]
    public partial class MatrixLong : ICloneable, IEquatable<MatrixLong>, IIndexable<int, int, long>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="N">Размерность матрицы</param>
        /// <returns>Единичная матрица размерности NxN</returns>
        public static MatrixLong GetUnitaryMatryx(int N)
        {
            var Result = new MatrixLong(N);
            for (var i = 0; i < N; i++) Result[i, i] = 1;
            return Result;
        }

        /// <summary>Трансвекция матрицы</summary>
        /// <param name="A">Трансвецируемая матрица</param>
        /// <param name="j">Оборный столбец</param>
        /// <returns>Трансвекция матрицы А</returns>                    
        public static MatrixLong GetTransvection(MatrixLong A, int j)
        {
            if (!A.IsSquare)
                throw new InvalidOperationException("Трансквенция неквадратной матрицы невозможна");

            var result = GetUnitaryMatryx(A.N);
            for (var i = 0; i < A.N; i++)
                result[i, j] = i == j ? 1 / A[j, j] : -A[i, j] / A[j, j];
            return result;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        private readonly int _N;

        /// <summary>Число столбцов матриц</summary>
        private readonly int _M;

        /// <summary>Элементы матрицы</summary>
        private readonly long[,] _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        public int N => _N;

        /// <summary>Число столбцов матрицы</summary>
        public int M => _M;

        /// <summary>Элемент матрицы</summary>
        /// <param name="i">Номер строки (элемента в столбце)</param>
        /// <param name="j">Номер столбца (элемента в строке)</param>
        /// <returns>Элемент матрицы</returns>
        public long this[int i, int j] { get => _Data[i, j]; set => _Data[i, j] = value; }

        /// <summary>Вектор-стольбец</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы</returns>
        public MatrixLong this[int j] => GetCol(j);

        /// <summary>Матрица является квадратной матрицей</summary>
        public bool IsSquare => M == N;

        /// <summary>Матрица является столбцом</summary>
        public bool IsCol => !IsSquare && M == 1;

        /// <summary>Матрица является строкой</summary>
        public bool IsRow => !IsSquare && N == 1;

        /// <summary>Матрица является числом</summary>
        public bool IsDigit => N == 1 && M == 1;

        public MatrixLong T => GetTransponse();

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Матрица</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        public MatrixLong(int N, int M)
        {
            _N = N;
            _M = M;
            _Data = new long[N, M];
        }

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        public MatrixLong(int N) : this(N, N) { }

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        public MatrixLong(int N, Func<int, int, long> CreateFunction) : this(N, N, CreateFunction) { }

        /// <summary>Матрица</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        public MatrixLong(int N, int M, Func<int, int, long> CreateFunction)
            : this(N, M)
        {
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    _Data[i, j] = CreateFunction(i, j);
        }

        //[CLSCompliant(false)]
        public MatrixLong(long[,] Data)
            : this(Data.GetLength(0), Data.GetLength(1))
        {
            for (var i = 0; i < _N; i++)
                for (var j = 0; j < _M; j++)
                    _Data[i, j] = Data[i, j];
        }

        public MatrixLong(IList<long> DataRow)
            : this(DataRow.Count, 1)
        {
            for (var i = 0; i < _N; i++)
                _Data[i, 0] = DataRow[i];
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить столбец матрицы</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы номер j</returns>
        public MatrixLong GetCol(int j)
        {
            var lv_A = new MatrixLong(N, 1);
            for (var i = 0; i < N; i++) lv_A[i, j] = this[i, j];
            return lv_A;
        }

        /// <summary>Получить строку матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <returns>Строка матрицы номер i</returns>
        public MatrixLong GetRow(int i)
        {
            var lv_A = new MatrixLong(1, M);
            for (var j = 0; j < M; j++) lv_A[i, j] = this[i, j];
            return lv_A;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <returns></returns>
        public MatrixLong GetTriangle()
        {
            var result = (MatrixLong)Clone();
            var lv_RowCount = N;
            var lv_ColCount = M;
            var row = new long[lv_ColCount];
            for (var lv_FirstRowIndex = 0; lv_FirstRowIndex < lv_RowCount - 1; lv_FirstRowIndex++)
            {
                var lv_A = result[lv_FirstRowIndex, lv_FirstRowIndex]; //Захватываем первый элемент строки
                for (var lv_RowElementI = lv_FirstRowIndex; lv_RowElementI < result.M; lv_RowElementI++) //Нормируем строку по первому элементу
                    row[lv_RowElementI] = result[lv_FirstRowIndex, lv_RowElementI] / lv_A;

                for (var i = lv_FirstRowIndex + 1; i < lv_RowCount; i++) //Для всех оставшихся строк:
                {
                    lv_A = result[i, lv_FirstRowIndex]; //Захватываем первый элемент строки
                    for (var j = lv_FirstRowIndex; j < lv_ColCount; j++)
                        result[i, j] -= lv_A * row[j]; //Вычитаем рабочую строку, домноженную на первый элемент
                }
            }
            return result;
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <returns>Обратная матрица</returns>
        public MatrixLong GetImverse()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Обратная матрица существует только для квадратной матрицы");

            var result = GetTransvection(this, 0);
            for (var i = 1; i < N; i++)
                result *= GetTransvection(this, i);
            return result;
        }

        /// <summary>Транспонирование матрицы</summary>
        /// <returns>Транспонированная матрица</returns>
        public MatrixLong GetTransponse()
        {
            var Result = new MatrixLong(M, N);

            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    Result[j, i] = this[i, j];

            return Result;
        }

        /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
        public MatrixLong GetAdjunct(int n, int m) => ((n + m) % 2 == 0 ? 1 : -1) * GetMinor(n, m).GetDeterminant();

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        public MatrixLong GetMinor(int n, int m)
        {
            var result = new MatrixLong(N - 1, M - 1);

            var i0 = 0;
            for (var i = 0; i < N; i++)
                if (i != n)
                {
                    var j0 = 0;
                    for (var j = 0; j < _M; j++)
                        if (j != m) result[i0, j0++] = this[i, j];
                    i0++;
                }
            return result;
        }

        /// <summary>Определитель матрицы</summary>
        public long GetDeterminant()
        {
            if (_N != _M)
                throw new InvalidOperationException("Нельзя найти определитель неквадратной матрицы!");
            var n = _N;
            if (n == 1) return this[0, 0];

            if (n == 2) return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];

            var DArray = (long[,])_Data.Clone();

            long det = 1;
            for (var k = 0; k <= n; k++)
            {
                int i;
                int j;
                if (DArray[k, k] == 0)
                {
                    j = k;
                    while (j < n && DArray[k, j] == 0) j++;

                    if (DArray[k, j] == 0) return 0;

                    for (i = k; i <= n; i++)
                    {
                        var save = DArray[i, j];
                        DArray[i, j] = DArray[i, k];
                        DArray[i, k] = save;
                    }
                    det = -det;
                }
                var ArrayK = DArray[k, k];

                det *= ArrayK;

                if (k >= n) continue;

                var k1 = k + 1;
                for (i = k1; i <= n; i++)
                    for (j = k1; j <= n; j++)
                        DArray[i, j] -= DArray[i, k] * DArray[k, j] / ArrayK;
            }

            //MatrixLong L, U, P;
            //GetLUDecomposition(out L, out U, out P);
            //long det = 1;
            //for(var i = 0; i < N; i++)
            //    det *= L[i, i] * U[i, i];

            //long det = 0;
            //for(int j = 0, k = 1; j < M; j++, k *= -1)
            //    det += this[0, j] * k * GetMinor(0, j).GetDeterminant();

            return det;
        }

        public void GetLUDecomposition(out MatrixLong L, out MatrixLong U, out MatrixLong P)
        {
            LUDecomposition(_Data, out var l, out var u, out var p);
            L = new MatrixLong(l);
            U = new MatrixLong(u);
            P = new MatrixLong(p);
        }

        /// <summary>
        /// Returns the LU Decomposition of a matrix. 
        /// the output is: lower triangular matrix L, upper
        /// triangular matrix U, and permutation matrix P so that
        ///	P*X = L*U.
        /// In case of an error the error is raised as an exception.
        /// Note: This method is based on the 'LU Decomposition and Its Applications'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.  
        /// </summary>
        /// <param name="Mat">Array which will be LU Decomposed</param>
        /// <param name="L">An array where the lower traingular matrix is returned</param>
        /// <param name="U">An array where the upper traingular matrix is returned</param>
        /// <param name="P">An array where the permutation matrix is returned</param>
        private static void LUDecomposition(long[,] Mat, out long[,] L, out long[,] U, out long[,] P)
        {
            var A = (long[,])Mat.Clone();
            var Rows = Mat.GetUpperBound(0);
            var Cols = Mat.GetUpperBound(1);

            if (Rows != Cols) throw new ArgumentException("Матрица не квадратная", nameof(Mat));


            var N = Rows;
            var indexes = new int[N + 1];
            var V = new long[N * 10];

            int i, j;
            long a_max;
            for (i = 0; i <= N; i++)
            {
                a_max = 0;
                for (j = 0; j <= N; j++)
                    if (Math.Abs(A[i, j]) > a_max)
                        a_max = Math.Abs(A[i, j]);

                if (a_max == 0)
                    throw new ArgumentException("Матрица вырождена", nameof(Mat));

                V[i] = 1 / a_max;
            }

            for (j = 0; j <= N; j++)
            {
                int k;
                long Sum;
                if (j > 0)
                    for (i = 0; i < j; i++)
                    {
                        Sum = A[i, j];
                        if (i <= 0) continue;
                        for (k = 0; k < i; k++)
                            Sum -= A[i, k] * A[k, j];
                        A[i, j] = Sum;
                    }

                a_max = 0;
                long Dum;
                var i_max = 0;
                for (i = j; i <= N; i++)
                {
                    Sum = A[i, j];
                    if (j > 0)
                    {
                        for (k = 0; k < j; k++)
                            Sum -= A[i, k] * A[k, j];
                        A[i, j] = Sum;
                    }
                    Dum = V[i] * Math.Abs(Sum);
                    if (Dum < a_max) continue;
                    i_max = i;
                    a_max = Dum;
                }

                if (j != i_max)
                {
                    for (k = 0; k <= N; k++)
                    {
                        Dum = A[i_max, k];
                        A[i_max, k] = A[j, k];
                        A[j, k] = Dum;
                    }
                    V[i_max] = V[j];
                }

                indexes[j] = i_max;

                if (j == N) continue;

                // ReSharper disable RedundantCast
                if (A[j, j] == 0)
                    A[j, j] = (long)1E-20;
                // ReSharper restore RedundantCast

                Dum = 1 / A[j, j];

                for (i = j + 1; i <= N; i++)
                    A[i, j] = A[i, j] * Dum;
            }

            // ReSharper disable RedundantCast
            if (A[N, N] == 0)
                A[N, N] = (long)1E-20;
            // ReSharper restore RedundantCast

            var count = 0;
            var l = new long[N + 1, N + 1];
            var u = new long[N + 1, N + 1];

            for (i = 0; i <= N; i++, count++)
                for (j = 0; j <= count; j++)
                {
                    if (i != 0) l[i, j] = A[i, j];
                    if (i == j) l[i, j] = 1;
                    u[N - i, N - j] = A[N - i, N - j];
                }

            L = l;
            U = u;

            P = Identity(N + 1);
            for (i = 0; i <= N; i++)
                P.SwapRows(i, indexes[i]);
        }

        private static long[,] Identity(int n)
        {
            var temp = new long[n, n];
            for (var i = 0; i < n; i++)
                temp[i, i] = 1;
            return temp;
        }

        /* -------------------------------------------------------------------------------------------- */

        public override string ToString() => $"MatrixLong[{N}x{M}]";

        public string ToStringFormat(string Format) => ToStringFormat('\t', Format);

        public string ToStringFormat(char Splitter) => ToStringFormat(Splitter, "r");

        public string ToStringFormat(char Splitter = '\t', string Format = "r")
        {
            var result = new StringBuilder();

            for (var i = 0; i < _N; i++)
            {
                var lv_Str = _Data[i, 0].ToString(Format);
                for (var j = 1; j < _M; j++)
                    lv_Str += Splitter + _Data[i, j].ToString(Format);
                result.AppendLine(lv_Str);
            }
            return result.ToString();
        }

        /* -------------------------------------------------------------------------------------------- */

        #region ICloneable Members

        /// <summary>Клонирование матрицы</summary>
        /// <returns>Копия текущей матрицы</returns>
        public object Clone()
        {
            var result = new MatrixLong(N, M);
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) result[i, j] = this[i, j];
            return result;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        public static bool operator ==(MatrixLong A, MatrixLong B) => A is null && B is null || !(A is null) && !(B is null) && A.Equals(B);

        public static bool operator !=(MatrixLong A, MatrixLong B) => !(A == B);

        public static MatrixLong operator +(MatrixLong M, long x)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] + x;
            return result;
        }

        public static MatrixLong operator +(long x, MatrixLong M)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] + x;
            return result;
        }

        public static MatrixLong operator -(MatrixLong M, long x)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] - x;
            return result;
        }

        public static MatrixLong operator -(long x, MatrixLong M)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] - x;
            return result;
        }

        public static MatrixLong operator *(MatrixLong M, long x)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] * x;
            return result;
        }

        public static MatrixLong operator *(long x, MatrixLong M)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] * x;
            return result;
        }

        //[CLSCompliant(false)]
        public static MatrixLong operator *(long[,] A, MatrixLong B) => (MatrixLong)A * B;

        //[CLSCompliant(false)]
        public static MatrixLong operator *(long[] A, MatrixLong B) => (MatrixLong)A * B;

        //[CLSCompliant(false)]
        public static MatrixLong operator *(MatrixLong A, long[] B) => A * (MatrixLong)B;

        //[CLSCompliant(false)]
        public static MatrixLong operator *(MatrixLong A, long[,] B) => A * (MatrixLong)B;

        public static MatrixLong operator /(MatrixLong M, long x)
        {
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] / x;
            return result;
        }

        public static MatrixLong operator /(long x, MatrixLong M)
        {
            M = M.GetImverse();
            var result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] * x;
            return result;
        }

        /// <summary>Оператор сложения двух матриц</summary>
        /// <param name="A">Первое слогаемое</param>
        /// <param name="B">Второе слогаемое</param>
        /// <returns>Сумма двух матриц</returns>
        public static MatrixLong operator +(MatrixLong A, MatrixLong B)
        {
            if (A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            var result = new MatrixLong(A.N, A.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    result[i, j] = A[i, j] + B[i, j];

            return result;
        }

        /// <summary>Оператор разности двух матриц</summary>
        /// <param name="A">Уменьшаемое</param>
        /// <param name="B">Вычитаемое</param>
        /// <returns>Разность двух матриц</returns>
        public static MatrixLong operator -(MatrixLong A, MatrixLong B)
        {
            if (A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            var result = new MatrixLong(A.N, A.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    result[i, j] = A[i, j] - B[i, j];

            return result;
        }

        /// <summary>Оператор произведения двух матриц</summary>
        /// <param name="A">Первый сомножитель</param>
        /// <param name="B">Второй сомножитель</param>
        /// <returns>Произведение двух матриц</returns>
        public static MatrixLong operator *(MatrixLong A, MatrixLong B)
        {
            if (A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var result = new MatrixLong(A.N, B.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    for (var k = 0; k < A.M; k++)
                        result[i, j] += A[i, k] * B[k, j];

            return result;
        }

        /// <summary>Оператор деления двух матриц</summary>
        /// <param name="A">Делимое</param>
        /// <param name="B">Делитель</param>
        /// <returns>Частное двух матриц</returns>
        public static MatrixLong operator /(MatrixLong A, MatrixLong B)
        {
            B = B.GetImverse();
            if (A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var result = new MatrixLong(A.N, B.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    for (var k = 0; k < A.M; k++)
                        result[i, j] += A[i, k] * B[k, j];

            return result;
        }

        /// <summary>Конкатинация двух матриц (либо по строкам, либо по столбцам)</summary>
        /// <param name="A">Первое слогаемое</param>
        /// <param name="B">Второе слогаемое</param>
        /// <returns>Объединённая матрица</returns>
        public static MatrixLong operator |(MatrixLong A, MatrixLong B)
        {
            MatrixLong result;
            if (A.M == B.M) // Конкатинация по строкам
            {
                result = new MatrixLong(A.N + B.N, A.M);
                for (var i = 0; i < A.N; i++)
                    for (var j = 0; j < A.M; j++)
                        result[i, j] = A[i, j];
                var i0 = A.N;
                for (var i = 0; i < B.N; i++)
                    for (var j = 0; j < B.M; j++)
                        result[i + i0, j] = B[i, j];

            }
            else if (A.N == B.N) //Конкатинация по строкам
            {
                result = new MatrixLong(A.N, A.M + B.M);
                for (var i = 0; i < A.N; i++)
                    for (var j = 0; j < A.M; j++)
                        result[i, j] = A[i, j];
                var j0 = A.M;
                for (var i = 0; i < B.N; i++)
                    for (var j = 0; j < B.M; j++)
                        result[i, j + j0] = B[i, j];
            }
            else
                throw new InvalidOperationException("Конкатинация возможна только по строкам, или по столбцам");

            return result;
        }



        /* -------------------------------------------------------------------------------------------- */

        /// <summary>
        /// Оператор неявного преведения типа вещественного числа двойной точнойсти к типу 
        /// Матрица порядка 1х1
        /// </summary>
        /// <param name="X">Приводимое число</param>
        /// <returns>Матрица порадка 1х1</returns>
        public static implicit operator MatrixLong(long X) => new MatrixLong(1, 1) { [0, 0] = X };

        public static explicit operator long[,](MatrixLong M) => (long[,])M._Data.Clone();

        //[CLSCompliant(false)]
        public static explicit operator MatrixLong(long[,] Data) => new MatrixLong(Data);

        public static explicit operator MatrixLong(long[] Data) => new MatrixLong(Data);

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable<MatrixLong> Members

        public bool Equals(MatrixLong other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._N == _N && other._M == _M && Equals(other._Data, _Data);
        }

        bool IEquatable<MatrixLong>.Equals(MatrixLong other) => Equals(other);

        #endregion

        public override bool Equals(object obj) => !(obj is null) && (ReferenceEquals(this, obj) || obj.GetType() == typeof(MatrixLong) && Equals((MatrixLong) obj));

        public override int GetHashCode()
        {
            unchecked
            {
                var result = _N;
                result = (result * 397) ^ _M;
                result = (result * 397) ^ (_Data != null ? _Data.GetHashCode() : 0);
                return result;
            }
        }
    }
}
