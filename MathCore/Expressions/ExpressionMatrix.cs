using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using MathCore;
using MathCore.Extentions.Expressions;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    using DST = DebuggerStepThroughAttribute;

    /// <summary>Матрица выражений NxM</summary>
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
    public class ExpressionMatrix :
        ICloneable<ExpressionMatrix>,
        IEquatable<ExpressionMatrix>,
        IIndexable<int, int, Expression>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="N">Размерность матрицы</param>
        /// <returns>Единичная матрица размерности NxN</returns>
        [DST]
        public static ExpressionMatrix GetUnitaryMatryx(int N)
        {
            var v0 = 0d.ToExpression();
            var v1 = 1d.ToExpression();
            return new ExpressionMatrix(N, (i, j) => (i == j ? v1 : v0));
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        private readonly int _N;

        /// <summary>Число столбцов матриц</summary>
        private readonly int _M;

        /// <summary>Элементы матрицы</summary>
        private readonly Expression[,] _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        public int N => _N;

        /// <summary>Число столбцов матрицы</summary>
        public int M => _M;

        /// <summary>Элемент матрицы</summary>
        /// <param name="i">Номер строки (элемента в столбце) 0..N-1</param>
        /// <param name="j">Номер столбца (элемента в строке) 0..M-1</param>
        /// <returns>Элемент матрицы</returns>
        public Expression this[int i, int j]
        {
            [DST]
            get
            {
                Contract.Requires(i >= 0);
                Contract.Requires(j >= 0);
                Contract.Requires(i < N);
                Contract.Requires(j < M);

                return _Data[i, j];
            }
            [DST]
            set
            {
                Contract.Requires(i >= 0);
                Contract.Requires(j >= 0);
                Contract.Requires(i < N);
                Contract.Requires(j < M);
                Contract.Requires(value != null);

                _Data[i, j] = value.NodeType == ExpressionType.Lambda ? ((LambdaExpression)value).Body : value;
            }
        }

        /// <summary>Вектор-стольбец</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы</returns>
        public ExpressionMatrix this[int j]
        {
            [DST]
            get
            {
                Contract.Requires(j >= 0);
                Contract.Requires(j < M);
                return GetCol(j);
            }
        }

        /// <summary>Матрица является квадратной матрицей</summary>
        public bool IsSquare
        {
            [DST]
            get
            {
                Contract.Ensures(Contract.Result<bool>() == (M == N));
                return M == N;
            }
        }

        /// <summary>Матрица является столбцом</summary>
        public bool IsCol => !IsSquare && M == 1;

        /// <summary>Матрица является строкой</summary>
        public bool IsRow => !IsSquare && N == 1;

        /// <summary>Матрица является числом</summary>
        public bool IsDigit => N == 1 && M == 1;

        /// <summary>Транспонированная матрица</summary>
        public ExpressionMatrix T => GetTransponse();

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Матрица</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        [DST]
        public ExpressionMatrix(int N, int M)
        {
            Contract.Requires(N > 0);
            Contract.Requires(M > 0);
            Contract.Ensures(this.N == N);
            Contract.Ensures(this.M == M);

            _Data = new Expression[_N = N, _M = M];
            var zerro = 0d.ToExpression();
            for(var i = 0; i < N; i++)
                for(var j = 0; j < M; j++)
                    _Data[i, j] = zerro;
        }

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        [DST]
        public ExpressionMatrix(int N) : this(N, N) { }

        /// <summary>Метод определения значения элемента матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <param name="j">Номер столбца</param>
        /// <returns>Значение элемента матрицы M[<paramref name="i"/>, <paramref name="j"/>]</returns>
        public delegate Expression MatrixItemCreator(int i, int j);

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST]
        public ExpressionMatrix(int N, MatrixItemCreator CreateFunction) : this(N, N, CreateFunction) { }

        /// <summary>Матрица</summary>
        /// <param name="N">Число строк</param>
        /// <param name="M">Число столбцов</param>
        /// <param name="CreateFunction">Порождающая функция f(i,j) - i-строка, j-столбец</param>
        [DST]
        public ExpressionMatrix(int N, int M, MatrixItemCreator CreateFunction)
            : this(N, M)
        {
            Contract.Requires(CreateFunction != null);
            for(var i = 0; i < N; i++)
                for(var j = 0; j < M; j++)
                    _Data[i, j] = CreateFunction(i, j);
        }

        [DST]
        public ExpressionMatrix(Expression[,] Data)
            : this(Data.GetLength(0), Data.GetLength(1))
        {
            for(var i = 0; i < _N; i++)
                for(var j = 0; j < _M; j++)
                    _Data[i, j] = Data[i, j];
        }

        [DST]
        public ExpressionMatrix(IList<Expression> DataRow)
            : this(DataRow.Count, 1)
        {
            for(var i = 0; i < _N; i++)
                _Data[i, 0] = DataRow[i];
        }


        public ExpressionMatrix(IEnumerable<IEnumerable<Expression>> Items) : this(GetElements(Items)) { }
        private static Expression[,] GetElements(IEnumerable<IEnumerable<Expression>> Items)
        {
            Contract.Requires(Items != null);
            Contract.Ensures(Contract.Result<Expression[,]>() != null);
            Contract.Ensures(Contract.Result<Expression[,]>().GetLength(0) > 0);
            Contract.Ensures(Contract.Result<Expression[,]>().GetLength(1) > 0);
            var cols = Items.Select(col => col.ToListFast()).ToList();
            var cols_count = cols.Count;
            var rows_count = cols.Max(col => col.Count);
            var data = new Expression[rows_count, cols_count];
            for(var j = 0; j < cols_count; j++)
            {
                var col = cols[j];
                for(var i = 0; i < col.Count && i < rows_count; i++)
                    data[i, j] = col[i];
            }
            return data;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить столбец матрицы</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы номер j</returns>
        [DST]
        public ExpressionMatrix GetCol(int j)
        {
            Contract.Requires(j >= 0);
            Contract.Requires(j < M);
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            var lv_A = new ExpressionMatrix(N, 1);
            for(var i = 0; i < N; i++) lv_A[i, j] = this[i, j];
            return lv_A;
        }

        /// <summary>Получить строку матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <returns>Строка матрицы номер i</returns>
        [DST]
        public ExpressionMatrix GetRow(int i)
        {
            Contract.Requires(i >= 0);
            Contract.Requires(i < N);
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            var lv_A = new ExpressionMatrix(1, M);
            for(var j = 0; j < M; j++) lv_A[i, j] = this[i, j];
            return lv_A;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <returns>Триугольная матрица</returns>
        public ExpressionMatrix GetTriangle()
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            var result = Clone();
            var n = N;
            var m = M;
            var row = new Expression[m];
            for(var i0 = 0; i0 < n - 1; i0++)
            {
                var a = result[i0, i0]; //Захватываем первый элемент строки
                for(var j = i0; j < m; j++) //Нормируем строку по первому элементу
                    row[j] = result[i0, j].Divide(a);

                for(var i = i0 + 1; i < n; i++) //Для всех оставшихся строк:
                {
                    a = result[i, i0]; //Захватываем первый элемент строки
                    for(var j = i0; j < m; j++)
                        //Вычитаем рабочую строку, домноженную на первый элемент
                        result[i, j] = result[i, j].Subtract(a.MultiplyWithConversion(row[j]));
                }
            }
            return result;
        }

        /// <summary>Получить обратную матрицу</summary>
        /// <returns>Обратная матрица</returns>
        public ExpressionMatrix GetInverse()
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            if(!IsSquare)
                throw new InvalidOperationException("Обратная матрица существует только для квадратной матрицы");

            var result = GetTransvection(0);
            for(var i = 1; i < N; i++)
                result *= GetTransvection(i);
            return result;
        }

        /// <summary>Трансвекция матрицы</summary>
        /// <param name="col">Оборный столбец</param>
        /// <returns>Трансвекция матрицы А</returns>                    
        public ExpressionMatrix GetTransvection(int col)
        {
            if(!IsSquare)
                throw new InvalidOperationException("Трансквенция неквадратной матрицы невозможна");

            var u_matrix = GetUnitaryMatryx(_N);
            var a = _Data;
            var result = u_matrix._Data;
            for(var row = 0; row < _N; row++)
                result[row, col] = row == col
                    ? 1d.ToExpression().DivideWithConversion(a[row, col])
                    : a[row, col].Negate().DivideWithConversion(a[col, col]);
            return u_matrix;
        }

        /// <summary>Транспонирование матрицы</summary>
        /// <returns>Транспонированная матрица</returns>
        [DST]
        public ExpressionMatrix GetTransponse()
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            Contract.Ensures(Contract.Result<ExpressionMatrix>().M == M);
            Contract.Ensures(Contract.Result<ExpressionMatrix>().N == N);
            var Result = new ExpressionMatrix(M, N);

            for(var i = 0; i < N; i++)
                for(var j = 0; j < M; j++)
                    Result[j, i] = this[i, j];

            return Result;
        }

        /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
        public ExpressionMatrix GetAdjunct(int n, int m) =>
            ((n + m) % 2 == 0 ? 1d : -1d).ToExpression().MultiplyWithConversion(GetMinor(n, m).GetDeterminant());

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        public ExpressionMatrix GetMinor(int n, int m)
        {
            var minor = new ExpressionMatrix(N - 1, M - 1);

            var i0 = 0;
            for(var i = 0; i < N; i++)
                if(i != n)
                {
                    var j0 = 0;
                    for(var j = 0; j < _M; j++)
                        if(j != m)
                            minor[i0, j0++] = this[i, j];
                    i0++;
                }
            return minor;
        }

        /// <summary>Определитель матрицы</summary>
        public Expression GetDeterminant()
        {
            if(_N != _M)
                throw new InvalidOperationException("Нельзя найти определитель неквадратной матрицы!");

            var n = _N;
            switch(n)
            {
                case 1:
                    return this[0, 0];
                case 2:
                    return this[0, 0].MultiplyWithConversion(this[1, 1]).SubtractWithConversion(this[0, 1].MultiplyWithConversion(this[1, 0]));
            }

            var data = _Data.CloneArray();

            Expression det = null;
            var negate = false;
            for(var k = 0; k < n; k++) //Разложение по элементам первой строки
            {
                int i;
                int j;
                if((data[k, k] as ConstantExpression)?.Value?.Equals(0) == true) //!!!
                {
                    j = k;
                    while(j < n && (data[k, j] as ConstantExpression)?.Value?.Equals(0) == true) j++;

                    if((data[k, j] as ConstantExpression)?.Value?.Equals(0) == true)
                        return 0d.ToExpression();

                    for(i = k; i <= n; i++)
                    {
                        var d = data[i, j];
                        data[i, j] = data[i, k];
                        data[i, k] = d;
                    }
                    negate = !negate;
                }

                var doagonal_item = data[k, k];

                // Определитель- произведение элементов главной диагонали треугольной матрицы
                det = det == null
                    ? (negate ? doagonal_item.Negate() : doagonal_item)
                    : (negate
                        ? (Expression)det.MultiplyWithConversion(doagonal_item).Negate()
                        : det.MultiplyWithConversion(doagonal_item));

                if(k >= n) continue;

                // Приведение к нижне-треугольному виду
                var k1 = k + 1;
                for(i = k1; i < n; i++)
                    for(j = k1; j < n; j++)
                        data[i, j] = data[i, j].SubtractWithConversion(
                            data[i, k].MultiplyWithConversion(
                                data[k, j].DivideWithConversion(
                                    doagonal_item)));
            }

            return det.Simplify();
        }

        /* -------------------------------------------------------------------------------------------- */

        [DST]
        public override string ToString() => $"ExpressionMatrix[{N}x{M}]";

        /* -------------------------------------------------------------------------------------------- */

        #region ICloneable Members

        /// <summary>Клонирование матрицы</summary>
        /// <returns>Копия текущей матрицы</returns>
        [DST]
        public ExpressionMatrix Clone()
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            var result = new ExpressionMatrix(N, M);
            for(var i = 0; i < N; i++) for(var j = 0; j < M; j++) result[i, j] = this[i, j];
            return result;
        }

        /// <summary> Создает новый объект, который является копией текущего экземпляра</summary>
        /// <returns> Новый объект, являющийся копией этого экземпляра </returns>
        object ICloneable.Clone() => Clone();

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        public static bool operator ==(ExpressionMatrix A, ExpressionMatrix B) => ReferenceEquals(A, null) && ReferenceEquals(B, null) || !ReferenceEquals(A, null) && !ReferenceEquals(B, null) && A.Equals(B);

        public static bool operator !=(ExpressionMatrix A, ExpressionMatrix B) => !(A == B);

        [DST]
        public static ExpressionMatrix operator +(ExpressionMatrix M, Expression x)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => M[i, j].AddWithConversion(x));
        }

        [DST]
        public static ExpressionMatrix operator +(Expression x, ExpressionMatrix M)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => x.AddWithConversion(M[i, j]));
        }

        [DST]
        public static ExpressionMatrix operator -(ExpressionMatrix M, Expression x)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => M[i, j].Subtract(x));
        }

        [DST]
        public static ExpressionMatrix operator -(Expression x, ExpressionMatrix M)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => x.Subtract(M[i, j]));
        }

        [DST]
        public static ExpressionMatrix operator *(ExpressionMatrix M, Expression x)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => M[i, j].MultiplyWithConversion(x));
        }

        [DST]
        public static ExpressionMatrix operator *(Expression x, ExpressionMatrix M)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => x.MultiplyWithConversion(M[i, j]));
        }

        [DST]
        public static ExpressionMatrix operator *(Expression[,] A, ExpressionMatrix B) => (ExpressionMatrix)A * B;

        [DST]
        public static ExpressionMatrix operator *(Expression[] A, ExpressionMatrix B) => (ExpressionMatrix)A * B;

        [DST]
        public static ExpressionMatrix operator *(ExpressionMatrix A, Expression[] B) => A * (ExpressionMatrix)B;

        [DST]
        public static ExpressionMatrix operator *(ExpressionMatrix A, Expression[,] B) => A * (ExpressionMatrix)B;

        [DST]
        public static ExpressionMatrix operator /(ExpressionMatrix M, Expression x)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(M.N, M.M, (i, j) => M[i, j].Divide(x));
        }

        public static ExpressionMatrix operator /(Expression x, ExpressionMatrix M)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            M = M.GetInverse();
            return new ExpressionMatrix(M.N, M.M, (i, j) => x.MultiplyWithConversion(M[i, j]));
        }

        /// <summary>Оператор сложения двух матриц</summary>
        /// <param name="A">Первое слогаемое</param>
        /// <param name="B">Второе слогаемое</param>
        /// <returns>Сумма двух матриц</returns>
        [DST]
        public static ExpressionMatrix operator +(ExpressionMatrix A, ExpressionMatrix B)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            if(A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            return new ExpressionMatrix(A.N, A.M, (i, j) => A[i, j].AddWithConversion(B[i, j]));
        }

        /// <summary>Оператор разности двух матриц</summary>
        /// <param name="A">Уменьшаемое</param>
        /// <param name="B">Вычитаемое</param>
        /// <returns>Разность двух матриц</returns>
        [DST]
        public static ExpressionMatrix operator -(ExpressionMatrix A, ExpressionMatrix B)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            if(A.N != B.N || A.M != B.M)
                throw new ArgumentOutOfRangeException(nameof(B), "Размеры матриц не равны.");

            return new ExpressionMatrix(A.N, A.M, (i, j) => A[i, j].Subtract(B[i, j]));
        }

        /// <summary>Оператор произведения двух матриц</summary>
        /// <param name="A">Первый сомножитель</param>
        /// <param name="B">Второй сомножитель</param>
        /// <returns>Произведение двух матриц</returns>
        [DST]
        public static ExpressionMatrix operator *(ExpressionMatrix A, ExpressionMatrix B)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            if(A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var result = new ExpressionMatrix(A.N, B.M);
            var data = result._Data;

            for(var i = 0; i < result.N; i++)
                for(var j = 0; j < result.M; j++)
                    for(var k = 0; k < A.M; k++)
                        data[i, j] = result[i, j].AddWithConversion(A[i, k].MultiplyWithConversion(B[k, j]));

            return result;
        }

        /// <summary>Оператор деления двух матриц</summary>
        /// <param name="A">Делимое</param>
        /// <param name="B">Делитель</param>
        /// <returns>Частное двух матриц</returns>
        public static ExpressionMatrix operator /(ExpressionMatrix A, ExpressionMatrix B)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            B = B.GetInverse();
            if(A.M != B.N)
                throw new ArgumentOutOfRangeException(nameof(B), "Матрицы несогласованных порядков.");

            var result = new ExpressionMatrix(A.N, B.M);
            var data = result._Data;

            for(var i = 0; i < result.N; i++)
                for(var j = 0; j < result.M; j++)
                    for(var k = 0; k < A.M; k++)
                        data[i, j] = result[i, j].AddWithConversion(A[i, k].MultiplyWithConversion(B[k, j]));

            return result;
        }

        /// <summary>Конкатинация двух матриц (либо по строкам, либо по столбцам)</summary>
        /// <param name="A">Первое слогаемое</param>
        /// <param name="B">Второе слогаемое</param>
        /// <returns>Объединённая матрица</returns>
        public static ExpressionMatrix operator |(ExpressionMatrix A, ExpressionMatrix B)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            ExpressionMatrix result;
            if(A.M == B.M) // Конкатинация по строкам
            {
                result = new ExpressionMatrix(A.N + B.N, A.M);
                var data = result._Data;

                for(var i = 0; i < A.N; i++)
                    for(var j = 0; j < A.M; j++)
                        data[i, j] = A[i, j];
                var i0 = A.N;
                for(var i = 0; i < B.N; i++)
                    for(var j = 0; j < B.M; j++)
                        data[i + i0, j] = B[i, j];

            }
            else if(A.N == B.N) //Конкатинация по строкам
            {
                result = new ExpressionMatrix(A.N, A.M + B.M);
                var data = result._Data;

                for(var i = 0; i < A.N; i++)
                    for(var j = 0; j < A.M; j++)
                        data[i, j] = A[i, j];
                var j0 = A.M;
                for(var i = 0; i < B.N; i++)
                    for(var j = 0; j < B.M; j++)
                        data[i, j + j0] = B[i, j];
            }
            else
                throw new InvalidOperationException("Конкатинация возможна только по строкам, или по столбцам");

            return result;
        }



        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Оператор неявного преведения типа вещественного числа двойной точнойсти к типу Матрица порядка 1х1</summary>
        /// <param name="X">Приводимое число</param><returns>Матрица порадка 1х1</returns>
        [DST]
        public static implicit operator ExpressionMatrix(Expression X)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            Contract.Ensures(Contract.Result<ExpressionMatrix>().N == 1);
            Contract.Ensures(Contract.Result<ExpressionMatrix>().M == 1);
            return new ExpressionMatrix(1, 1) { [0, 0] = X };
        }

        [DST]
        public static explicit operator Expression[,] (ExpressionMatrix M)
        {
            Contract.Ensures(Contract.Result<Expression[,]>() != null);
            return (Expression[,])M._Data.Clone();
        }

        [DST]
        public static explicit operator ExpressionMatrix(Expression[,] Data)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(Data);
        }

        [DST]
        public static explicit operator ExpressionMatrix(Expression[] Data)
        {
            Contract.Ensures(Contract.Result<ExpressionMatrix>() != null);
            return new ExpressionMatrix(Data);
        }

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable<ExpressionMatrix> Members

        public bool Equals(ExpressionMatrix other) => !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || other._N == _N && other._M == _M && Equals(other._Data, _Data));

        private static bool Equals(Expression[,] E1, Expression[,] E2)
        {
            if(ReferenceEquals(E1, E2)) return true;
            var N1 = E1.GetLength(0);
            var M1 = E1.GetLength(1);
            var N2 = E2.GetLength(0);
            var M2 = E2.GetLength(1);
            if(N1 != N2 || M1 != M2) return false;
            for(var i = 0; i < N1; i++)
                for(var j = 0; j < M1; j++)
                    if(E1[i, j] != E2[i, j]) return false;
            return true;
        }

        [DST]
        bool IEquatable<ExpressionMatrix>.Equals(ExpressionMatrix other) => Equals(other);

        #endregion

        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || Equals(obj as ExpressionMatrix));

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
    }
}

