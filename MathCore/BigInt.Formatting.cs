namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Возвращает строковое представление BigInteger в системе счисления 10
    //***********************************************************************

    /// <summary>Преобразование числа в строку в десятичной системе счисления</summary>
    /// <returns>Строковое представление числа</returns>
    public override string ToString() => ToString(10);


    //***********************************************************************
    // Возвращает строковое представление BigInteger в знако-модульном формате
    // в заданной системе счисления
    //
    // Пример
    // -------
    // Если значение BigInteger равно -255 в системе счисления 10, то
    // ToString(16) возвращает "-FF"
    //
    //***********************************************************************

    /// <summary>Преобразование числа в строку в указанной системе счисления</summary>
    /// <param name="radix">Основание системы счисления</param>
    /// <returns>Строковое представление числа</returns>
    /// <exception cref="ArgumentException">Если основание вне диапазона от 2 до 36</exception>
    public string ToString(int radix)
    {
        if (radix is < 2 or > 36)
            throw new ArgumentException("Radix must be >= 2 and <= 36");

        // ReSharper disable once StringLiteralTypo
        const string char_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var result = string.Empty;

        var a = this;

        var negative = false;
        if ((a._Data[MaxLength - 1] & 0x80000000) != 0)
        {
            negative = true;
            try { a = -a; }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // игнорируется
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        var quotient = new BigInt();
        var remainder = new BigInt();
        var x_radix = new BigInt(radix);

        if (a._DataLength == 1 && a._Data[0] == 0)
            result = "0";
        else
        {
            while (a._DataLength > 1 || (a._DataLength == 1 && a._Data[0] != 0))
            {
                SingleByteDivide(a, x_radix, quotient, remainder);

                result = remainder._Data[0] < 10
                    ? remainder._Data[0] + result
                    : char_set[(int)remainder._Data[0] - 10] + result;

                a = quotient;
            }
            if (negative) result = "-" + result;
        }

        return result;
    }


    // ReSharper disable CommentTypo
    //***********************************************************************
    // Возвращает шестнадцатеричную строку, содержащую данные BigInteger
    //
    // Примеры
    // -------
    // 1) Если значение BigInteger равно 255 в системе счисления 10, то
    //    ToHexString() возвращает "FF"
    //
    // 2) Если значение BigInteger равно -255 в системе счисления 10, то
    //    ToHexString() возвращает ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
    //    что является представлением -255 в дополнительном коде
    //
    //***********************************************************************
    // ReSharper restore CommentTypo

    /// <summary>Представление <see cref="BigInt"/> в шестнадцатеричной системе счисления</summary>
    /// <returns>Строка шестнадцатеричного представления числа <see cref="BigInt"/></returns>
    public string ToHexString()
    {
        var result = _Data[_DataLength - 1].ToString("X");

        for (var i = _DataLength - 2; i >= 0; i--)
            result += _Data[i].ToString("X8");

        return result;
    }
}
