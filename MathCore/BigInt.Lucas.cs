namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Возвращает k-й элемент последовательности Лукаса по модулю n
    //
    // Использует удвоение индекса для ускорения. Например, для вычисления V(k)
    // поддерживаются два числа последовательности V(n) и V(n+1)
    //
    // Для получения V(2n) используется тождество
    //      V(2n) = (V(n) * V(n)) - (2 * Q^n)
    // Для получения V(2n+1) сначала записываем
    //      V(2n+1) = V((n+1) + n)
    // и используем тождество
    //      V(m+n) = V(m) * V(n) - Q * V(m-n)
    // Следовательно,
    //      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
    //                   = V(n+1) * V(n) - Q^n * V(1)
    //                   = V(n+1) * V(n) - Q^n * P
    //
    // Используется двоичное представление k и удвоение индекса для каждого бита
    // Для каждого установленного бита выполняется удвоение индекса и добавление
    // Это означает, что для V(n) нужно обновить значение до V(2n+1)
    // Для V(n+1) нужно обновить значение до V((2n+1)+1) = V(2*(n+1))
    //
    // Функция возвращает
    // [0] = U(k)
    // [1] = V(k)
    // [2] = Q^n
    //
    // Где U(0) = 0 % n, U(1) = 1 % n
    //       V(0) = 2 % n, V(1) = P % n
    //***********************************************************************

    /// <summary>Вычисление элементов последовательности Лукаса по модулю</summary>
    /// <param name="P">Параметр P</param>
    /// <param name="Q">Параметр Q</param>
    /// <param name="k">Номер элемента</param>
    /// <param name="n">Модуль</param>
    /// <returns>Массив значений U(k), V(k) и Q^n</returns>
    public static BigInt[] LucasSequence(
        BigInt P,
        BigInt Q,
        BigInt k,
        BigInt n)
    {
        if (k._DataLength == 1 && k._Data[0] == 0)
        {
            var result = new BigInt[3];

            result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
            return result;
        }

        // Вычисление constant = b^(2k) / m
        // для редукции Барретта
        var constant = new BigInt();

        var n_len = n._DataLength << 1;
        constant._Data[n_len] = 0x00000001;
        constant._DataLength = n_len + 1;

        constant /= n;

        // Вычисление значений s и t
        var s = 0;

        for (var index = 0; index < k._DataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++, mask <<= 1, s++)
                if ((k._Data[index] & mask) != 0)
                {
                    index = k._DataLength; // выход из внешнего цикла
                    break;
                }
        }

        return LucasSequenceHelper(P, Q, k >> s, n, constant, s);
    }


    //***********************************************************************
    // Выполняет вычисление k-го элемента последовательности Лукаса
    // Подробнее об алгоритме см. ссылку [9]
    //
    // k должно быть нечётным, т.е. младший бит равен 1
    //***********************************************************************

    private static BigInt[] LucasSequenceHelper(
        BigInt P,
        BigInt Q,
        BigInt k,
        BigInt n,
        BigInt constant,
        int s)
    {
        var result = new BigInt[3];

        if ((k._Data[0] & 0x00000001) == 0)
            throw new ArgumentException("Argument k must be odd.");

        var num_of_bits = k.BitCount;
        var mask = (uint)0x1 << ((num_of_bits & 0x1F) - 1);

        var v = 2 % n;
        var q_k = 1 % n;
        var v1 = P % n;
        var u1 = q_k;
        var flag = true;

        for (var i = k._DataLength - 1; i >= 0; i--) // итерация по двоичному представлению k
        {
            while (mask != 0)
            {
                if (i == 0 && mask == 0x00000001) // последний бит
                    break;

                if ((k._Data[i] & mask) != 0) // бит установлен
                {
                    // удвоение индекса с добавлением

                    u1 = u1 * v1 % n;

                    v = (v * v1 - P * q_k) % n;
                    v1 = BarrettReduction(v1 * v1, n, constant);
                    v1 = (v1 - ((q_k * Q) << 1)) % n;

                    if (flag)
                        flag = false;
                    else
                        q_k = BarrettReduction(q_k * q_k, n, constant);

                    q_k = q_k * Q % n;
                }
                else
                {
                    // удвоение индекса
                    u1 = (u1 * v - q_k) % n;

                    v1 = (v * v1 - P * q_k) % n;
                    v = BarrettReduction(v * v, n, constant);
                    v = (v - (q_k << 1)) % n;

                    if (flag)
                    {
                        q_k = Q % n;
                        flag = false;
                    }
                    else
                        q_k = BarrettReduction(q_k * q_k, n, constant);
                }

                mask >>= 1;
            }
            mask = 0x80000000;
        }

        // на этом этапе u1 = u(n+1) и v = v(n)
        // так как последний бит всегда равен 1, нужно преобразовать u1 в u(2n+1), а v в v(2n+1)

        u1 = (u1 * v - q_k) % n;
        v = (v * v1 - P * q_k) % n;
        if (!flag)
            q_k = BarrettReduction(q_k * q_k, n, constant);
        //        else
        //            flag = false;

        q_k = q_k * Q % n;


        for (var i = 0; i < s; i++)
        {
            // удвоение индекса
            u1 = u1 * v % n;
            v = (v * v - (q_k << 1)) % n;

            q_k = BarrettReduction(q_k * q_k, n, constant);
        }

        result[0] = u1;
        result[1] = v;
        result[2] = q_k;

        return result;
    }
}
