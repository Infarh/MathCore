using System;
using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Linq;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    /// <summary>Матрица NxM</summary>
    /// <remarks>
    /// i (первый индекс) - номер строки, 
    /// j (второй индекс) - номер столбца
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
    public class MatrixInt : ICloneable, IEquatable<MatrixInt>, IIndexable<int, int, int>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="N">Размерность матрицы</param>
        /// <returns>Единичная матрица размерности NxN</returns>
        [DST]
        public static MatrixInt GetUnitaryMatrix(int N)
        {
            var Result = new MatrixInt(N);
            for (var i = 0; i < N; i++) Result[i, i] = 1;
            return Result;
        }

        /// <summary>Трансвекция матрицы</summary>
        /// <param name="A">Трансвецируемая матрица</param>
        /// <param name="j">Опорный столбец</param>
        /// <returns>Трансвекция матрицы А</returns>
        public static MatrixInt GetTransvection(MatrixInt A, int j)
        {
            if (!A.IsSquare)
                throw new InvalidOperationException("Трансвенция неквадратной матрицы невозможна");

            var result = GetUnitaryMatrix(A.N);
            for (var i = 0; i < A.N; i++)
                result[i, j] = i == j ? 1 / A[j, j] : -A[i, j] / A[j, j];
            return result;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        private readonly int _N;

        /// <summary>Число столбцов матрицы</summary>
        private readonly int _M;

        /// <summary>Элементы матрицы</summary>
        private readonly int[,] _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        public int N => _N;

        /// <summary>Число столбцов матрицы</summary>
        public int M => _M;

        /// <summary>Элемент матрицы</summary>
        /// <param name="i">Номер строки (элемента в столбце)</param>
        /// <param name="j">Номер столбца (элемента в строке)</param>
        /// <returns>Элемент матрицы</returns>
        public int this[int i, int j]
        {
            [DST]
            get => _Data[i, j];
            [DST]
            set => _Data[i, j] = value;
        }

        /// <summary>Вектор-столбец</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы</returns>
        public MatrixInt this[int j] => GetCol(j);

        /// <summary>Матрица является квадратной матрицей</summary>
        public bool IsSquare => M == N;

        /// <summary>Матрица является столбцом</summary>
        public bool IsCol => !IsSquare && M == 1;

        /// <summary>Матрица является строкой</summary>
        public bool IsRow => !IsSquare && N == 1;

        /// <summary>Матрица является числом</summary>
        public bool IsDigit => N == 1 && M == 1;

        public MatrixInt T => GetTranspose();

        public int Norm_m
        {
            get
            {
                var v = new int[N];
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        v[i] += Math.Abs(_Data[i, j]);
                return v.Max();
            }
        }

        public int Norm_l
        {
            get
            {
                var v = new int[M];
                for (var j = 0; j < M; j++)
                    for (var i = 0; i < N; i++)
                        v[j] += Math.Abs(_Data[i, j]);
                return v.Max();
            }
        }

        public double Norm_k
        {
            get
            {
                var v = default(int);
                for (var i = 0; i < N; i++)
                    for (var j = 0; j < M; j++)
                        v += _Data[i, j] * _Data[i, j];
                return Math.Sqrt(v);
            }
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Матрица</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        [DST]
        public MatrixInt(int N, int M) => _Data = new int[_N = N, _M = M];

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        [DST]
        public MatrixInt(int N) : this(N, N) { }

        /// <summary>Метод определения значения элемента матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <param name="j">Номер столбца</param>
        /// <returns>Значение элемента матрицы M[<paramref name="i"/>, <paramref name="j"/>]</returns>
        public delegate int MatrixIntItemCreator(int i, int j);

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST]
        public MatrixInt(int N, MatrixIntItemCreator CreateFunction) : this(N, N, CreateFunction) { }

        /// <summary>Матрица</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST]
        public MatrixInt(int N, int M, MatrixIntItemCreator CreateFunction)
            : this(N, M)
        {
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    _Data[i, j] = CreateFunction(i, j);
        }

        [DST]
        public MatrixInt(int[,] Data)
            : this(Data.GetLength(0), Data.GetLength(1))
        {
            for (var i = 0; i < _N; i++)
                for (var j = 0; j < _M; j++)
                    _Data[i, j] = Data[i, j];
        }

        [DST]
        public MatrixInt(IList<int> DataRow)
            : this(DataRow.Count, 1)
        {
            for (var i = 0; i < _N; i++)
                _Data[i, 0] = DataRow[i];
        }

        public MatrixInt(IEnumerable<IEnumerable<int>> Items) : this(GetElements(Items)) { }

        private static int[,] GetElements(IEnumerable<IEnumerable<int>> Items)
        {
            var cols = Items.Select(col => col.ToListFast()).ToList();
            var cols_count = cols.Count;
            var rows_count = cols.Max(col => col.Count);
            var data = new int[rows_count, cols_count];
            for (var j = 0; j < cols_count; j++)
            {
                var col = cols[j];
                for (var i = 0; i < col.Count && i < rows_count; i++)
                    data[i, j] = col[i];
            }
            return data;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить столбец матрицы</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы номер j</returns>
        [DST]
        public MatrixInt GetCol(int j)
        {
            var a = new MatrixInt(N, 1);
            for (var i = 0; i < N; i++) a[i, j] = this[i, j];
            return a;
        }

        /// <summary>Получить строку матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <returns>Строка матрицы номер i</returns>
        [DST]
        public MatrixInt GetRow(int i)
        {
            var a = new MatrixInt(1, M);
            for (var j = 0; j < M; j++) a[i, j] = this[i, j];
            return a;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
        /// <returns></returns>
        public MatrixInt GetTriangle()
        {
            var result = (MatrixInt)Clone();
            var row_count = N;
            var col_count = M;
            var row = new int[col_count];
            for (var first_row_index = 0; first_row_index < row_count - 1; first_row_index++)
            {
                var a = result[first_row_index, first_row_index]; //Захватываем первый элемент строки
                for (var row_element_i = first_row_index; row_element_i < result.M; row_element_i++) //Нормируем строку по первому элементу
                    row[row_element_i] = result[first_row_index, row_element_i] / a;

                for (var i = first_row_index + 1; i < row_count; i++) //Для всех оставшихся строк:
                {
                    a = result[i, first_row_index]; //Захватываем первый элемент строки
                    for (var j = first_row_index; j < col_count; j++)
                        result[i, j] -= a * row[j]; //Вычитаем рабочую строку, домноженную на первый элемент
                }
            }
            return result;
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <returns>Обратная матрица</returns>
        public MatrixInt GetInverse()
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
        [DST]
        public MatrixInt GetTranspose()
        {
            var Result = new MatrixInt(M, N);

            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    Result[j, i] = this[i, j];

            return Result;
        }

        /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
        public MatrixInt GetAdjunct(int n, int m) => ((n + m) % 2 == 0 ? 1 : -1) * GetMinor(n, m).GetDeterminant();

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        public MatrixInt GetMinor(int n, int m)
        {
            var result = new MatrixInt(N - 1, M - 1);

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
        public int GetDeterminant()
        {
            if (!IsSquare) throw new InvalidOperationException("Нельзя найти определитель неквадратной матрицы!");
            var n = _N;
            if (n == 1) return this[0, 0];

            if (n == 2) return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];

            var DArray = (int[,])_Data.Clone();

            var det = 1;
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

            //MatrixInt L, U, P;
            //GetLUDecomposition(out L, out U, out P);
            //int det = 1;
            //for(var i = 0; i < N; i++)
            //    det *= L[i, i] * U[i, i];

            //int det = 0;
            //for(int j = 0, k = 1; j < M; j++, k *= -1)
            //    det += this[0, j] * k * GetMinor(0, j).GetDeterminant();

            return det;
        }

        //public void GetLUDecomposition(out MatrixInt L, out MatrixInt U, out MatrixInt P)
        //{
        //    int[,] l, u, p;
        //    LUDecomposition(_Data, out l, out u, out p);
        //    L = new MatrixInt(l);
        //    U = new MatrixInt(u);
        //    P = new MatrixInt(p);
        //}

        ///// <summary>
        ///// Returns the LU Decomposition of a matrix. 
        ///// the output is: lower triangular matrix L, upper
        ///// triangular matrix U, and permutation matrix P so that
        /////	P*X = L*U.
        ///// In case of an error the error is raised as an exception.
        ///// Note - This method is based on the 'LU Decomposition and Its Applications'
        ///// section of Numerical Recipes in C by William H. Press,
        ///// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        ///// University of Cambridge Press 1992.  
        ///// </summary>
        ///// <param name="Mat">Array which will be LU Decomposed</param>
        ///// <param name="L">An array where the lower triangular matrix is returned</param>
        ///// <param name="U">An array where the upper triangular matrix is returned</param>
        ///// <param name="P">An array where the permutation matrix is returned</param>
        //private static void LUDecomposition(int[,] Mat, out int[,] L, out int[,] U, out int[,] P)
        //{
        //    var A = (int[,])Mat.Clone();
        //    var Rows = Mat.GetUpperBound(0);
        //    var Cols = Mat.GetUpperBound(1);

        //    if(Rows != Cols) throw new ArgumentException("Матрица не квадратная", nameof(Mat));


        //    var N = Rows;
        //    var lv_Indexes = new int[N + 1];
        //    var V = new int[N * 10];

        //    int i, j;
        //    for(i = 0; i <= N; i++)
        //    {
        //        var lv_AMax = 0d;
        //        for(j = 0; j <= N; j++)
        //            if(Math.Abs(A[i, j]) > lv_AMax)
        //                lv_AMax = Math.Abs(A[i, j]);

        //        if(lv_AMax.Equals(0))
        //            throw new ArgumentException("Матрица вырождена", nameof(Mat));

        //        V[i] = 1 / lv_AMax;
        //    }

        //    for(j = 0; j <= N; j++)
        //    {
        //        int k;
        //        int Sum;
        //        if(j > 0)
        //            for(i = 0; i < j; i++)
        //            {
        //                Sum = A[i, j];
        //                if(i <= 0) continue;
        //                for(k = 0; k < i; k++)
        //                    Sum -= A[i, k] * A[k, j];
        //                A[i, j] = Sum;
        //            }

        //        var lv_AMax = 0d;
        //        int Dum;
        //        var lv_IMax = 0;
        //        for(i = j; i <= N; i++)
        //        {
        //            Sum = A[i, j];
        //            if(j > 0)
        //            {
        //                for(k = 0; k < j; k++)
        //                    Sum -= A[i, k] * A[k, j];
        //                A[i, j] = Sum;
        //            }
        //            Dum = V[i] * Math.Abs(Sum);
        //            if(Dum < lv_AMax) continue;
        //            lv_IMax = i;
        //            lv_AMax = Dum;
        //        }

        //        if(j != lv_IMax)
        //        {
        //            for(k = 0; k <= N; k++)
        //            {
        //                Dum = A[lv_IMax, k];
        //                A[lv_IMax, k] = A[j, k];
        //                A[j, k] = Dum;
        //            }
        //            V[lv_IMax] = V[j];
        //        }

        //        lv_Indexes[j] = lv_IMax;

        //        if(j == N) continue;

        //        // ReSharper disable RedundantCast
        //        if(A[j, j].Equals(0))
        //            A[j, j] = (int)1E-20;
        //        // ReSharper restore RedundantCast

        //        Dum = 1 / A[j, j];

        //        for(i = j + 1; i <= N; i++)
        //            A[i, j] = A[i, j] * Dum;
        //    }

        //    // ReSharper disable RedundantCast
        //    if(A[N, N].Equals(0))
        //        A[N, N] = (int)1E-20;
        //    // ReSharper restore RedundantCast

        //    var count = 0;
        //    var l = new int[N + 1, N + 1];
        //    var u = new int[N + 1, N + 1];

        //    for(i = 0; i <= N; i++, count++)
        //        for(j = 0; j <= count; j++)
        //        {
        //            if(i != 0) l[i, j] = A[i, j];
        //            if(i == j) l[i, j] = 1;
        //            u[N - i, N - j] = A[N - i, N - j];
        //        }

        //    L = l;
        //    U = u;

        //    P = Identity(N + 1);
        //    for(i = 0; i <= N; i++)
        //        P.SwapRows(i, lv_Indexes[i]);
        //}

        private static int[,] Identity(int n)
        {
            var temp = new int[n, n];
            for (var i = 0; i < n; i++)
                temp[i, i] = 1;
            return temp;
        }

        /* -------------------------------------------------------------------------------------------- */

        [DST]
        public override string ToString() => $"MatrixInt[{N}x{M}]";

        [DST]
        public string ToStringFormat(string Format) => ToStringFormat('\t', Format);

        //[DST] public string ToStringFormat(char Splitter) { return ToStringFormat(Splitter, "r"); }

        public string ToStringFormat(char Splitter = '\t', string Format = "r")
        {
            var result = new StringBuilder();

            for (var i = 0; i < _N; i++)
            {
                var str = _Data[i, 0].ToString(Format);
                for (var j = 1; j < _M; j++)
                    str += Splitter + _Data[i, j].ToString(Format);
                result.AppendLine(str);
            }
            return result.ToString();
        }

        /* -------------------------------------------------------------------------------------------- */

        #region ICloneable Members

        /// <summary>Клонирование матрицы</summary>
        /// <returns>Копия текущей матрицы</returns>
        [DST]
        public object Clone()
        {
            var result = new MatrixInt(N, M);
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) result[i, j] = this[i, j];
            return result;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        public static bool operator ==(MatrixInt A, MatrixInt B) => A is null && (B is null) || A is { } && B is { } && A.Equals(B);

        public static bool operator !=(MatrixInt A, MatrixInt B) => !(A == B);

        [DST]
        public static MatrixInt operator +(MatrixInt M, int x)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] + x;
            return result;
        }

        [DST]
        public static MatrixInt operator +(int x, MatrixInt M)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] + x;
            return result;
        }

        [DST]
        public static MatrixInt operator -(MatrixInt M, int x)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] - x;
            return result;
        }

        [DST]
        public static MatrixInt operator -(int x, MatrixInt M)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = x - M[i, j];
            return result;
        }

        [DST]
        public static MatrixInt operator *(MatrixInt M, int x)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] * x;
            return result;
        }

        [DST]
        public static MatrixInt operator *(int x, MatrixInt M)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] * x;
            return result;
        }

        [DST]
        public static MatrixInt operator *(int[,] A, MatrixInt B) => (MatrixInt)A * B;

        [DST]
        public static MatrixInt operator *(int[] A, MatrixInt B) => (MatrixInt)A * B;

        [DST]
        public static MatrixInt operator *(MatrixInt A, int[] B) => A * (MatrixInt)B;

        [DST]
        public static MatrixInt operator *(MatrixInt A, int[,] B) => A * (MatrixInt)B;

        [DST]
        public static MatrixInt operator /(MatrixInt M, int x)
        {
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] / x;
            return result;
        }

        public static MatrixInt operator /(int x, MatrixInt M)
        {
            M = M.GetInverse();
            var result = new MatrixInt(M.N, M.M);
            for (var i = 0; i < M.N; i++)
                for (var j = 0; j < M.M; j++)
                    result[i, j] = M[i, j] * x;
            return result;
        }

        /// <summary>Оператор сложения двух матриц</summary>
        /// <param name="A">Первое слагаемое</param>
        /// <param name="B">Второе слагаемое</param>
        /// <returns>Сумма двух матриц</returns>
        [DST]
        public static MatrixInt operator +(MatrixInt A, MatrixInt B)
        {
            if (A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            var result = new MatrixInt(A.N, A.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    result[i, j] = A[i, j] + B[i, j];

            return result;
        }

        /// <summary>Оператор разности двух матриц</summary>
        /// <param name="A">Уменьшаемое</param>
        /// <param name="B">Вычитаемое</param>
        /// <returns>Разность двух матриц</returns>
        [DST]
        public static MatrixInt operator -(MatrixInt A, MatrixInt B)
        {
            if (A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            var result = new MatrixInt(A.N, A.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    result[i, j] = A[i, j] - B[i, j];

            return result;
        }

        /// <summary>Оператор произведения двух матриц</summary>
        /// <param name="A">Первый сомножитель</param>
        /// <param name="B">Второй сомножитель</param>
        /// <returns>Произведение двух матриц</returns>
        [DST]
        public static MatrixInt operator *(MatrixInt A, MatrixInt B)
        {
            if (A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var result = new MatrixInt(A.N, B.M);

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
        public static MatrixInt operator /(MatrixInt A, MatrixInt B)
        {
            B = B.GetInverse();
            if (A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var result = new MatrixInt(A.N, B.M);

            for (var i = 0; i < result.N; i++)
                for (var j = 0; j < result.M; j++)
                    for (var k = 0; k < A.M; k++)
                        result[i, j] += A[i, k] * B[k, j];

            return result;
        }

        /// <summary>Конкатенация двух матриц (либо по строкам, либо по столбцам)</summary>
        /// <param name="A">Первое слагаемое</param>
        /// <param name="B">Второе слагаемое</param>
        /// <returns>Объединённая матрица</returns>
        public static MatrixInt operator |(MatrixInt A, MatrixInt B)
        {
            MatrixInt result;
            if (A.M == B.M) // Конкатенация по строкам
            {
                result = new MatrixInt(A.N + B.N, A.M);
                for (var i = 0; i < A.N; i++)
                    for (var j = 0; j < A.M; j++)
                        result[i, j] = A[i, j];
                var i0 = A.N;
                for (var i = 0; i < B.N; i++)
                    for (var j = 0; j < B.M; j++)
                        result[i + i0, j] = B[i, j];

            }
            else if (A.N == B.N) //Конкатенация по строкам
            {
                result = new MatrixInt(A.N, A.M + B.M);
                for (var i = 0; i < A.N; i++)
                    for (var j = 0; j < A.M; j++)
                        result[i, j] = A[i, j];
                var j0 = A.M;
                for (var i = 0; i < B.N; i++)
                    for (var j = 0; j < B.M; j++)
                        result[i, j + j0] = B[i, j];
            }
            else
                throw new InvalidOperationException("Конкатенация возможна только по строкам, или по столбцам");

            return result;
        }



        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Оператор неявного приведения типа вещественного числа двойной точности к типу Матрица порядка 1х1</summary>
        /// <param name="X">Приводимое число</param><returns>Матрица порядка 1х1</returns>
        [DST]
        public static implicit operator MatrixInt(int X) => new MatrixInt(1, 1) { [0, 0] = X };

        [DST]
        public static explicit operator int[,](MatrixInt M) => (int[,])M._Data.Clone();

        [DST]
        public static explicit operator MatrixInt(int[,] Data) => new MatrixInt(Data);

        [DST]
        public static explicit operator MatrixInt(int[] Data) => new MatrixInt(Data);

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable<MatrixInt> Members

        public bool Equals(MatrixInt other) =>
            other is { }
            && (ReferenceEquals(this, other)
                || other._N == _N
                && other._M == _M
                && Equals(other._Data, _Data));

        [DST]
        bool IEquatable<MatrixInt>.Equals(MatrixInt other) => Equals(other);

        #endregion

        public override bool Equals(object obj) =>
            obj is { }
            && (ReferenceEquals(this, obj)
                || obj.GetType() == typeof(MatrixInt)
                && Equals((MatrixInt)obj));

        [DST]
        public override int GetHashCode()
        {
            unchecked
            {
                var result = _N;
                result = (result * 397) ^ _M;
                _Data.Foreach((i, j, v) => result = (result * 397) ^ i ^ j ^ v.GetHashCode());
                //result = (result * 397) ^ (_Data != null ? _Data.GetHashCode() : 0);
                return result;
            }
        }

        /* -------------------------------------------------------------------------------------------- */

        public int[,] GetData() => _Data;
    }
}