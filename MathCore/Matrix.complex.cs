#nullable enable
using static MathCore.MatrixComplex.Array.Operator;

// ReSharper disable ExceptionNotThrown
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable LocalizableElement
// ReSharper disable UnusedMember.Global

namespace MathCore;

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
public partial class MatrixComplex : ICloneable<MatrixComplex>, ICloneable<Complex[,]>, IFormattable,
                                     IEquatable<MatrixComplex>, IEquatable<Complex[,]>
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Создать матрицу-столбец</summary><param name="data">Элементы столбца</param><returns>Матрица-столбец</returns>
    /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
    /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
    public static MatrixComplex CreateCol(params Complex[] data) => new(Array.CreateColArray(data));

    /// <summary>Создать матрицу-строку</summary><param name="data">Элементы строки</param><returns>Матрица-строка</returns>
    /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
    /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
    public static MatrixComplex CreateRow(params Complex[] data) => new(Array.CreateRowArray(data));

    /// <summary>Создать диагональную матрицу</summary><param name="elements">Элементы диагональной матрицы</param>
    /// <returns>Диагональная матрица</returns>
    public static MatrixComplex CreateDiagonalMatrixComplex(params Complex[] elements) => new(Array.CreateDiagonal(elements));

    /// <summary>Операции над двумерными массивами</summary>
    public static partial class Array
    {
        /// <summary>Операторы над двумерными массивами</summary>
        public static partial class Operator;
    }

    /// <summary>Получить единичную матрицу размерности NxN</summary>
    /// <param name="N">Размерность матрицы</param><returns>Единичная матрица размерности NxN с 1 на главной диагонали</returns>
    [DST]
    public static MatrixComplex GetUnitaryMatrix(int N) => new(Array.GetUnitaryArrayMatrix(N));

    /// <summary>Трансвекция матрицы</summary><param name="A">Трансвецируемая матрица</param><param name="j">Опорный столбец</param>
    /// <returns>Трансвекция матрицы А</returns>
    public static MatrixComplex GetTransvection(MatrixComplex A, int j) => new(Array.GetTransvection(A._Data, j));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Число строк матрицы</summary>
    private readonly int _N;

    /// <summary>Число столбцов матрицы</summary>
    private readonly int _M;

    /// <summary>Элементы матрицы</summary>
    private readonly Complex[,] _Data;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Число строк матрицы</summary>
    public int N => _N;

    /// <summary>Число столбцов матрицы</summary>
    public int M => _M;

    /// <summary>Элемент матрицы</summary>
    /// <param name="i">Номер строки (элемента в столбце)</param>
    /// <param name="j">Номер столбца (элемента в строке)</param>
    /// <returns>Элемент матрицы</returns>
    public ref Complex this[int i, int j] { [DST] get => ref _Data[i, j]; }

    /// <summary>Вектор-столбец</summary><param name="j">Номер столбца</param><returns>Столбец матрицы</returns>
    public MatrixComplex this[int j] => GetCol(j);

    /// <summary>Матрица является квадратной матрицей</summary>
    public bool IsSquare => _M == _N;

    /// <summary>Матрица является столбцом</summary>
    public bool IsCol => _M == 1;

    /// <summary>Матрица является строкой</summary>
    public bool IsRow => _N == 1;

    /// <summary>Матрица является числом</summary>
    public bool IsScalar => _N == 1 && _M == 1;

    /// <summary>Транспонированная матрица</summary>
    public MatrixComplex T => GetTranspose();

    /// <summary>Максимум среди абсолютных сумм элементов строк</summary>
    public Complex Norm_m => Array.GetMaxRowAbsSum(_Data);

    /// <summary>Максимум среди абсолютных сумм элементов столбцов</summary>
    public Complex Norm_l => Array.GetMaxColAbsSum(_Data);

    /// <summary>Среднеквадратичное значение элементов матрицы</summary>
    public Complex Norm_k => Array.GetRMS(_Data);

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="N"/> &lt; 0 || <paramref name="M"/> &lt; 0</exception>
    [DST]
    public MatrixComplex(int N, int M)
    {
        if (N <= 0) throw new ArgumentOutOfRangeException(nameof(N), N, "N должна быть больше 0");
        if (M <= 0) throw new ArgumentOutOfRangeException(nameof(M), M, "M должна быть больше 0");

        _Data = new Complex[_N = N, _M = M];
    }

    /// <summary>Квадратная матрица</summary><param name="N">Размерность</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="N" /> &lt; 0</exception>
    [DST] public MatrixComplex(int N) : this(N, N) { }

    /// <summary>Метод определения значения элемента матрицы</summary>
    /// <param name="i">Номер строки</param><param name="j">Номер столбца</param>
    /// <returns>Значение элемента матрицы M[<paramref name="i"/>, <paramref name="j"/>]</returns>
    public delegate Complex MatrixComplexItemCreator(int i, int j);

    /// <summary>Квадратная матрица</summary>
    /// <param name="N">Размерность</param>
    /// <param name="CreateFunction">Порождающая функция</param>
    [DST] public MatrixComplex(int N, MatrixComplexItemCreator CreateFunction) : this(N, N, CreateFunction) { }

    /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
    /// <param name="CreateFunction">Порождающая функция</param>
    [DST]
    public MatrixComplex(int N, int M, MatrixComplexItemCreator CreateFunction) : this(N, M)
    {
        for (var i = 0; i < N; i++)
            for (var j = 0; j < M; j++)
                _Data[i, j] = CreateFunction(i, j);
    }

    /// <summary>Инициализация новой матрицы по двумерному массиву её элементов</summary>
    /// <param name="Data">Двумерный массив элементов матрицы</param>
    /// <param name="clone">Создать копию данных</param>
    [DST]
    public MatrixComplex(Complex[,] Data, bool clone = false)
    {
        _N    = Data.GetLength(0);
        _M    = Data.GetLength(1);
        _Data = clone ? Data.CloneObject() : Data;
    }

    /// <summary>Инициализация новой матрицы - столбца/строки</summary>
    /// <param name="DataCol">Элементы столбца матрицы</param>
    /// <param name="IsColumn">Создаётся матрица-столбец</param>
    [DST]
    public MatrixComplex(IList<Complex> DataCol, bool IsColumn = true) : this(IsColumn ? DataCol.Count : 1, IsColumn ? 1 : DataCol.Count)
    {
        if (IsColumn) for (var i = 0; i < _N; i++) _Data[i, 0] = DataCol[i];
        else for (var j = 0; j < _M; j++) _Data[0, j]          = DataCol[j];
    }

    /// <summary>Инициализация новой матрицы на основе перечисления строк (перечисления элементов строк) </summary>
    /// <param name="Items">Перечисление строк, состоящих из перечисления элементов строк</param>
    public MatrixComplex(IEnumerable<IEnumerable<Complex>> Items) : this(GetElements(Items)) { }

    /// <summary>Получить двумерный массив элементов матрицы</summary>
    /// <param name="ColsItems">Перечисление элементов (по столбцам)</param>
    /// <returns>Двумерный массив элементов матрицы</returns>
    [DST]
    private static Complex[,] GetElements(IEnumerable<IEnumerable<Complex>> ColsItems)
    {
        var cols       = ColsItems.Select(col => col.ToListFast()).ToList();
        var cols_count = cols.Count;
        var rows_count = cols.Max(col => col.Count);
        var data       = new Complex[rows_count, cols_count];
        for (var j = 0; j < cols_count; j++)
        {
            var col                                                          = cols[j];
            for (var i = 0; i < col.Count && i < rows_count; i++) data[i, j] = col[i];
        }
        return data;
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Получить столбец матрицы</summary>
    /// <param name="j">Номер столбца</param>
    /// <returns>Столбец матрицы номер j</returns>
    [DST] public MatrixComplex GetCol(int j) => new(Array.GetCol(_Data, j));

    /// <summary>Получить строку матрицы</summary>
    /// <param name="i">Номер строки</param>
    /// <returns>Строка матрицы номер i</returns>
    [DST] public MatrixComplex GetRow(int i) => new(Array.GetRow(_Data, i));

    /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
    /// <param name="P">Матрица перестановок</param>
    /// <param name="rank">Ранг матрицы</param>
    /// <param name="D">Определитель</param>
    /// <returns>Треугольная матрица</returns>
    public MatrixComplex GetTriangle(out MatrixComplex P, out int rank, out Complex D)
    {
        var result = new MatrixComplex(Array.GetTriangle(_Data, out var p, out rank, out D));
        P = new(p);
        return result;
    }

    /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
    /// <param name="B">Присоединённая матрица правой части СЛАУ</param>
    /// <param name="CloneB">Работать с клоном матрицы <paramref name="B"/></param>
    /// <returns>Треугольная матрица</returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="B"/> <see langword="null"/></exception>
    public MatrixComplex GetTriangle(ref MatrixComplex B, bool CloneB = true)
    {
        var b         = CloneB ? B._Data.CloneObject() : B._Data;
        var result    = new MatrixComplex(Array.GetTriangle(_Data, b, out _, out _));
        if (CloneB) B = new(b);
        return result;
    }

    /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
    /// <param name="B">Матрица правой части СЛАУ</param>
    /// <param name="P">Матрица перестановок</param>
    /// <param name="rank">Ранг матрицы</param>
    /// <param name="d">Определитель матрицы</param>
    /// <param name="CloneB">Клонировать матрицу правой части</param>
    /// <returns>Треугольная матрица</returns>
    public MatrixComplex GetTriangle(ref MatrixComplex B, out MatrixComplex P, out int rank, out Complex d, bool CloneB = true)
    {
        var b      = B._Data;
        var result = new MatrixComplex(Array.GetTriangle(_Data, ref b, out var p, out rank, out d, CloneB));
        P = new(p);
        if (CloneB) B = new(b);
        return result;
    }

    /// <summary>Получить обратную матрицу</summary>                                                     
    /// <param name="P">Матрица перестановок</param>
    /// <returns>Обратная матрица</returns>
    public MatrixComplex GetInverse(out MatrixComplex P)
    {
        var inverse = new MatrixComplex(Array.Inverse(_Data, out var p));
        P = new(p);
        return inverse;
    }

    /// <summary>Транспонирование матрицы</summary>
    /// <returns>Транспонированная матрица</returns>
    [DST] public MatrixComplex GetTranspose() => new(Array.Transpose(_Data));

    /// <summary>Алгебраическое дополнение к элементу [n,m]</summary>
    /// <param name="n">Номер столбца</param>
    /// <param name="m">Номер строки</param>
    /// <returns>Алгебраическое дополнение к элементу [n,m]</returns>
    public Complex GetAdjunct(int n, int m) => Array.GetAdjunct(_Data, n, m);

    /// <summary>Минор матрицы по определённому элементу</summary>
    /// <param name="n">Номер столбца</param>
    /// <param name="m">Номер строки</param>
    /// <returns>Минор элемента матрицы [n,m]</returns>
    public MatrixComplex GetMinor(int n, int m) => new(Array.GetMinor(_Data, n, m));

    /// <summary>Определитель матрицы</summary>
    public Complex GetDeterminant() => Array.GetDeterminant(_Data);

    /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
    /// <param name="L">Нижне-треугольная матрица</param>
    /// <param name="U">Верхне-треугольная матрица</param>
    /// <param name="P">Матрица преобразований P*X = L*U</param>
    /// <param name="D">Знак определителя</param>
    /// <returns>Истина, если разложение выполнено успешно, ложь - если матрица вырожденная</returns>
    public bool GetLUDecomposition(out MatrixComplex? L, out MatrixComplex? U, out MatrixComplex? P, out Complex D)
    {
        if (!IsSquare) throw new InvalidOperationException("Невозможно осуществить LU-разложение неквадратной матрицы");

        var decomposition_success = Array.GetLUPDecomposition(_Data, out var l, out var u, out var p, out var d);
        L = decomposition_success ? new MatrixComplex(l ?? throw new InvalidOperationException("Отсутствует ссылка на массив матрицы l")) : null;
        U = decomposition_success ? new MatrixComplex(u ?? throw new InvalidOperationException("Отсутствует ссылка на массив матрицы u")) : null;
        P = decomposition_success ? new MatrixComplex(p ?? throw new InvalidOperationException("Отсутствует ссылка на массив матрицы p")) : null;
        D = decomposition_success ? d : 0;
        return decomposition_success;
    }

    /// <summary>Получить внутренний массив элементов матрицы</summary>
    /// <returns></returns>
    [DST] public Complex[,] GetData() => _Data;

    /* -------------------------------------------------------------------------------------------- */

    /// <inheritdoc/>
    [DST]
    public override string ToString() => $"MatrixComplex[{_N}x{_M}]";

    /// <summary>Преобразование матрицы в строку с форматированием</summary>
    /// <param name="Format">Строка формата вывода чисел</param>
    /// <param name="Splitter">Разделитель элементов матрицы</param>
    /// <param name="provider">Механизм форматирования чисел матрицы</param>
    /// <returns>Строковое представление матрицы</returns>
    [DST]
    public string ToStringFormat
    (
        string Format = "r",
        string? Splitter = "\t",
        IFormatProvider? provider = null
    ) => _Data.ToStringFormatView(Format, Splitter, provider) ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    [DST] public string ToString(string format, IFormatProvider? provider) => _Data.ToStringFormatView(format, "\t", provider) ?? throw new InvalidOperationException();

    /* -------------------------------------------------------------------------------------------- */

    #region ICloneable Members

    /// <inheritdoc/>
    [DST] object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    [DST] Complex[,] ICloneable<Complex[,]>.Clone() => _Data.CloneObject();

    /// <inheritdoc/>
    [DST] public MatrixComplex Clone() => new(_Data, true);

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    [DST] public static bool operator ==(MatrixComplex? A, MatrixComplex? B) => A is null && B is null || A is not null && B is not null && A.Equals(B);

    [DST] public static bool operator !=(MatrixComplex? A, MatrixComplex? B) => !(A == B);

    [DST] public static bool operator ==(Complex[,]? A, MatrixComplex? B) => B == A;

    [DST] public static bool operator ==(MatrixComplex? A, Complex[,]? B) => A is null && B is null || A is not null && B is not null && A.Equals(B);

    [DST] public static bool operator !=(Complex[,]? A, MatrixComplex? B) => !(A == B);

    [DST] public static bool operator !=(MatrixComplex? A, Complex[,]? B) => !(A == B);

    [DST] public static MatrixComplex operator +(MatrixComplex M, Complex x) => new(Add(M._Data, x));

    [DST] public static MatrixComplex operator +(Complex x, MatrixComplex M) => new(Add(M._Data, x));

    [DST] public static MatrixComplex operator -(MatrixComplex M, Complex x) => new(Subtract(M._Data, x));

    [DST] public static MatrixComplex operator -(MatrixComplex M) => new(new Complex[M._N, M._M].Initialize(M._Data, (i, j, data) => -data[i, j]));

    [DST] public static MatrixComplex operator -(Complex x, MatrixComplex M) => new(Subtract(x, M._Data));

    [DST] public static MatrixComplex operator *(MatrixComplex M, Complex x) => new(Multiply(M._Data, x));

    [DST] public static MatrixComplex operator *(Complex x, MatrixComplex M) => new(Multiply(M._Data, x));

    [DST] public static MatrixComplex operator *(Complex[,] A, MatrixComplex B) => new(Multiply(A, B._Data));

    [DST] public static MatrixComplex operator *(Complex[] A, MatrixComplex B) => new(Multiply(Array.CreateColArray(A), B._Data));

    [DST] public static MatrixComplex operator *(MatrixComplex A, Complex[] B) => new(Multiply(A._Data, Array.CreateColArray(B)));

    [DST] public static MatrixComplex operator *(MatrixComplex A, Complex[,] B) => new(Multiply(A._Data, B));

    [DST] public static MatrixComplex operator /(MatrixComplex M, Complex x) => new(Divide(M._Data, x));

    [DST] public static MatrixComplex operator /(Complex x, MatrixComplex M) => new(Divide(x, M._Data));

    [DST]
    public static MatrixComplex operator ^(MatrixComplex M, int n)
    {
        if (!M.IsSquare) throw new ArgumentException("Матрица не квадратная", nameof(M));
        switch (n)
        {
            case 1:  return M.Clone();
            case -1: return M.GetInverse(out _);
            default:
                var m = M._Data;
                if (n < 0)
                {
                    m = Array.Inverse(m, out _);
                    n = -n;
                }
                var result                         = Array.GetUnitaryArrayMatrix(M._N);
                for (var i = 0; i < n; i++) result = Multiply(result, m);
                return new(result);
        }
    }

    /// <summary>Оператор сложения двух матриц</summary>
    /// <param name="A">Первое слагаемое</param><param name="B">Второе слагаемое</param><returns>Сумма двух матриц</returns>
    [DST] public static MatrixComplex operator +(MatrixComplex A, MatrixComplex B) => new(Add(A._Data, B._Data));

    /// <summary>Оператор разности двух матриц</summary>
    /// <param name="A">Уменьшаемое</param><param name="B">Вычитаемое</param><returns>Разность двух матриц</returns>
    [DST] public static MatrixComplex operator -(MatrixComplex A, MatrixComplex B) => new(Subtract(A._Data, B._Data));

    /// <summary>Оператор произведения двух матриц</summary>
    /// <param name="A">Первый сомножитель</param><param name="B">Второй сомножитель</param><returns>Произведение двух матриц</returns>
    [DST] public static MatrixComplex operator *(MatrixComplex A, MatrixComplex B) => new(Multiply(A._Data, B._Data));

    /// <summary>Оператор деления двух матриц</summary>
    /// <param name="A">Делимое</param><param name="B">Делитель</param><returns>Частное двух матриц</returns>
    [DST] public static MatrixComplex operator /(MatrixComplex A, MatrixComplex B) => new(Divide(A._Data, B._Data));

    /// <summary>Конкатенация двух матриц (либо по строкам, либо по столбцам)</summary>
    /// <param name="A">Первое слагаемое</param><param name="B">Второе слагаемое</param><returns>Объединённая матрица</returns>
    [DST] public static MatrixComplex operator |(MatrixComplex A, MatrixComplex B) => new(Concatenate(A._Data, B._Data));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного приведения типа вещественного числа двойной точности к типу Матрица порядка 1х1</summary>
    /// <param name="X">Приводимое число</param><returns>Матрица порядка 1х1</returns>
    [DST] public static implicit operator MatrixComplex(Complex X) => new(1, 1) { [0, 0] = X };

    [DST] public static explicit operator Complex[,](MatrixComplex M) => M._Data;

    [DST] public static explicit operator MatrixComplex(Complex[,] Data) => new(Data);

    [DST] public static explicit operator MatrixComplex(Complex[] Data) => new(Data);

    /* -------------------------------------------------------------------------------------------- */

    #region IEquatable Members

    /// <inheritdoc/>
    [DST] public bool Equals(Complex[,]? other) => other != null && Array.AreEquals(_Data, other);

    /// <inheritdoc/>
    [DST] public bool Equals(MatrixComplex? other) => other is not null && (ReferenceEquals(this, other) || Array.AreEquals(_Data, other._Data));

    #endregion

    /// <inheritdoc/>
    [DST] public override bool Equals(object? obj) => obj != null && (ReferenceEquals(this, obj) || Equals(obj as MatrixComplex) || Equals(obj as Complex[,]));

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