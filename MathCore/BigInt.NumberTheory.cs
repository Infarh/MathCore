namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Возвращает gcd(this, Value)
    //***********************************************************************

    /// <summary>Наибольший общий делитель</summary>
    /// <param name="X">Второе значение</param>
    /// <returns>Наибольший общий делитель</returns>
    public BigInt Gcd(BigInt X)
    {
        var x = (_Data[MaxLength - 1] & 0x80000000) != 0 ? -this : this;

        var y = (X._Data[MaxLength - 1] & 0x80000000) != 0 ? -X : X;

        var g = y;

        while (x._DataLength > 1 || (x._DataLength == 1 && x._Data[0] != 0))
        {
            g = x;
            x = y % x;
            y = g;
        }

        return g;
    }


    //***********************************************************************
    // Вычисляет символ Якоби для a и b
    // Алгоритм адаптирован из [3] и [4] с некоторыми оптимизациями
    //***********************************************************************

    /// <summary>Вычисление символа Якоби</summary>
    /// <param name="a">Первый аргумент</param>
    /// <param name="b">Второй аргумент</param>
    /// <returns>Значение символа Якоби</returns>
    /// <exception cref="ArgumentException">Если <paramref name="b"/> чётное</exception>
    public static int Jacobi(BigInt a, BigInt b)
    {
        // Символ Якоби определён только для нечётных чисел
        if ((b._Data[0] & 0x1) == 0)
            throw new ArgumentException("Jacobi defined only for odd integers.");

        if (a >= b) a %= b;
        switch (a._DataLength)
        {
            case 1 when a._Data[0] == 0: return 0; // a == 0
            case 1 when a._Data[0] == 1: return 1; // a == 1
        }

        if (a < 0)
            return ((b - 1)._Data[0] & 0x2) == 0
                ? Jacobi(-a, b)
                : -Jacobi(-a, b);

        var e = 0;
        for (var index = 0; index < a._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++, mask <<= 1, e++)
                if ((a._Data[index] & mask) != 0)
                {
                    index = a._DataLength; // выход из внешнего цикла
                    break;
                }
        }

        var a1 = a >> e;

        var s = 1;
        if ((e & 0x1) != 0 && ((b._Data[0] & 0x7) == 3 || (b._Data[0] & 0x7) == 5))
            s = -1;

        if ((b._Data[0] & 0x3) == 3 && (a1._Data[0] & 0x3) == 3)
            s = -s;

        return a1._DataLength == 1 && a1._Data[0] == 1 ? s : s * Jacobi(b % a1, a1);
    }


    //***********************************************************************
    // Генерирует положительное число, которое вероятно простое
    //***********************************************************************

    /// <summary>Генерация псевдопростого числа</summary>
    /// <param name="bits">Количество бит</param>
    /// <param name="confidence">Уровень уверенности</param>
    /// <param name="rand">Генератор случайных чисел</param>
    /// <returns>Псевдопростое число</returns>
    public static BigInt GetPseudoPrime(int bits, int confidence, Random rand)
    {
        var result = new BigInt();
        var done = false;

        while (!done)
        {
            result.GenRandomBits(bits, rand);
            result._Data[0] |= 0x01; // делаем число нечётным

            // проверка простоты
            done = result.IsProbablePrime(confidence);
        }
        return result;
    }


    //***********************************************************************
    // Генерирует случайное число заданной длины бит так,
    // что gcd(number, this) = 1
    //***********************************************************************

}
