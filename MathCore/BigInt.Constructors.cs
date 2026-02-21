namespace MathCore;

public partial class BigInt
{
    /// <summary>Инициализация нового пустого <see cref="BigInt"/> = 0</summary>
    public BigInt() => (_Data, _DataLength) = (new uint[MaxLength], 1);

    /// <summary>Инициализация нового пустого <see cref="BigInt"/> = 0</summary>
    /// <param name="Value">Исходное значение числа</param>
    public BigInt(long Value)
    {
        (_Data, var temp_val) = (new uint[MaxLength], Value);

        // Копирование байтов из long без предположений о размере типа

        var length = 0;
        while (Value != 0 && length < MaxLength)
        {
            _Data[length] = (uint)(Value & 0xFFFFFFFF);
            Value >>= 32;
            length++;
        }

        if (temp_val > 0) // Проверка переполнения для положительного значения
        {
            if (Value != 0 || (_Data[MaxLength - 1] & 0x80000000) != 0)
                throw new ArithmeticException("Positive overflow in constructor.");
        }
        else if (temp_val < 0) // Проверка переполнения для отрицательного значения
            if (Value != -1 || (_Data[length - 1] & 0x80000000) == 0)
                throw new ArithmeticException("Negative underflow in constructor.");

        _DataLength = Math.Max(1, length);
    }

    /// <summary>Инициализация нового пустого <see cref="BigInt"/> = 0</summary>
    /// <param name="Value">Исходное значение числа</param>
    public BigInt(ulong Value)
    {
        _Data = new uint[MaxLength];

        // Копирование байтов из ulong без предположений о размере типа

        var length = 0;
        while (Value != 0 && length < MaxLength)
        {
            _Data[length] = (uint)(Value & 0xFFFFFFFF);
            Value >>= 32;
            length++;
        }

        if (Value != 0 || (_Data[MaxLength - 1] & 0x80000000) != 0)
            throw new ArithmeticException("Positive overflow in constructor.");

        _DataLength = Math.Max(1, length);
    }

    /// <summary>Инициализация нового пустого <see cref="BigInt"/> = 0</summary>
    /// <param name="Value">Исходное значение числа</param>
    public BigInt(BigInt Value) => (_Data, _DataLength) = ((uint[])Value._Data.Clone(), Value._DataLength);

    //***********************************************************************
    // Конструктор (значение задаётся строкой цифр в указанной системе счисления)
    //
    // Пример (основание 10)
    // ---------------------
    // Для инициализации "a" значением 1234 в системе счисления 10
    //      BigInteger a = new BigInteger("1234", 10)
    //
    // Для инициализации "a" значением -1234
    //      BigInteger a = new BigInteger("-1234", 10)
    //
    // Пример (основание 16)
    // ---------------------
    // Для инициализации "a" значением 0x1D4F в системе счисления 16
    //      BigInteger a = new BigInteger("1D4F", 16)
    //
    // Для инициализации "a" значением -0x1D4F
    //      BigInteger a = new BigInteger("-1D4F", 16)
    //
    // Строковые значения задаются в формате <знак><модуль>
    //***********************************************************************

    /// <summary>Инициализация нового пустого <see cref="BigInt"/> = 0</summary>
    /// <param name="StringValue">Строковая форма записи <see cref="BigInt"/></param>
    /// <param name="Base">Основание системы счисления</param>
    /// <exception cref="ArithmeticException">Если очередной символ в строке больше, либо равен <paramref name="Base"/></exception>
    /// <exception cref="OverflowException">При переполнении разрядной сетки</exception>
    public BigInt(string StringValue, int Base = 10)
    {
        var multiplier = new BigInt(1);
        var result = new BigInt();
        StringValue = StringValue.ToUpper().Trim();
        var limit = 0;

        if (StringValue[0] == '-') limit = 1;

        for (var i = StringValue.Length - 1; i >= limit; i--)
        {
            var pos_val = (int)StringValue[i];

            if (pos_val is >= '0' and <= '9')
                pos_val -= '0';
            else
                pos_val = pos_val is >= 'A' and <= 'Z'
                    ? pos_val - 'A' + 10
                    : 9999999; // Произвольно большое значение


            if (pos_val >= Base)
                throw new ArithmeticException("Invalid string in constructor.");
            if (StringValue[0] == '-') pos_val = -pos_val;

            result += multiplier * pos_val;

            if (i - 1 >= limit) multiplier *= Base;
        }

        if (StringValue[0] == '-') // Отрицательные значения
        {
            if ((result._Data[MaxLength - 1] & 0x80000000) == 0)
                throw new OverflowException("Negative underflow in constructor.");
        }
        else // Положительные значения
        {
            if ((result._Data[MaxLength - 1] & 0x80000000) != 0)
                throw new OverflowException("Positive overflow in constructor.");
        }

        _Data = new uint[MaxLength];
        for (var i = 0; i < result._DataLength; i++)
            _Data[i] = result._Data[i];

        _DataLength = result._DataLength;
    }


    //***********************************************************************
    // Конструктор (значение задаётся массивом байтов)
    //
    // Младший индекс входного массива (т.е. [0]) содержит старший байт числа,
    // а старший индекс содержит младший байт
    //
    // Пример
    // Для инициализации "a" значением 0x1D4F в системе счисления 16
    //      byte[] temp = { 0x1D, 0x4F };
    //      BigInteger a = new BigInteger(temp)
    //
    // Этот способ инициализации не позволяет указать знак числа
    //***********************************************************************

    /// <summary>Инициализация нового пустого <see cref="BigInt"/> = 0</summary>
    /// <param name="Data">Байты данных <see cref="BigInt"/></param>
    /// <exception cref="OverflowException">При переполнении разрядной сетки</exception>
    public BigInt(byte[] Data)
    {
        _DataLength = Data.Length >> 2;

        var left_over = Data.Length & 0x3;
        if (left_over != 0) // Длина не кратна 4
            _DataLength++;

        if (_DataLength > MaxLength)
            throw new OverflowException("Byte overflow in constructor.");

        _Data = new uint[MaxLength];

        for (int i = Data.Length - 1, j = 0; i >= 3; i -= 4, j++)
            _Data[j] = (uint)((Data[i - 3] << 24) + (Data[i - 2] << 16) +
                (Data[i - 1] << 8) + Data[i]);

        _Data[_DataLength - 1] = left_over switch
        {
            1 => Data[0],
            2 => (uint)((Data[0] << 8) + Data[1]),
            3 => (uint)((Data[0] << 16) + (Data[1] << 8) + Data[2]),
            _ => _Data[_DataLength - 1]
        };

        while (_DataLength > 1 && _Data[_DataLength - 1] == 0) _DataLength--;
    }


    //***********************************************************************
    // Конструктор (значение задаётся массивом байтов указанной длины)
    //***********************************************************************
    /// <summary>Инициализация нового <see cref="BigInt"/> из массива байтов указанной длины</summary>
    /// <param name="Data">Байтовые данные числа</param>
    /// <param name="Length">Длина данных в байтах</param>
    /// <exception cref="OverflowException">При переполнении разрядной сетки</exception>
    public BigInt(byte[] Data, int Length)
    {
        _DataLength = Length >> 2;

        var left_over = Length & 0x3;
        if (left_over != 0) _DataLength++; // Длина не кратна 4

        if (_DataLength > MaxLength || Length > Data.Length)
            throw new OverflowException("Byte overflow in constructor.");

        _Data = new uint[MaxLength];

        for (int i = Length - 1, j = 0; i >= 3; i -= 4, j++)
            _Data[j] = (uint)((Data[i - 3] << 24) + (Data[i - 2] << 16) +
                (Data[i - 1] << 8) + Data[i]);

        _Data[_DataLength - 1] = left_over switch
        {
            1 => Data[0],
            2 => (uint)((Data[0] << 8) + Data[1]),
            3 => (uint)((Data[0] << 16) + (Data[1] << 8) + Data[2]),
            _ => _Data[_DataLength - 1]
        };

        if (_DataLength == 0)
            _DataLength = 1;

        while (_DataLength > 1 && _Data[_DataLength - 1] == 0)
            _DataLength--;
    }


    //***********************************************************************
    // Конструктор (значение задаётся массивом беззнаковых целых)
    //*********************************************************************

    /// <summary>Инициализация нового <see cref="BigInt"/> из массива 32-битных слов</summary>
    /// <param name="UintWords">Слова беззнаковых значений</param>
    /// <exception cref="OverflowException">При переполнении разрядной сетки</exception>
    public BigInt(uint[] UintWords)
    {
        _DataLength = UintWords.Length;

        if (_DataLength > MaxLength)
            throw new OverflowException("Byte overflow in constructor.");

        _Data = new uint[MaxLength];

        for (int i = _DataLength - 1, j = 0; i >= 0; i--, j++)
            _Data[j] = UintWords[i];

        while (_DataLength > 1 && _Data[_DataLength - 1] == 0)
            _DataLength--;

        //Console.WriteLine("Длина = " + _DataLength);
    }


    //***********************************************************************
    // Перегрузка оператора приведения типов
    // Для BigInteger Value = 10
    //***********************************************************************

    /// <summary>Неявное преобразование из <see cref="long"/></summary>
    /// <param name="value">Исходное значение</param>
    /// <returns>Экземпляр <see cref="BigInt"/></returns>
    public static implicit operator BigInt(long value) => new(value);

    /// <summary>Неявное преобразование из <see cref="ulong"/></summary>
    /// <param name="value">Исходное значение</param>
    /// <returns>Экземпляр <see cref="BigInt"/></returns>
    public static implicit operator BigInt(ulong value) => new(value);

    /// <summary>Неявное преобразование из <see cref="int"/></summary>
    /// <param name="value">Исходное значение</param>
    /// <returns>Экземпляр <see cref="BigInt"/></returns>
    public static implicit operator BigInt(int value) => new(value);

    /// <summary>Неявное преобразование из <see cref="uint"/></summary>
    /// <param name="value">Исходное значение</param>
    /// <returns>Экземпляр <see cref="BigInt"/></returns>
    public static implicit operator BigInt(uint value) => new((ulong)value);
}
