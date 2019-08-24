using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using static MathCore.MatrixFloat.Array.Operator;
// ReSharper disable ExceptionNotThrown
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable LocalizableElement
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    using DST = DebuggerStepThroughAttribute;

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
    public class MatrixFloat : ICloneable<MatrixFloat>, ICloneable<float[,]>, IFormattable,
        IEquatable<MatrixFloat>, IEquatable<float[,]>, IIndexable<int, int, float>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Создать матрицу-столбец</summary><param name="data">Элементы столбца</param><returns>Матрица-столбец</returns>
        /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
        /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
        [NotNull] public static MatrixFloat CreateCol([NotNull] params float[] data) => new MatrixFloat(Array.CreateColArray(data));

        /// <summary>Создать матрицу-строку</summary><param name="data">Элементы строки</param><returns>Матрица-строка</returns>
        /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
        /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
        [NotNull] public static MatrixFloat CreateRow([NotNull] params float[] data) => new MatrixFloat(Array.CreateRowArray(data));

        /// <summary>Создать диагональную матрицу</summary><param name="elements">Элементы диагональной матрицы</param>
        /// <returns>Диагональная матрица</returns>
        [NotNull] public static MatrixFloat CreateDiagonalMatrixFloat([NotNull] params float[] elements) => new MatrixFloat(Array.CreateDiagonal(elements));

        /// <summary>Операции над двумерными массивами</summary>
        public static partial class Array
        {
            /// <summary>Операторы над двумерными массивами</summary>
            public static partial class Operator { }
        }

        /// <summary>Получить единичную матрицу размерности NxN</summary>
        /// <param name="N">Размерность матрицы</param><returns>Единичная матрица размерности NxN с 1 на главной диагонали</returns>
        [DST]
        public static MatrixFloat GetUnitaryMatryx(int N) => new MatrixFloat(Array.GetUnitaryArrayMatrixFloat(N));

        /// <summary>Трансвекция матрицы</summary><param name="A">Трансвецируемая матрица</param><param name="j">Оборный столбец</param>
        /// <returns>Трансвекция матрицы А</returns>                    
        public static MatrixFloat GetTransvection(MatrixFloat A, int j) => new MatrixFloat(Array.GetTransvection(A._Data, j));

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        private readonly int _N;

        /// <summary>Число столбцов матрицы</summary>
        private readonly int _M;

        /// <summary>Элементы матрицы</summary>
        [NotNull] private readonly float[,] _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Число строк матрицы</summary>
        public int N => _N;

        /// <summary>Число столбцов матрицы</summary>
        public int M => _M;

        /// <summary>Элемент матрицы</summary>
        /// <param name="i">Номер строки (элемента в столбце)</param>
        /// <param name="j">Номер столбца (элемента в строке)</param>
        /// <returns>Элемент матрицы</returns>
        public float this[int i, int j] { [DST] get => _Data[i, j]; [DST] set => _Data[i, j] = value; }

        /// <summary>Вектор-стольбец</summary><param name="j">Номер столбца</param><returns>Столбец матрицы</returns>
        [NotNull] public MatrixFloat this[int j] => GetCol(j);

        /// <summary>Матрица является квадратной матрицей</summary>
        public bool IsSquare => _M == _N;

        /// <summary>Матрица является столбцом</summary>
        public bool IsCol => _M == 1;

        /// <summary>Матрица является строкой</summary>
        public bool IsRow => _N == 1;

        /// <summary>Матрица является числом</summary>
        public bool IsScalar => _N == 1 && _M == 1;

        /// <summary>Транспонированная матрица</summary>
        public MatrixFloat T => GetTransponse();

        /// <summary>Максимум среди абсолютных сумм элементов строк</summary>
        public float Norm_m => Array.GetMaxRowAbsSumm(_Data);

        /// <summary>Максимум среди абсолютных сумм элементов столбцов</summary>
        public float Norm_l => Array.GetMaxColAbsSumm(_Data);

        /// <summary>Среднеквадратическое значение элементов матрицы</summary>
        public float Norm_k => Array.GetRMS(_Data);

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="N"/> &lt; 0 || <paramref name="M"/> &lt; 0</exception>
        [DST]
        public MatrixFloat(int N, int M)
        {
            if (N <= 0) throw new ArgumentOutOfRangeException(nameof(N), N, "N должна быть больше 0");
            if (M <= 0) throw new ArgumentOutOfRangeException(nameof(M), M, "M должна быть больше 0");
            Contract.EndContractBlock();

            _Data = new float[_N = N, _M = M];
        }

        /// <summary>Квадратная матрица</summary><param name="N">Размерность</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="N" /> &lt; 0</exception>
        [DST] public MatrixFloat(int N) : this(N, N) { }

        /// <summary>Метод определения значения элемента матрицы</summary>
        /// <param name="i">Номер строки</param><param name="j">Номер столбца</param>
        /// <returns>Значение элемента матрицы M[<paramref name="i"/>, <paramref name="j"/>]</returns>
        public delegate float MatrixFloatItemCreator(int i, int j);

        /// <summary>Квадратная матрица</summary>
        /// <param name="N">Размерность</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST] public MatrixFloat(int N, [NotNull] MatrixFloatItemCreator CreateFunction) : this(N, N, CreateFunction) { }

        /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
        /// <param name="CreateFunction">Порождающая функция</param>
        [DST]
        public MatrixFloat(int N, int M, [NotNull] MatrixFloatItemCreator CreateFunction) : this(N, M)
        {
            Contract.Requires(N > 0);
            Contract.Requires(M > 0);
            Contract.Requires(CreateFunction != null);
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) _Data[i, j] = CreateFunction(i, j);
        }

        /// <summary>Инициализация новой матрицы по двумерному массиву её элементов</summary>
        /// <param name="Data">Двумерный массив элементов матрицы</param>
        /// <param name="clone">Создать копию данных</param>
        [DST]
        public MatrixFloat([NotNull] float[,] Data, bool clone = false)
        {
            Contract.Requires(Data != null);
            _N = Data.GetLength(0);
            _M = Data.GetLength(1);
            _Data = clone ? Data.CloneObject() : Data;
        }

        /// <summary>Инициализация новой матрицы - столбца/строки</summary>
        /// <param name="DataCol">Элементы столбца матрицы</param>
        /// <param name="IsColumn">Создаётся матрица-столбец</param>
        [DST]
        public MatrixFloat([NotNull] IList<float> DataCol, bool IsColumn = true) : this(IsColumn ? DataCol.Count : 1, IsColumn ? 1 : DataCol.Count)
        {
            Contract.Requires(DataCol != null);
            if (IsColumn) for (var i = 0; i < _N; i++) _Data[i, 0] = DataCol[i];
            else for (var j = 0; j < _M; j++) _Data[0, j] = DataCol[j];
        }

        /// <summary>Инициализация новой матрицы на основе перечисления строк (перечисления элементов строк) </summary>
        /// <param name="Items">Перечисление строк, состоящих из перечисления эламентов строк</param>
        public MatrixFloat([NotNull] IEnumerable<IEnumerable<float>> Items) : this(GetElements(Items)) { }

        /// <summary>Получить двумерный массив элементов матрицы</summary>
        /// <param name="ColsItems">Перечисление элементов (по столбцам)</param>
        /// <returns>Двумерный массив элементов матрицы</returns>
        [DST, NotNull]
        private static float[,] GetElements([NotNull] IEnumerable<IEnumerable<float>> ColsItems)
        {
            Contract.Requires(ColsItems != null);
            var cols = ColsItems.Select(col => col.ToListFast()).ToList();
            var cols_count = cols.Count;
            var rows_count = cols.Max(col => col.Count);
            var data = new float[rows_count, cols_count];
            for (var j = 0; j < cols_count; j++)
            {
                var col = cols[j];
                for (var i = 0; i < col.Count && i < rows_count; i++) data[i, j] = col[i];
            }
            return data;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить столбец матрицы</summary>
        /// <param name="j">Номер столбца</param>
        /// <returns>Столбец матрицы номер j</returns>
        [DST, NotNull] public MatrixFloat GetCol(int j) => new MatrixFloat(Array.GetCol(_Data, j));

        /// <summary>Получить строку матрицы</summary>
        /// <param name="i">Номер строки</param>
        /// <returns>Строка матрицы номер i</returns>
        [DST, NotNull] public MatrixFloat GetRow(int i) => new MatrixFloat(Array.GetRow(_Data, i));

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <param name="P">Матрица перестановок</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="D">Определитель</param>
        /// <returns>Триугольная матрица</returns>
        [NotNull]
        public MatrixFloat GetTriangle([NotNull] out MatrixFloat P, out int rank, out float D)
        {
            var result = new MatrixFloat(Array.GetTriangle(_Data, out var p, out rank, out D));
            P = new MatrixFloat(p);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <param name="B">Присоединённая матрица правой части СЛАУ</param>
        /// <param name="CloneB">Работать с клоном матрицы <paramref name="B"/></param>
        /// <returns>Триугольная матрица</returns>
        /// <exception cref="ArgumentNullException">Если <paramref name="B"/> <see langword="null"/></exception>
        [NotNull]
        public MatrixFloat GetTriangle([NotNull] ref MatrixFloat B, bool CloneB = true)
        {
            var b = CloneB ? B._Data.CloneObject() : B._Data;
            var result = new MatrixFloat(Array.GetTriangle(_Data, b, out var _, out var _));
            if (CloneB) B = new MatrixFloat(b);
            return result;
        }

        /// <summary>Приведение матрицы к ступенчатому виду методом гауса</summary>
        /// <param name="B">Матрица правой части СЛАУ</param>
        /// <param name="P">Матрица перестановок</param>
        /// <param name="rank">Ранг матрицы</param>
        /// <param name="d">Определитель матрицы</param>
        /// <param name="CloneB">Клонировать матрицу правой части</param>
        /// <returns>Треугольная матрица</returns>
        [NotNull]
        public MatrixFloat GetTriangle([NotNull] ref MatrixFloat B, [NotNull] out MatrixFloat P, out int rank, out float d, bool CloneB = true)
        {
            var b = B._Data;
            var result = new MatrixFloat(Array.GetTriangle(_Data, ref b, out var p, out rank, out d, CloneB));
            P = new MatrixFloat(p);
            if (CloneB) B = new MatrixFloat(b);
            return result;
        }

        /// <summary>Получить обратную матрицу</summary>                                                     
        /// <param name="P">Матрица перестановок</param>
        /// <returns>Обратная матрица</returns>
        [NotNull]
        public MatrixFloat GetInverse(out MatrixFloat P)
        {
            var inverse = new MatrixFloat(Array.Inverse(_Data, out var p));
            P = new MatrixFloat(p);
            return inverse;
        }

        /// <summary>Транспонирование матрицы</summary>
        /// <returns>Транспонированная матрица</returns>
        [DST, NotNull] public MatrixFloat GetTransponse() => new MatrixFloat(Array.Transponse(_Data));

        /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
        public float GetAdjunct(int n, int m) => Array.GetAdjunct(_Data, n, m);

        /// <summary>Минор матрицы по определённому элементу</summary>
        /// <param name="n">Номер столбца</param>
        /// <param name="m">Номер строки</param>
        /// <returns>Минор элемента матрицы [n,m]</returns>
        [NotNull] public MatrixFloat GetMinor(int n, int m) => new MatrixFloat(Array.GetMinor(_Data, n, m));

        /// <summary>Определитель матрицы</summary>
        public float GetDeterminant() => Array.GetDeterminant(_Data);

        /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
        /// <param name="L">Нижне-треугольная матрица</param>
        /// <param name="U">Верхнетреугольная матрица</param>
        /// <param name="P">Матрица преобразований P*X = L*U</param>
        /// <param name="D">Знак определителя</param>
        /// <returns>Истина, если разложение выполнено успешно, ложь - если матрица вырожденная</returns>
        public bool GetLUDecomposition([CanBeNull] out MatrixFloat L, [CanBeNull] out MatrixFloat U, [CanBeNull] out MatrixFloat P, out float D)
        {
            if (!IsSquare) throw new InvalidOperationException("Невозможно осуществить LU-разложение неквадратной метрицы");

            var decomposition_success = Array.GetLUPDecomposition(_Data, out var l, out var u, out var p, out var d);
            L = decomposition_success ? new MatrixFloat(l) : null;
            U = decomposition_success ? new MatrixFloat(u) : null;
            P = decomposition_success ? new MatrixFloat(p) : null;
            D = decomposition_success ? d : 0;
            return decomposition_success;
        }

        /// <summary>Получить внутренний массив элементов матрицы</summary>
        /// <returns></returns>
        [DST, NotNull] public float[,] GetData() => _Data;

        /* -------------------------------------------------------------------------------------------- */

        /// <inheritdoc/>
        [DST] public override string ToString() => $"MatrixFloat[{_N}x{_M}]";

        /// <summary>Преобразование матрицы в строку с форматированием</summary>
        /// <param name="Format">Строка формата вывода чисел</param>
        /// <param name="Splitter">Разделитель элементов матрицы</param>
        /// <param name="provider">Механизм форматирования чисел матрицы</param>
        /// <returns>Строковое представление матрицы</returns>
        [DST, NotNull]
        public string ToStringFormat
        (
            [NotNull] string Format = "r",
            [CanBeNull] string Splitter = "\t",
            [CanBeNull] IFormatProvider provider = null
        ) => _Data.ToStringFormatView(Format, Splitter, provider) ?? throw new InvalidOperationException();

        /// <inheritdoc/>
        [DST] public string ToString([NotNull] string format, [CanBeNull] IFormatProvider provider) => _Data.ToStringFormatView(format, "\t", provider) ?? throw new InvalidOperationException();

        /* -------------------------------------------------------------------------------------------- */

        #region ICloneable Members

        /// <inheritdoc/>
        [DST] object ICloneable.Clone() => Clone();

        /// <inheritdoc/>
        [DST, NotNull] float[,] ICloneable<float[,]>.Clone() => _Data.CloneObject();

        /// <inheritdoc/>
        [DST, NotNull] public MatrixFloat Clone() => new MatrixFloat(_Data, true);

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        [DST] public static bool operator ==([CanBeNull] MatrixFloat A, [CanBeNull] MatrixFloat B) => ReferenceEquals(A, null) && ReferenceEquals(B, null) || !ReferenceEquals(A, null) && !ReferenceEquals(B, null) && A.Equals(B);

        [DST] public static bool operator !=([CanBeNull] MatrixFloat A, [CanBeNull] MatrixFloat B) => !(A == B);

        [DST] public static bool operator ==([CanBeNull] float[,] A, [CanBeNull] MatrixFloat B) => B == A;

        [DST] public static bool operator ==([CanBeNull] MatrixFloat A, [CanBeNull] float[,] B) => ReferenceEquals(A, null) && ReferenceEquals(B, null) || !ReferenceEquals(A, null) && !ReferenceEquals(B, null) && A.Equals(B);

        [DST] public static bool operator !=([CanBeNull] float[,] A, [CanBeNull] MatrixFloat B) => !(A == B);

        [DST] public static bool operator !=([CanBeNull] MatrixFloat A, [CanBeNull] float[,] B) => !(A == B);

        [DST, NotNull] public static MatrixFloat operator +([NotNull] MatrixFloat M, float x) => new MatrixFloat(Add(M._Data, x));

        [DST, NotNull] public static MatrixFloat operator +(float x, [NotNull] MatrixFloat M) => new MatrixFloat(Add(M._Data, x));

        [DST, NotNull] public static MatrixFloat operator -([NotNull] MatrixFloat M, float x) => new MatrixFloat(Substract(M._Data, x));

        [DST, NotNull] public static MatrixFloat operator -([NotNull] MatrixFloat M) => new MatrixFloat(new float[M._N, M._M].Initialize(M._Data, (i, j, data) => -data[i, j]));

        [DST, NotNull] public static MatrixFloat operator -(float x, [NotNull] MatrixFloat M) => new MatrixFloat(Substract(x, M._Data));

        [DST, NotNull] public static MatrixFloat operator *([NotNull] MatrixFloat M, float x) => new MatrixFloat(Multiply(M._Data, x));

        [DST, NotNull] public static MatrixFloat operator *(float x, [NotNull] MatrixFloat M) => new MatrixFloat(Multiply(M._Data, x));

        [DST, NotNull] public static MatrixFloat operator *([NotNull] float[,] A, [NotNull] MatrixFloat B) => new MatrixFloat(Multiply(A, B._Data));

        [DST, NotNull] public static MatrixFloat operator *([NotNull] float[] A, [NotNull] MatrixFloat B) => new MatrixFloat(Multiply(Array.CreateColArray(A), B._Data));

        [DST, NotNull] public static MatrixFloat operator *([NotNull] MatrixFloat A, [NotNull] float[] B) => new MatrixFloat(Multiply(A._Data, Array.CreateColArray(B)));

        [DST, NotNull] public static MatrixFloat operator *([NotNull] MatrixFloat A, [NotNull] float[,] B) => new MatrixFloat(Multiply(A._Data, B));

        [DST, NotNull] public static MatrixFloat operator /([NotNull] MatrixFloat M, float x) => new MatrixFloat(Divade(M._Data, x));

        [DST, NotNull] public static MatrixFloat operator /(float x, [NotNull] MatrixFloat M) => new MatrixFloat(Divade(x, M._Data));

        [DST, NotNull]
        public static MatrixFloat operator ^([NotNull] MatrixFloat M, int n)
        {
            if (!M.IsSquare) throw new ArgumentException("Матрица не квадратная", nameof(M));
            switch (n)
            {
                case 1: return M.Clone();
                case -1: return M.GetInverse(out _);
                default:
                    var m = M._Data;
                    if (n < 0)
                    {
                        m = Array.Inverse(m, out _);
                        n = -n;
                    }
                    var result = Array.GetUnitaryArrayMatrixFloat(M._N);
                    for (var i = 0; i < n; i++) result = Multiply(result, m);
                    return new MatrixFloat(result);
            }
        }

        /// <summary>Оператор сложения двух матриц</summary>
        /// <param name="A">Первое слогаемое</param><param name="B">Второе слогаемое</param><returns>Сумма двух матриц</returns>
        [DST, NotNull] public static MatrixFloat operator +([NotNull] MatrixFloat A, [NotNull] MatrixFloat B) => new MatrixFloat(Add(A._Data, B._Data));

        /// <summary>Оператор разности двух матриц</summary>
        /// <param name="A">Уменьшаемое</param><param name="B">Вычитаемое</param><returns>Разность двух матриц</returns>
        [DST, NotNull] public static MatrixFloat operator -([NotNull] MatrixFloat A, [NotNull] MatrixFloat B) => new MatrixFloat(Substract(A._Data, B._Data));

        /// <summary>Оператор произведения двух матриц</summary>
        /// <param name="A">Первый сомножитель</param><param name="B">Второй сомножитель</param><returns>Произведение двух матриц</returns>
        [DST, NotNull] public static MatrixFloat operator *([NotNull] MatrixFloat A, [NotNull] MatrixFloat B) => new MatrixFloat(Multiply(A._Data, B._Data));

        /// <summary>Оператор деления двух матриц</summary>
        /// <param name="A">Делимое</param><param name="B">Делитель</param><returns>Частное двух матриц</returns>
        [DST, NotNull] public static MatrixFloat operator /([NotNull] MatrixFloat A, [NotNull] MatrixFloat B) => new MatrixFloat(Divade(A._Data, B._Data));

        /// <summary>Конкатинация двух матриц (либо по строкам, либо по столбцам)</summary>
        /// <param name="A">Первое слогаемое</param><param name="B">Второе слогаемое</param><returns>Объединённая матрица</returns>
        [DST, NotNull] public static MatrixFloat operator |([NotNull] MatrixFloat A, [NotNull] MatrixFloat B) => new MatrixFloat(Concatinate(A._Data, B._Data));

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Оператор неявного преведения типа вещественного числа двойной точнойсти к типу Матрица порядка 1х1</summary>
        /// <param name="X">Приводимое число</param><returns>Матрица порадка 1х1</returns>
        [DST, NotNull] public static implicit operator MatrixFloat(float X) => new MatrixFloat(1, 1) { [0, 0] = X };

        [DST, NotNull] public static explicit operator float[,] ([NotNull] MatrixFloat M) => M._Data;

        [DST, NotNull] public static explicit operator MatrixFloat([NotNull] float[,] Data) => new MatrixFloat(Data);

        [DST, NotNull] public static explicit operator MatrixFloat([NotNull] float[] Data) => new MatrixFloat(Data);

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable Members

        /// <inheritdoc/>
        [DST] public bool Equals(float[,] other) => !ReferenceEquals(null, other) && Array.AreEquals(_Data, other);

        /// <inheritdoc/>
        [DST] public bool Equals(MatrixFloat other) => !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Array.AreEquals(_Data, other._Data));

        #endregion

        /// <inheritdoc/>
        [DST] public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || Equals(obj as MatrixFloat) || Equals(obj as float[,]));

        /// <inheritdoc/>
        [DST]
        public override int GetHashCode()
        {
            unchecked
            {
                var result = (_N * 397) ^ _M;
                for (var i = 0; i < _N; i++)
                    for (var j = 0; j < _M; j++)
                        result = (result * 397) ^ i ^ j ^ _Data[i, j].GetHashCode();
                return result;
            }
        }

        /* -------------------------------------------------------------------------------------------- */
    }
}