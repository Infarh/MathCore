#if NET5_0_OR_GREATER

#nullable enable
using System.Numerics;
using System.Text;

namespace MathCore;

public partial class Matrix<T>
    :
    ICloneable<Matrix<T>>, ICloneable<T[,]>, IFormattable,
    IEquatable<Matrix<T>>, IEquatable<T[,]>
    where T : IBinaryFloatingPointIeee754<T>
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Создать матрицу из двумерного массива чисел с плавающей запятой</summary>
    /// <param name="array">Двумерный массив чисел с плавающей запятой, из которого будет создана матрица</param>
    /// <param name="Clone">Если <c>true</c>, создаёт клон массива, в противном случае использует исходный массив</param>
    /// <returns>Новая матрица, созданная из указанного массива</returns>
    public static Matrix<T> Create(T[,] array, bool Clone = false) => new(array, Clone);

    /// <summary>Создать матрицу NxM, заполненную значениями, полученными с помощью указанной функции</summary>
    /// <param name="N">Число строк</param>
    /// <param name="M">Число столбцов</param>
    /// <param name="Initializer">Функция, принимающая индексы <paramref name="i"/> и <paramref name="j"/>, возвращающая значение элемента матрицы с индексами <paramref name="i"/> и <paramref name="j"/></param>
    /// <returns>Новая матрица, созданная с помощью указанной функции</returns>
    public static Matrix<T> Create(int N, int M, Func<int, int, T> Initializer) => new(N, M, (i, j) => Initializer(i, j));

    /// <summary>Создать матрицу-столбец</summary><param name="data">Элементы столбца</param><returns>Матрица-столбец</returns>
    /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
    /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
    public static Matrix<T> CreateCol(params T[] data) => new(Array.CreateColArray(data));

    /// <summary>Создать матрицу-строку</summary><param name="data">Элементы строки</param><returns>Матрица-строка</returns>
    /// <exception cref="ArgumentNullException">Если массив <paramref name="data"/> не определён</exception>
    /// <exception cref="ArgumentException">Если массив <paramref name="data"/> имеет длину 0</exception>
    public static Matrix<T> CreateRow(params T[] data) => new(Array.CreateRowArray(data));

    /// <summary>Создать диагональную матрицу</summary><param name="elements">Элементы диагональной матрицы</param>
    /// <returns>Диагональная матрица</returns>
    public static Matrix<T> CreateDiagonal(params T[] elements) => new(Array.CreateDiagonal(elements));

    /// <summary>Операции над двумерными массивами</summary>
    public static partial class Array
    {
        /// <summary>Операторы над двумерными массивами</summary>
        public static partial class Operator;
    }

    /// <summary>Получить единичную матрицу размерности NxN</summary>
    /// <param name="N">Размерность матрицы</param><returns>Единичная матрица размерности NxN с 1 на главной диагонали</returns>
    [DST]
    public static Matrix<T> GetUnitary(int N) => new(Array.GetUnitaryArrayMatrix(N));

    /// <summary>Создать матрицу NxM, заполненную единицами</summary>
    /// <param name="N">Число строк</param>
    /// <param name="M">Число столбцов</param>
    /// <returns>Матрица размером NxM с элементами, равными 1</returns>
    public static Matrix<T> GetOnes(int N, int M)
    {
        var result = new T[N, M];
        for (var i = 0; i < N; i++)
            for (var j = 0; j < N; j++)
                result[i, j] = T.One;
        return new(result);
    }

    /// <summary>Создать матрицу NxM, заполненную нулями</summary>
    /// <param name="N">Число строк</param>
    /// <param name="M">Число столбцов</param>
    /// <returns>Матрица размером NxM с элементами, равными 0</returns>
    public static Matrix GetZeros(int N, int M) => new(N, M);

    /// <summary>Трансвекция матрицы</summary><param name="A">Трансвецируемая матрица</param><param name="j">Опорный столбец</param>
    /// <returns>Трансвекция матрицы А</returns>
    public static Matrix<T> GetTransvection(Matrix<T> A, int j) => new(Array.GetTransvection(A._Data, j));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Число строк матрицы</summary>
    private readonly int _N;

    /// <summary>Число столбцов матрицы</summary>
    private readonly int _M;

    /// <summary>Элементы матрицы</summary>
    private readonly T[,] _Data;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Число строк матрицы</summary>
    public int N => _N;

    /// <summary>Число столбцов матрицы</summary>
    public int M => _M;

    /// <summary>Элемент матрицы</summary>
    /// <param name="i">Номер строки (элемента в столбце)</param>
    /// <param name="j">Номер столбца (элемента в строке)</param>
    /// <returns>Элемент матрицы</returns>
    public ref T this[int i, int j] { [DST] get => ref _Data[i, j]; }

    /// <summary>Вектор-столбец</summary><param name="j">Номер столбца</param><returns>Столбец матрицы</returns>
    public Matrix<T> this[int j] => GetCol(j);

    /// <summary>Матрица является квадратной матрицей</summary>
    public bool IsSquare => _M == _N;

    /// <summary>Матрица является столбцом</summary>
    public bool IsCol => _M == 1;

    /// <summary>Матрица является строкой</summary>
    public bool IsRow => _N == 1;

    /// <summary>Матрица является числом</summary>
    public bool IsScalar => _N == 1 && _M == 1;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="N"/> &lt; 0 || <paramref name="M"/> &lt; 0</exception>
    [DST]
    public Matrix(int N, int M)
    {
        if (N <= 0) throw new ArgumentOutOfRangeException(nameof(N), N, "N должна быть больше 0");
        if (M <= 0) throw new ArgumentOutOfRangeException(nameof(M), M, "M должна быть больше 0");

        _Data = new T[_N = N, _M = M];
    }

    /// <summary>Квадратная матрица</summary><param name="N">Размерность</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="N" /> &lt; 0</exception>
    [DST] public Matrix(int N) : this(N, N) { }

    /// <summary>Метод определения значения элемента матрицы</summary>
    /// <param name="i">Номер строки</param><param name="j">Номер столбца</param>
    /// <returns>Значение элемента матрицы M[<paramref name="i"/>, <paramref name="j"/>]</returns>
    public delegate T MatrixItemCreator(int i, int j);

    /// <summary>Квадратная матрица</summary>
    /// <param name="N">Размерность</param>
    /// <param name="CreateFunction">Порождающая функция</param>
    [DST] public Matrix(int N, MatrixItemCreator CreateFunction) : this(N, N, CreateFunction) { }

    /// <summary>Матрица</summary><param name="N">Число строк</param><param name="M">Число столбцов</param>
    /// <param name="CreateFunction">Порождающая функция</param>
    [DST]
    public Matrix(int N, int M, MatrixItemCreator CreateFunction) : this(N, M)
    {
        for (var i = 0; i < N; i++)
            for (var j = 0; j < M; j++)
                _Data[i, j] = CreateFunction(i, j);
    }

    /// <summary>Инициализация новой матрицы по двумерному массиву её элементов</summary>
    /// <param name="Data">Двумерный массив элементов матрицы</param>
    /// <param name="clone">Создать копию данных</param>
    [DST]
    public Matrix(T[,] Data, bool clone = false)
    {
        _N = Data.GetLength(0);
        _M = Data.GetLength(1);
        _Data = clone ? Data.CloneObject() : Data;
    }

    /// <summary>Инициализация новой матрицы - столбца/строки</summary>
    /// <param name="DataCol">Элементы столбца матрицы</param>
    /// <param name="IsColumn">Создаётся матрица-столбец</param>
    [DST]
    public Matrix(IList<T> DataCol, bool IsColumn = true) : this(IsColumn ? DataCol.Count : 1, IsColumn ? 1 : DataCol.Count)
    {
        if (IsColumn) for (var i = 0; i < _N; i++) _Data[i, 0] = DataCol[i];
        else for (var j = 0; j < _M; j++) _Data[0, j] = DataCol[j];
    }

    /// <summary>Инициализация новой матрицы на основе перечисления строк (перечисления элементов строк) </summary>
    /// <param name="Items">Перечисление строк, состоящих из перечисления элементов строк</param>
    public Matrix(IEnumerable<IEnumerable<T>> Items) : this(GetElements(Items)) { }

    /// <summary>Получить двумерный массив элементов матрицы</summary>
    /// <param name="ColsItems">Перечисление элементов (по столбцам)</param>
    /// <returns>Двумерный массив элементов матрицы</returns>
    [DST]
    private static T[,] GetElements(IEnumerable<IEnumerable<T>> ColsItems)
    {
        var cols = ColsItems.Select(col => col.ToListFast()).ToList();
        var cols_count = cols.Count;
        var rows_count = cols.Max(col => col.Count);
        var data = new T[rows_count, cols_count];
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
    [DST] public Matrix<T> GetCol(int j) => new(Array.GetCol(_Data, j));

    /// <summary>Получить строку матрицы</summary>
    /// <param name="i">Номер строки</param>
    /// <returns>Строка матрицы номер i</returns>
    [DST] public Matrix<T> GetRow(int i) => new(Array.GetRow(_Data, i));

    /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
    /// <param name="P">Матрица перестановок</param>
    /// <param name="rank">Ранг матрицы</param>
    /// <param name="D">Определитель</param>
    /// <returns>Треугольная матрица</returns>
    public Matrix<T> GetTriangle(out Matrix<T> P, out int rank, out T D)
    {
        var result = new Matrix<T>(Array.GetTriangle(_Data, out var p, out rank, out D));
        P = new(p);
        return result;
    }

    /// <summary>Приведение матрицы к ступенчатому виду методом Гаусса</summary>
    /// <param name="B">Присоединённая матрица правой части СЛАУ</param>
    /// <param name="CloneB">Работать с клоном матрицы <paramref name="B"/></param>
    /// <returns>Треугольная матрица</returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="B"/> <see langword="null"/></exception>
    public Matrix<T> GetTriangle(ref Matrix<T> B, bool CloneB = true)
    {
        var b = CloneB ? B._Data.CloneObject() : B._Data;
        var result = new Matrix<T>(Array.GetTriangle(_Data, b, out _, out _));
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
    public Matrix<T> GetTriangle(ref Matrix<T> B, out Matrix<T> P, out int rank, out T d, bool CloneB = true)
    {
        var b = B._Data;
        var result = new Matrix<T>(Array.GetTriangle(_Data, ref b, out var p, out rank, out d, CloneB));
        P = new(p);
        if (CloneB) B = new(b);
        return result;
    }

    /// <summary>Получить обратную матрицу</summary>
    /// <param name="P">Матрица перестановок</param>
    /// <returns>Обратная матрица</returns>
    public Matrix<T> GetInverse(out Matrix<T> P)
    {
        var inverse = new Matrix<T>(Array.Inverse(_Data, out var p));
        P = new(p);
        return inverse;
    }

    /// <summary>Транспонирование матрицы</summary>
    /// <returns>Транспонированная матрица</returns>
    [DST] public Matrix<T> GetTranspose() => new(Array.Transpose(_Data));

    /// <summary>Алгебраическое дополнение к элементу [n, m]</summary>
    /// <param name="n">Номер столбца</param>
    /// <param name="m">Номер строки</param>
    /// <returns>Алгебраическое дополнение к элементу [n, m]</returns>
    public T GetAdjunct(int n, int m) => Array.GetAdjunct(_Data, n, m);

    /// <summary>Минор матрицы по определённому элементу</summary>
    /// <param name="n">Номер столбца</param>
    /// <param name="m">Номер строки</param>
    /// <returns>Минор элемента матрицы [n, m]</returns>
    public Matrix<T> GetMinor(int n, int m) => new(Array.GetMinor(_Data, n, m));

    /// <summary>Определитель матрицы</summary>
    public T GetDeterminant() => Array.GetDeterminant(_Data);

    /// <summary>Разложение матрицы на верхне-треугольную и нижне-треугольную</summary>
    /// <param name="L">Нижне-треугольная матрица</param>
    /// <param name="U">Верхне-треугольная матрица</param>
    /// <param name="P">Матрица преобразований P*X = L*U</param>
    /// <param name="D">Знак определителя</param>
    /// <returns>Истина, если разложение выполнено успешно, ложь - если матрица вырожденная</returns>
    public bool GetLUDecomposition(out Matrix<T>? L, out Matrix<T>? U, out Matrix<T>? P, out T D)
    {
        if (!IsSquare) throw new InvalidOperationException("Невозможно осуществить LU-разложение неквадратной матрицы");

        var decomposition_success = Array.GetLUPDecomposition(_Data, out var l, out var u, out var p, out var d);
        L = decomposition_success ? new Matrix<T>(l ?? throw new InvalidOperationException("l is null")) : null;
        U = decomposition_success ? new Matrix<T>(u ?? throw new InvalidOperationException("u is null")) : null;
        P = decomposition_success ? new Matrix<T>(p ?? throw new InvalidOperationException("p is null")) : null;
        D = decomposition_success ? d : T.Zero;
        return decomposition_success;
    }

    /// <summary>Получить внутренний массив элементов матрицы</summary>
    /// <returns></returns>
    [DST] public T[,] GetData() => _Data;

    /// <summary>Деконструктор матрицы, позволяющий получить размеры матрицы</summary>
    /// <param name="N">Число строк (первое измерение)</param>
    /// <param name="M">Число столбцов (второе измерение)</param>
    public void Deconstruct(out int N, out int M) => (N, M) = (_N, _M);

    /* -------------------------------------------------------------------------------------------- */

    /// <inheritdoc/>
    [DST] public override string ToString() => $"Matrix[{_N}x{_M}]";

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

    /// <summary>Структура-оболочка для матрицы, которая обеспечивает вывод матрицы в виде строки C#-инициализации</summary>
    /// <param name="Matrix">Матрица, которую нужно вывести</param>
    public readonly ref struct MatrixView(Matrix<T> Matrix)
    {
        /// <summary>Выдать строку C#-инициализации матрицы</summary>
        /// <returns>Строка C#-инициализации матрицы</returns>
        public override string ToString()
        {
            var (n, m) = Matrix;

            var ss = new string[n, m];
            var ll = new int[m];
            var nn = Matrix._Data;

            for (var i = 0; i < n; i++)
                for (var j = 0; j < m; j++)
                {
                    var s = FormattableString.CurrentCulture($"{nn[i, j]}");
                    ss[i, j] = s;
                    ll[j] = Math.Max(ll[j], s.Length);
                }
            var colum_nums = string.Join(", ", Enumerable.Range(1, n).Select(i => i.ToString().PadLeft(ll[i - 1])));

            var result = new StringBuilder("T[,] z_matrix = ").LN()
               .Append('{').LN()
               .Append("    //       {0}", colum_nums).LN()
               .Append("    //       {0}", new string('-', colum_nums.Length)).LN()
                ;

            var i_pad = (int)Math.Ceiling(Math.Log10(n));
            for (var i = 0; i < n; i++)
            {
                result.Append("    /*{0}*/ {{ ", (i + 1).ToString().PadLeft(i_pad));
                for (var j = 0; j < m; j++)
                    result.Append("{0}, ", ss[i, j].PadLeft(ll[j]));

                result.Length -= 2;
                result.AppendLine(" },");
            }

            result.AppendLine("};");
            return result.ToString();
        }

        /// <summary>Выдать строку C#-инициализации матрицы</summary>
        /// <param name="Format">Строка формата вывода чисел</param>
        /// <returns>Строка C#-инициализации матрицы</returns>
        public string ToString(string Format)
        {
            var (n, m) = Matrix;

            var ss = new string[n, m];
            var ll = new int[m];
            var nn = Matrix._Data;

            for (var i = 0; i < n; i++)
                for (var j = 0; j < m; j++)
                {
                    var s = nn[i, j].ToString(Format, null);
                    ss[i, j] = s;
                    ll[j] = Math.Max(ll[j], s.Length);
                }

            var colum_nums = string.Join(", ", Enumerable.Range(1, n).Select(i => i.ToString().PadLeft(ll[i - 1])));

            var result = new StringBuilder("T[,] z_matrix = ").LN()
                   .Append('{')
                   .Append("    //       {0}", colum_nums).AppendLine()
                   .Append("    //       {0}", new string('-', colum_nums.Length)).AppendLine()
                ;

            for (var i = 0; i < n; i++)
            {
                result.Append("    /*{0,2}*/ {{ ", i + 1);
                for (var j = 0; j < m; j++)
                    result.Append("{0}, ", ss[i, j].PadLeft(ll[j]));

                result.Length -= 2;
                result.AppendLine(" },");
            }

            result.AppendLine("};");
            return result.ToString();
        }

        /// <summary>Выдать строку C#-инициализации матрицы</summary>
        /// <param name="Provider">Механизм форматирования чисел</param>
        /// <returns>Строка C#-инициализации матрицы</returns>
        public string ToString(IFormatProvider Provider)
        {
            var (n, m) = Matrix;

            var ss = new string[n, m];
            var ll = new int[m];
            var nn = Matrix._Data;

            for (var i = 0; i < n; i++)
                for (var j = 0; j < m; j++)
                {
                    FormattableString sss = $"{nn[i, j]}";
                    var s = sss.ToString(Provider);
                    ss[i, j] = s;
                    ll[j] = Math.Max(ll[j], s.Length);
                }
            var colum_nums = string.Join(", ", Enumerable.Range(1, n).Select(i => i.ToString().PadLeft(ll[i - 1])));

            var result = new StringBuilder("T[,] z_matrix = ").LN()
                   .Append('{')
                   .Append("    //       {0}", colum_nums).AppendLine()
                   .Append("    //       {0}", new string('-', colum_nums.Length)).AppendLine()
                ;

            for (var i = 0; i < n; i++)
            {
                result.Append("    /*{0,2}*/ {{ ", i + 1);
                for (var j = 0; j < m; j++)
                    result.Append("{0}, ", ss[i, j].PadLeft(ll[j]));

                result.Length -= 2;
                result.AppendLine(" },");
            }

            result.AppendLine("};");
            return result.ToString();
        }

        /// <summary>Выдать строку C#-инициализации матрицы</summary>
        /// <param name="Format">Строка формата вывода чисел</param>
        /// <param name="Provider">Механизм форматирования чисел</param>
        /// <returns>Строка C#-инициализации матрицы</returns>
        public string ToString(string Format, IFormatProvider Provider)
        {
            var (n, m) = Matrix;

            var ss = new string[n, m];
            var ll = new int[m];
            var nn = Matrix._Data;

            for (var i = 0; i < n; i++)
                for (var j = 0; j < m; j++)
                {
                    var s = nn[i, j].ToString(Format, Provider);
                    ss[i, j] = s;
                    ll[j] = Math.Max(ll[j], s.Length);
                }
            var colum_nums = string.Join(", ", Enumerable.Range(1, n).Select(i => i.ToString().PadLeft(ll[i - 1])));

            var result = new StringBuilder("T[,] z_matrix = ").LN()
                   .Append('{')
                   .Append("    //       {0}", colum_nums).AppendLine()
                   .Append("    //       {0}", new string('-', colum_nums.Length)).AppendLine()
                ;

            for (var i = 0; i < n; i++)
            {
                result.Append("    /*{0,2}*/ {{ ", i + 1);
                for (var j = 0; j < m; j++)
                    result.Append("{0}, ", ss[i, j].PadLeft(ll[j]));

                result.Length -= 2;
                result.AppendLine(" },");
            }

            result.AppendLine("};");
            return result.ToString();
        }
    }

    public MatrixView View() => new(this);

    /* -------------------------------------------------------------------------------------------- */

    #region ICloneable Members

    /// <inheritdoc/>
    [DST] object ICloneable.Clone() => Clone();

    /// <inheritdoc/>
    [DST] T[,] ICloneable<T[,]>.Clone() => _Data.CloneObject();

    /// <inheritdoc/>
    [DST] public Matrix<T> Clone() => new(_Data, true);

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Оператор равенства двух матриц</summary>
    /// <returns>Истина, если матрицы совпадают по размеру и поэлементно</returns>
    [DST] public static bool operator ==(Matrix<T>? A, Matrix<T>? B) => A is null && B is null || A is not null && B is not null && A.Equals(B);

    /// <summary>Оператор неравенства двух матриц</summary>
    /// <returns>Истина, если матрицы не совпадают по размеру или поэлементно</returns>
    [DST] public static bool operator !=(Matrix<T>? A, Matrix<T>? B) => !(A == B);

    /// <summary>Оператор равенства матрицы и двумерного массива</summary>
    /// <returns>Истина, если матрица и двумерный массив совпадают по размеру и поэлементно</returns>
    [DST] public static bool operator ==(T[,]? A, Matrix<T>? B) => B == A;

    /// <summary>Оператор равенства матрицы и двумерного массива</summary>
    /// <returns>Истина, если матрица и двумерный массив совпадают по размеру и поэлементно</returns>
    [DST] public static bool operator ==(Matrix<T>? A, T[,]? B) => A is null && B is null || A is not null && B is not null && A.Equals(B);

    /// <summary>Оператор неравенства матрицы и двумерного массива</summary>
    /// <returns>Истина, если матрица и двумерный массив не совпадают по размеру или поэлементно</returns>
    [DST] public static bool operator !=(T[,]? A, Matrix<T>? B) => !(A == B);

    /// <summary>Оператор неравенства матрицы и двумерного массива</summary>
    /// <returns>Истина, если матрица и двумерный массив не совпадают по размеру или поэлементно</returns>
    [DST] public static bool operator !=(Matrix<T>? A, T[,]? B) => !(A == B);

    /// <summary>Оператор суммы матрицы и числа</summary>
    /// <returns>Матрица, элементы которой равны сумме элементов исходной матрицы и числа</returns>
    [DST] public static Matrix<T> operator +(Matrix<T> M, T x) => new(Array.Operator.Add(M._Data, x));

    /// <summary>Оператор суммы матрицы и числа</summary>
    /// <returns>Матрица, элементы которой равны сумме элементов исходной матрицы и числа</returns>
    [DST] public static Matrix<T> operator +(T x, Matrix<T> M) => new(Array.Operator.Add(M._Data, x));

    /// <summary>Оператор разности матрицы и числа</summary>
    /// <returns>Матрица, элементы которой равны разности элементов исходной матрицы и числа</returns>
    [DST] public static Matrix<T> operator -(Matrix<T> M, T x) => new(Array.Operator.Subtract(M._Data, x));

    /// <summary>Оператор отрицания элементов матрицы</summary>
    /// <returns>Матрица, элементы которой являются отрицательными по отношению к элементам исходной матрицы</returns>
    [DST] public static Matrix<T> operator -(Matrix<T> M) => new(new T[M._N, M._M].Initialize(M._Data, (i, j, data) => -data[i, j]));

    /// <summary>Оператор разности числа и матрицы</summary>
    /// <returns>Матрица, элементы которой равны разности числа и элементов исходной матрицы</returns>
    [DST] public static Matrix<T> operator -(T x, Matrix<T> M) => new(Array.Operator.Subtract(x, M._Data));

    /// <summary>Оператор произведения матрицы и числа</summary>
    /// <returns>Матрица, элементы которой равны произведения элементов исходной матрицы и числа</returns>
    [DST] public static Matrix<T> operator *(Matrix<T> M, T x) => new(Array.Operator.Multiply(M._Data, x));

    /// <summary>Оператор суммы матрицы и числа</summary>
    /// <returns>Матрица, элементы которой равны сумме элементов исходной матрицы и числа</returns>
    [DST] public static Matrix<T> operator *(T x, Matrix<T> M) => new(Array.Operator.Multiply(M._Data, x));

    /// <summary>Оператор матричного произведения двумерного массива и матрицы</summary>
    /// <returns>Матрица - результат матричного умножения двухмерного массива и матрицы</returns>
    [DST] public static Matrix<T> operator *(T[,] A, Matrix<T> B) => new(Array.Operator.Multiply(A, B._Data));

    /// <summary>Оператор матричного произведения одномерного массива (строки) и матрицы</summary>
    /// <returns>Матрица - результат матричного умножения одномерного массива (строки) и матрицы</returns>
    [DST] public static Matrix<T> operator *(T[] A, Matrix<T> B) => new(Array.Operator.Multiply(Array.CreateColArray(A), B._Data));

    /// <summary>Оператор матричного произведения матрицы и одномерного массива (столбца)</summary>
    /// <returns>Матрица - результат матричного умножения матрицы и одномерного массива (столбца)</returns>
    [DST] public static Matrix<T> operator *(Matrix<T> A, T[] B) => new(Array.Operator.Multiply(A._Data, Array.CreateColArray(B)));

    /// <summary>Оператор матричного произведения матрицы и двумерного массива</summary>
    /// <returns>Матрица - результат матричного умножения матрицы и двумерного массива</returns>
    [DST] public static Matrix<T> operator *(Matrix<T> A, T[,] B) => new(Array.Operator.Multiply(A._Data, B));

    /// <summary>Оператор деления матрицы и числа</summary>
    /// <returns>Матрица, элементы которой равны результату деления элементов исходной матрицы и числа</returns>
    [DST] public static Matrix<T> operator /(Matrix<T> M, T x) => new(Array.Operator.Divide(M._Data, x));

    /// <summary>Оператор деления числа и матрицы</summary>
    /// <returns>Матрица, элементы которой равны результату деления числа и элементов исходной матрицы</returns>
    [DST] public static Matrix<T> operator /(T x, Matrix<T> M) => new(Array.Operator.Divide(x, M._Data));

    /// <summary>Оператор возведения матрицы в степень</summary>
    /// <param name="M">Матрица - основание</param>
    /// <param name="n">Показатель степени</param>
    /// <returns>Матрица - результат возведения исходной матрицы в целую степень</returns>
    [DST]
    public static Matrix<T> operator ^(Matrix<T> M, int n)
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
                var result = Array.GetUnitaryArrayMatrix(M._N);
                for (var i = 0; i < n; i++) 
                    result = Array.Operator.Multiply(result, m);
                return new(result);
        }
    }

    /// <summary>Оператор сложения двух матриц</summary>
    /// <param name="A">Первое слагаемое</param><param name="B">Второе слагаемое</param><returns>Сумма двух матриц</returns>
    [DST] public static Matrix<T> operator +(Matrix<T> A, Matrix<T> B) => new(Array.Operator.Add(A._Data, B._Data));

    /// <summary>Оператор разности двух матриц</summary>
    /// <param name="A">Уменьшаемое</param><param name="B">Вычитаемое</param><returns>Разность двух матриц</returns>
    [DST] public static Matrix<T> operator -(Matrix<T> A, Matrix<T> B) => new(Array.Operator.Subtract(A._Data, B._Data));

    /// <summary>Оператор произведения двух матриц</summary>
    /// <param name="A">Первый сомножитель</param><param name="B">Второй сомножитель</param><returns>Произведение двух матриц</returns>
    [DST] public static Matrix<T> operator *(Matrix<T> A, Matrix<T> B) => new(Array.Operator.Multiply(A._Data, B._Data));

    /// <summary>Оператор деления двух матриц</summary>
    /// <param name="A">Делимое</param><param name="B">Делитель</param><returns>Частное двух матриц</returns>
    [DST] public static Matrix<T> operator /(Matrix<T> A, Matrix<T> B) => new(Array.Operator.Divide(A._Data, B._Data));

    /// <summary>Конкатенация двух матриц (либо по строкам, либо по столбцам)</summary>
    /// <param name="A">Первое слагаемое</param><param name="B">Второе слагаемое</param><returns>Объединённая матрица</returns>
    [DST] public static Matrix<T> operator |(Matrix<T> A, Matrix<T> B) => new(Array.Operator.Concatenate(A._Data, B._Data));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного приведения типа вещественного числа двойной точности к типу Матрица порядка 1х1</summary>
    /// <param name="X">Приводимое число</param><returns>Матрица порядка 1х1</returns>
    [DST] public static implicit operator Matrix<T>(T X) => new(1, 1) { [0, 0] = X };

    /// <summary>Оператор явного приведения матрицы к двумерному массиву</summary>
    /// <param name="M">Исходная матрица</param>
    [DST] public static explicit operator T[,](Matrix<T> M) => M._Data;

    /// <summary>Оператор явного приведения типа двумерного массива к матрице</summary>
    /// <param name="Data">Двумерный массив</param>
    [DST] public static explicit operator Matrix<T>(T[,] Data) => new(Data);

    /// <summary>Оператор явного приведения одномерного массива к матрице (столбцу)</summary>
    /// <param name="Data">Одномерный массив</param>
    [DST] public static explicit operator Matrix<T>(T[] Data) => new(Data);

    /* -------------------------------------------------------------------------------------------- */

    #region IEquatable Members

    /// <inheritdoc/>
    [DST] public bool Equals(T[,]? other) => other != null && Array.AreEquals(_Data, other);

    /// <inheritdoc/>
    [DST] public bool Equals(Matrix<T>? other) => other is not null && (ReferenceEquals(this, other) || Array.AreEquals(_Data, other._Data));

    #endregion

    /// <inheritdoc/>
    [DST] public override bool Equals(object? obj) => obj != null && (ReferenceEquals(this, obj) || Equals(obj as Matrix) || Equals(obj as T[,]));

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


#endif