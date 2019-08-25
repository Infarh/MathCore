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

            var lv_Result = GetUnitaryMatryx(A.N);
            for (var i = 0; i < A.N; i++)
                lv_Result[i, j] = i == j ? 1 / A[j, j] : -A[i, j] / A[j, j];
            return lv_Result;
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
        public long this[int i, int j] { get { return _Data[i, j]; } set { _Data[i, j] = value; } }

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
            var lv_Result = (MatrixLong)Clone();
            var lv_RowCount = N;
            var lv_ColCount = M;
            var row = new long[lv_ColCount];
            for (var lv_FirstRowIndex = 0; lv_FirstRowIndex < lv_RowCount - 1; lv_FirstRowIndex++)
            {
                var lv_A = lv_Result[lv_FirstRowIndex, lv_FirstRowIndex]; //Захватываем первый элемент строки
                for (var lv_RowElementI = lv_FirstRowIndex; lv_RowElementI < lv_Result.M; lv_RowElementI++) //Нормируем строку по первому элементу
                    row[lv_RowElementI] = lv_Result[lv_FirstRowIndex, lv_RowElementI] / lv_A;

                for (var i = lv_FirstRowIndex + 1; i < lv_RowCount; i++) //Для всех оставшихся строк:
                {
                    lv_A = lv_Result[i, lv_FirstRowIndex]; //Захватываем первый элемент строки
                    for (var j = lv_FirstRowIndex; j < lv_ColCount; j++)
                        lv_Result[i, j] -= lv_A * row[j]; //Вычитаем рабочую строку, домноженную на первый элемент
                }
            }
            return lv_Result;
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <returns>Обратная матрица</returns>
        public MatrixLong GetImverse()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Обратная матрица существует только для квадратной матрицы");

            var lv_Result = GetTransvection(this, 0);
            for (var i = 1; i < N; i++)
                lv_Result *= GetTransvection(this, i);
            return lv_Result;
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
        public MatrixLong GetAdjunct(int n, int m)
        {
            return (((n + m) % 2) == 0 ? 1 : -1) * GetMinor(n, m).GetDeterminant();
        }

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        public MatrixLong GetMinor(int n, int m)
        {
            var lv_Result = new MatrixLong(N - 1, M - 1);

            var i0 = 0;
            for (var i = 0; i < N; i++)
                if (i != n)
                {
                    var j0 = 0;
                    for (var j = 0; j < _M; j++)
                        if (j != m) lv_Result[i0, j0++] = this[i, j];
                    i0++;
                }
            return lv_Result;
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
            long[,] l, u, p;
            LUDecomposition(_Data, out l, out u, out p);
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
            var lv_Indexex = new int[N + 1];
            var V = new long[N * 10];

            int i, j;
            long lv_AMax;
            for (i = 0; i <= N; i++)
            {
                lv_AMax = 0;
                for (j = 0; j <= N; j++)
                    if (Math.Abs(A[i, j]) > lv_AMax)
                        lv_AMax = Math.Abs(A[i, j]);

                if (lv_AMax == 0)
                    throw new ArgumentException("Матрица вырождена", nameof(Mat));

                V[i] = 1 / lv_AMax;
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

                lv_AMax = 0;
                long Dum;
                var lv_IMax = 0;
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
                    if (Dum < lv_AMax) continue;
                    lv_IMax = i;
                    lv_AMax = Dum;
                }

                if (j != lv_IMax)
                {
                    for (k = 0; k <= N; k++)
                    {
                        Dum = A[lv_IMax, k];
                        A[lv_IMax, k] = A[j, k];
                        A[j, k] = Dum;
                    }
                    V[lv_IMax] = V[j];
                }

                lv_Indexex[j] = lv_IMax;

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
                P.SwapRows(i, lv_Indexex[i]);
        }

        private static long[,] Identity(int n)
        {
            var temp = new long[n, n];
            for (var i = 0; i < n; i++)
                temp[i, i] = 1;
            return temp;
        }

        /* -------------------------------------------------------------------------------------------- */

        public override string ToString()
        {
            return string.Format("MatrixLong[{0}x{1}]", N, M);
        }

        public string ToStringFormat(string Format)
        {
            return ToStringFormat('\t', Format);
        }

        public string ToStringFormat(char Splitter)
        {
            return ToStringFormat(Splitter, "r");

        }

        public string ToStringFormat(char Splitter = '\t', string Format = "r")
        {
            var lv_Result = new StringBuilder();

            for (var i = 0; i < _N; i++)
            {
                var lv_Str = _Data[i, 0].ToString(Format);
                for (var j = 1; j < _M; j++)
                    lv_Str += Splitter + _Data[i, j].ToString(Format);
                lv_Result.AppendLine(lv_Str);
            }
            return lv_Result.ToString();
        }

        /* -------------------------------------------------------------------------------------------- */

        #region ICloneable Members

        /// <summary>Клонирование матрицы</summary>
        /// <returns>Копия текущей матрицы</returns>
        public object Clone()
        {
            var lv_Result = new MatrixLong(N, M);
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) lv_Result[i, j] = this[i, j];
            return lv_Result;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        public static bool operator ==(MatrixLong A, MatrixLong B)
        {
            if (((object)A == null) && (object)B == null) return true;
            if (((object)A == null) || ((object)B == null)) return false;
            return A.Equals(B);
        }

        public static bool operator !=(MatrixLong A, MatrixLong B)
        {
            return !(A == B);
        }

        public static MatrixLong operator +(MatrixLong M, long x)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] + x;
            return lv_Result;
        }

        public static MatrixLong operator +(long x, MatrixLong M)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] + x;
            return lv_Result;
        }

        public static MatrixLong operator -(MatrixLong M, long x)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] - x;
            return lv_Result;
        }

        public static MatrixLong operator -(long x, MatrixLong M)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] - x;
            return lv_Result;
        }

        public static MatrixLong operator *(MatrixLong M, long x)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] * x;
            return lv_Result;
        }

        public static MatrixLong operator *(long x, MatrixLong M)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] * x;
            return lv_Result;
        }

        //[CLSCompliant(false)]
        public static MatrixLong operator *(long[,] A, MatrixLong B)
        {
            return (((MatrixLong)A) * B);
        }

        //[CLSCompliant(false)]
        public static MatrixLong operator *(long[] A, MatrixLong B)
        {
            return (((MatrixLong)A) * B);
        }

        //[CLSCompliant(false)]
        public static MatrixLong operator *(MatrixLong A, long[] B)
        {
            return (A * ((MatrixLong)B));
        }

        //[CLSCompliant(false)]
        public static MatrixLong operator *(MatrixLong A, long[,] B)
        {
            return (A * ((MatrixLong)B));
        }

        public static MatrixLong operator /(MatrixLong M, long x)
        {
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] / x;
            return lv_Result;
        }

        public static MatrixLong operator /(long x, MatrixLong M)
        {
            M = M.GetImverse();
            var lv_Result = new MatrixLong(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    lv_Result[i, j] = M[i, j] * x;
            return lv_Result;
        }

        /// <summary>Оператор сложения двух матриц</summary>
        /// <param name="A">Первое слогаемое</param>
        /// <param name="B">Второе слогаемое</param>
        /// <returns>Сумма двух матриц</returns>
        public static MatrixLong operator +(MatrixLong A, MatrixLong B)
        {
            if (A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            var lv_Result = new MatrixLong(A.N, A.M);

            for (var i = 0; i < lv_Result.N; i++)
                for (var j = 0; j < lv_Result.M; j++)
                    lv_Result[i, j] = A[i, j] + B[i, j];

            return lv_Result;
        }

        /// <summary>Оператор разности двух матриц</summary>
        /// <param name="A">Уменьшаемое</param>
        /// <param name="B">Вычитаемое</param>
        /// <returns>Разность двух матриц</returns>
        public static MatrixLong operator -(MatrixLong A, MatrixLong B)
        {
            if (A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            var lv_Result = new MatrixLong(A.N, A.M);

            for (var i = 0; i < lv_Result.N; i++)
                for (var j = 0; j < lv_Result.M; j++)
                    lv_Result[i, j] = A[i, j] - B[i, j];

            return lv_Result;
        }

        /// <summary>Оператор произведения двух матриц</summary>
        /// <param name="A">Первый сомножитель</param>
        /// <param name="B">Второй сомножитель</param>
        /// <returns>Произведение двух матриц</returns>
        public static MatrixLong operator *(MatrixLong A, MatrixLong B)
        {
            if (A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var lv_Result = new MatrixLong(A.N, B.M);

            for (var i = 0; i < lv_Result.N; i++)
                for (var j = 0; j < lv_Result.M; j++)
                    for (var k = 0; k < A.M; k++)
                        lv_Result[i, j] += A[i, k] * B[k, j];

            return lv_Result;
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

            var lv_Result = new MatrixLong(A.N, B.M);

            for (var i = 0; i < lv_Result.N; i++)
                for (var j = 0; j < lv_Result.M; j++)
                    for (var k = 0; k < A.M; k++)
                        lv_Result[i, j] += A[i, k] * B[k, j];

            return lv_Result;
        }

        /// <summary>Конкатинация двух матриц (либо по строкам, либо по столбцам)</summary>
        /// <param name="A">Первое слогаемое</param>
        /// <param name="B">Второе слогаемое</param>
        /// <returns>Объединённая матрица</returns>
        public static MatrixLong operator |(MatrixLong A, MatrixLong B)
        {
            MatrixLong lv_Result;
            if (A.M == B.M) // Конкатинация по строкам
            {
                lv_Result = new MatrixLong(A.N + B.N, A.M);
                for (var i = 0; i < A.N; i++)
                    for (var j = 0; j < A.M; j++)
                        lv_Result[i, j] = A[i, j];
                var i0 = A.N;
                for (var i = 0; i < B.N; i++)
                    for (var j = 0; j < B.M; j++)
                        lv_Result[i + i0, j] = B[i, j];

            }
            else if (A.N == B.N) //Конкатинация по строкам
            {
                lv_Result = new MatrixLong(A.N, A.M + B.M);
                for (var i = 0; i < A.N; i++)
                    for (var j = 0; j < A.M; j++)
                        lv_Result[i, j] = A[i, j];
                var j0 = A.M;
                for (var i = 0; i < B.N; i++)
                    for (var j = 0; j < B.M; j++)
                        lv_Result[i, j + j0] = B[i, j];
            }
            else
                throw new InvalidOperationException("Конкатинация возможна только по строкам, или по столбцам");

            return lv_Result;
        }



        /* -------------------------------------------------------------------------------------------- */

        /// <summary>
        /// Оператор неявного преведения типа вещественного числа двойной точнойсти к типу 
        /// Матрица порядка 1х1
        /// </summary>
        /// <param name="X">Приводимое число</param>
        /// <returns>Матрица порадка 1х1</returns>
        public static implicit operator MatrixLong(long X)
        {
            var lv_Result = new MatrixLong(1, 1);
            lv_Result[0, 0] = X;
            return lv_Result;
        }

        public static explicit operator long[,](MatrixLong M)
        {
            return (long[,])M._Data.Clone();
        }

        //[CLSCompliant(false)]
        public static explicit operator MatrixLong(long[,] Data)
        {
            return new MatrixLong(Data);
        }

        public static explicit operator MatrixLong(long[] Data)
        {
            return new MatrixLong(Data);
        }

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable<MatrixLong> Members

        public bool Equals(MatrixLong other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._N == _N && other._M == _M && Equals(other._Data, _Data);
        }

        bool IEquatable<MatrixLong>.Equals(MatrixLong other)
        {
            return Equals(other);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(MatrixLong) && Equals((MatrixLong)obj);
        }

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
