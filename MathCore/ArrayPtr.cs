using System.ComponentModel;
using System.Text;

namespace MathCore;

/// <summary>
/// Статический класс-помощник для создания и работы с указателями на массивы ArrayPtr&lt;T&gt;
/// </summary>
public static class ArrayPtr
{
    /// <summary>
    /// Создаёт указатель на весь массив
    /// </summary>
    /// <param name="array">Исходный массив</param>
    /// <returns>Указатель на весь массив с нулевым смещением</returns>
    /// <example>
    /// <![CDATA[
    /// var array = new[] { 1, 2, 3, 4, 5 };
    /// var ptr = ArrayPtr.Create(array);
    /// // ptr указывает на весь массив
    /// ]]>
    /// </example>
    public static ArrayPtr<T> Create<T>(T[] array) => new(array);

    /// <summary>
    /// Преобразует массив в указатель на весь массив
    /// </summary>
    /// <param name="array">Исходный массив</param>
    /// <returns>Указатель на весь массив</returns>
    public static ArrayPtr<T> ToArrayPtr<T>(this T[] array) => new(array);

    /// <summary>
    /// Преобразует массив в указатель с указанным смещением
    /// </summary>
    /// <param name="array">Исходный массив</param>
    /// <param name="Offset">Начальное смещение в массиве</param>
    /// <returns>Указатель на массив со смещением</returns>
    /// <example>
    /// <![CDATA[
    /// var array = new[] { 1, 2, 3, 4, 5 };
    /// var ptr = array.ToArrayPtr(2);
    /// // ptr указывает на элементы начиная с индекса 2
    /// ]]>
    /// </example>
    public static ArrayPtr<T> ToArrayPtr<T>(this T[] array, int Offset) => new(array, Offset);

    /// <summary>
    /// Преобразует массив в указатель с указанным смещением и длиной
    /// </summary>
    /// <param name="array">Исходный массив</param>
    /// <param name="Offset">Начальное смещение в массиве</param>
    /// <param name="Length">Количество элементов для доступа</param>
    /// <returns>Указатель на подмассив с заданным смещением и длиной</returns>
    /// <example>
    /// <![CDATA[
    /// var array = new[] { 1, 2, 3, 4, 5 };
    /// var ptr = array.ToArrayPtr(1, 3);
    /// // ptr указывает на элементы [2, 3, 4]
    /// ]]>
    /// </example>
    public static ArrayPtr<T> ToArrayPtr<T>(this T[] array, int Offset, int Length) => new(array, Offset, Length);
}

/// <summary>
/// Изменяемый ссылочный тип для работы с подмассивами без выделения дополнительной памяти
/// </summary>
/// <remarks>
/// ArrayPtr&lt;T&gt; является ref struct, что означает, что он может использоваться только на стеке
/// и не может быть боксирован, использован в async методах или храниться в полях класса.
/// Поддерживает отрицательные индексы (как Python): -1 указывает на последний элемент.
/// </remarks>
/// <example>
/// <![CDATA[
/// var array = new[] { 10, 20, 30, 40, 50 };
/// var ptr = new ArrayPtr<int>(array, 1, 3); // указывает на [20, 30, 40]
/// 
/// // Обращение по индексу
/// var first = ptr[0];  // 20
/// var last = ptr[-1];  // 40
/// 
/// // Разложение на голову и хвост
/// var (head, tail) = ptr;
/// // head = 20, tail указывает на [30, 40]
/// ]]>
/// </example>
public readonly ref struct ArrayPtr<T>(T[] array, int Offset = 0, int Length = -1)
{
    /// <summary>
    /// Конструктор с кортежем позиции (смещение и длина)
    /// </summary>
    /// <param name="array">Исходный массив</param>
    /// <param name="Position">Кортеж с полями Offset и Length</param>
    public ArrayPtr(T[] array, (int Offset, int Length) Position) : this(array, Position.Offset, Position.Length) { }

    private readonly T[] _Array = array;

    private readonly int _Offset = Offset < 0 ? (Length < 0 ? array.Length : Math.Max(array.Length, Length)) + Offset : Offset;

    /// <summary>
    /// Получает количество доступных элементов в подмассиве
    /// </summary>
    public int Length { get; } = Length < 0 ? array.Length : Math.Min(Length, array.Length);

    /// <summary>
    /// Получает или устанавливает элемент по индексу с поддержкой отрицательных индексов
    /// </summary>
    /// <param name="index">Индекс элемента. Отрицательные значения отсчитываются с конца</param>
    /// <returns>Ссылка на элемент массива</returns>
    /// <remarks>
    /// Индекс -1 указывает на последний элемент, -2 на предпоследний и так далее
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var ptr = new ArrayPtr<int>(new[] { 10, 20, 30 });
    /// var first = ptr[0];   // 10
    /// var last = ptr[-1];   // 30
    /// ptr[1] = 25;          // изменение элемента
    /// ]]>
    /// </example>
    public ref T this[int index] => ref _Array[_Offset + (index < 0 ? Length + index : index)];

    private int GetIndex(int index) => _Offset + (index < 0 ? Length + index : index);

    /// <summary>
    /// Получает срез между двумя индексами (в том числе отрицательными)
    /// </summary>
    /// <param name="Start">Начальный индекс</param>
    /// <param name="End">Конечный индекс</param>
    /// <returns>Новый ArrayPtr&lt;T&gt; для подмассива между индексами</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new ArrayPtr<int>(new[] { 10, 20, 30, 40, 50 });
    /// var slice = ptr[1, 4];  // новый указатель на элементы [20, 30, 40]
    /// ]]>
    /// </example>
    public ArrayPtr<T> this[int Start, int End] => new(_Array, (GetIndex(Start), GetIndex(End)).MinMaxToMinLength());

    /// <summary>
    /// Создаёт срез начиная с указанного смещения до конца
    /// </summary>
    /// <param name="Offset">Смещение относительно начала текущего подмассива</param>
    /// <returns>Новый ArrayPtr&lt;T&gt; для подмассива со смещением</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new ArrayPtr<int>(new[] { 10, 20, 30, 40 });
    /// var slice = ptr.Slice(2);  // новый указатель на [30, 40]
    /// ]]>
    /// </example>
    public ArrayPtr<T> Slice(int Offset) => new(_Array, _Offset + Offset, Length - Offset);

    /// <summary>
    /// Создаёт срез с указанным смещением и длиной
    /// </summary>
    /// <param name="Offset">Смещение относительно начала текущего подмассива</param>
    /// <param name="Length">Максимальное количество элементов в срезе</param>
    /// <returns>Новый ArrayPtr&lt;T&gt; для подмассива</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new ArrayPtr<int>(new[] { 10, 20, 30, 40, 50 });
    /// var slice = ptr.Slice(1, 2);  // новый указатель на [20, 30]
    /// ]]>
    /// </example>
    public ArrayPtr<T> Slice(int Offset, int Length) => new(_Array, _Offset + Offset, Math.Min(this.Length - Offset, Length));

    /// <summary>
    /// Разложение на первый элемент (голову) и оставшиеся элементы (хвост)
    /// </summary>
    /// <param name="head">Первый элемент подмассива</param>
    /// <param name="tail">Новый указатель на оставшиеся элементы начиная со второго</param>
    /// <remarks>
    /// Полезна для рекурсивной обработки элементов массива
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var ptr = new ArrayPtr<int>(new[] { 1, 2, 3 });
    /// var (head, tail) = ptr;
    /// // head = 1, tail указывает на [2, 3]
    /// ]]>
    /// </example>
    public void Deconstruct(out T head, out ArrayPtr<T> tail)
    {
        head = this[0];
        tail = Slice(1);
    }

    /// <summary>
    /// Копирует элементы подмассива в новый отдельный массив
    /// </summary>
    /// <returns>Новый массив, содержащий копию элементов подмассива</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new ArrayPtr<int>(new[] { 10, 20, 30, 40 }, 1, 2);
    /// var copy = ptr.ToArray();  // [20, 30]
    /// ]]>
    /// </example>
    public T[] ToArray()
    {
        var result = new T[Length];
        Array.Copy(_Array, _Offset, result, 0, Length);
        return result;
    }

    /// <summary>
    /// Возвращает строковое представление указателя с типом, смещением, длиной и элементами
    /// </summary>
    /// <remarks>
    /// Если элементов больше 10, выводится только информация о типе, смещении и длине с маркером *.
    /// В противном случае выводятся все элементы
    /// </remarks>
    public override string ToString()
    {
        var result = new StringBuilder(100).Append(typeof(T).Name);
        result.Append($"[{_Offset}:{Length}]");
        if (Length > 10)
            result.Append('*');
        else
        {
            result.Append('[');
            for (var (i, i1) = (_Offset, Math.Min(_Offset + Length, _Array.Length)); i < i1; i++)
                result.Append(_Array[i]).Append(',');
            result.Length--;
            result.Append(']');
        }

        return result.ToString();
    }

    /// <summary>
    /// Получает хеш-код для указателя
    /// </summary>
    /// <returns>Хеш-код, основанный на массиве, смещении и длине</returns>
    public override int GetHashCode() => HashBuilder.New(_Array).Append(_Offset).Append(Length);

    /// <summary>
    /// Сравнение объектов по равенству не поддерживается
    /// </summary>
    /// <exception cref="NotSupportedException">Всегда выбрасывает исключение</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <summary>
    /// Проверяет равенство двух указателей
    /// </summary>
    /// <param name="other">Другой указатель для сравнения</param>
    /// <returns>true, если оба указателя ссылаются на один и тот же массив с одинаковым смещением и длиной</returns>
    public bool Equals(ArrayPtr<T> other) =>
        ReferenceEquals(_Array, other._Array)
        && _Offset == other._Offset
        && Length == other.Length;

    /// <summary>
    /// Оператор проверки равенства двух указателей
    /// </summary>
    public static bool operator ==(ArrayPtr<T> a, ArrayPtr<T> b) => a.Equals(b);

    /// <summary>
    /// Оператор проверки неравенства двух указателей
    /// </summary>
    public static bool operator !=(ArrayPtr<T> a, ArrayPtr<T> b) => !a.Equals(b);

    /// <summary>
    /// Неявное преобразование массива в указатель на весь массив
    /// </summary>
    /// <param name="array">Исходный массив</param>
    public static implicit operator ArrayPtr<T>(T[] array) => new(array);

    /// <summary>
    /// Явное преобразование указателя в массив
    /// </summary>
    /// <param name="ptr">Исходный указатель</param>
    /// <returns>
    /// Если указатель ссылается на весь исходный массив, возвращает исходный массив.
    /// В противном случае создаёт и возвращает копию элементов подмассива
    /// </returns>
    /// <remarks>
    /// Используйте явное приведение, если нужна гарантия работы с отдельным массивом
    /// </remarks>
    public static explicit operator T[](ArrayPtr<T> ptr) =>
        ptr._Offset == 0 && ptr.Length == ptr._Array.Length
            ? ptr._Array
            : ptr.ToArray();
}
