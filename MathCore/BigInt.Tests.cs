namespace MathCore;

public partial class BigInt
{
    //***********************************************************************
    // Проверка корректности реализации операторов /, %, * и +
    //***********************************************************************

    ////todo: Перенести в модульные тесты
    //public static void MulDivTest(int rounds)
    //{
    //    var rand = new Random();
    //    var val = new byte[64];
    //    var val2 = new byte[64];

    //    for (var count = 0; count < rounds; count++)
    //    {
    //        // генерация 2 чисел случайной длины
    //        var t1 = 0;
    //        while (t1 == 0)
    //            t1 = (int)(rand.NextDouble() * 65);

    //        var t2 = 0;
    //        while (t2 == 0)
    //            t2 = (int)(rand.NextDouble() * 65);

    //        var done = false;
    //        while (!done)
    //            for (var i = 0; i < 64; i++)
    //            {
    //                if (i < t1)
    //                    val[i] = (byte)(rand.NextDouble() * 256);
    //                else
    //                    val[i] = 0;

    //                if (val[i] != 0)
    //                    done = true;
    //            }

    //        done = false;
    //        while (!done)
    //            for (var i = 0; i < 64; i++)
    //            {
    //                val2[i] = (byte)(i < t2 ? (byte)(rand.NextDouble() * 256) : 0);

    //                if (val2[i] != 0)
    //                    done = true;
    //            }

    //        while (val[0] == 0)
    //            val[0] = (byte)(rand.NextDouble() * 256);
    //        while (val2[0] == 0)
    //            val2[0] = (byte)(rand.NextDouble() * 256);

    //        //            Console.WriteLine(count);
    //        var bn1 = new BigInteger(val, t1);
    //        var bn2 = new BigInteger(val2, t2);


    //        // Определение частного и остатка при делении первого числа на второе

    //        var bn3 = bn1 / bn2;
    //        var bn4 = bn1 % bn2;

    //        // Пересчёт числа
    //        var bn5 = bn3 * bn2 + bn4;

    //        // Убедиться, что значения совпадают
    //        if (bn5 == bn1) continue;
    //        //            Console.WriteLine("Ошибка на " + count);
    //        //            Console.WriteLine(bn1 + "\n");
    //        //            Console.WriteLine(bn2 + "\n");
    //        //            Console.WriteLine(bn3 + "\n");
    //        //            Console.WriteLine(bn4 + "\n");
    //        //            Console.WriteLine(bn5 + "\n");
    //        return;
    //    }
    //}


    //***********************************************************************
    // Проверка корректности функции модульного возведения в степень
    // с использованием RSA-шифрования и расшифрования (используются
    // заранее вычисленные ключи шифрования и расшифрования)
    //***********************************************************************
    ////todo: Перенести в модульные тесты
    //public static void RSATest(int rounds)
    //{
    //    var rand = new Random(1);
    //    var val = new byte[64];

    //    // Закрытый и открытый ключи
    //// ReSharper disable CommentTypo
    //    var bi_e = new BigInteger("a932b948feed4fb2b692609bd22164fc9edb59fae7880c" +
    //                              "c1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb4" +
    //                              "51917663d47232488f23a117fc97720f1e7", 16);
    //    var bi_d = new BigInteger("4adf2f7a89da93248509347d2ae506d683dd3a16357e85" +
    //                              "9a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60" +
    //                              "bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864" +
    //                              "dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc924" +
    //                              "70a747b8792d6a83b0092d2e5ebaf852c85cacf34278ef" +
    //                              "a99160f2f8aa7ee7214de07b7", 16);
    //    var bi_n = new BigInteger("e8e77781f36a7b3188d711c2190b560f205a52391b3479" +
    //                              "cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a" +
    //                              "73610d94ec36f17f3f46ad75e17bc1adfec99839589f45" +
    //                              "f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f014" +
    //                              "7a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e7" +
    //                              "1e921b9bd9017c16a5231af7f", 16);
    //// ReSharper restore CommentTypo

    //    Console.WriteLine("e =\n" + bi_e.ToString(10));
    //    Console.WriteLine("\nd =\n" + bi_d.ToString(10));
    //    Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

    //    for (var count = 0; count < rounds; count++)
    //    {
    //        // генерация данных случайной длины
    //        var t1 = 0;
    //        while (t1 == 0)
    //            t1 = (int)(rand.NextDouble() * 65);

    //        var done = false;
    //        while (!done)
    //            for (var i = 0; i < 64; i++)
    //            {
    //                val[i] = i < t1 ? (byte)(rand.NextDouble() * 256) : (byte)0;

    //                if (val[i] != 0)
    //                    done = true;
    //            }

    //        while (val[0] == 0)
    //            val[0] = (byte)(rand.NextDouble() * 256);

    //        Console.Write("Раунд = " + count);

    //        // шифрование и расшифрование данных
    //        var bi_data = new BigInteger(val, t1);
    //        var bi_encrypted = bi_data.ModPow(bi_e, bi_n);
    //        var bi_decrypted = bi_encrypted.ModPow(bi_d, bi_n);

    //        // сравнение
    //        if (bi_decrypted != bi_data)
    //        {
    //            Console.WriteLine("\nОшибка на раунде " + count);
    //            Console.WriteLine(bi_data + "\n");
    //            return;
    //        }
    //        Console.WriteLine(" <ПРОЙДЕНО>.");
    //    }
    //}


    //***********************************************************************
    // Проверка корректности модульного возведения в степень и
    // функции обратного по модулю с использованием RSA-шифрования
    // и расшифрования. Два псевдопростых p и q фиксированы, а ключи
    // RSA генерируются для каждого раунда тестирования
    //***********************************************************************
    ////todo: Перенести в модульные тесты
    //public static void RSATest2(int rounds)
    //{
    //    var rand = new Random();
    //    var val = new byte[64];

    //    byte[] lv_PseudoPrime1 =
    //    {
    //        0x85, 0x84, 0x64, 0xFD, 0x70, 0x6A,
    //        0x9F, 0xF0, 0x94, 0x0C, 0x3E, 0x2C,
    //        0x74, 0x34, 0x05, 0xC9, 0x55, 0xB3,
    //        0x85, 0x32, 0x98, 0x71, 0xF9, 0x41,
    //        0x21, 0x5F, 0x02, 0x9E, 0xEA, 0x56,
    //        0x8D, 0x8C, 0x44, 0xCC, 0xEE, 0xEE,
    //        0x3D, 0x2C, 0x9D, 0x2C, 0x12, 0x41,
    //        0x1E, 0xF1, 0xC5, 0x32, 0xC3, 0xAA,
    //        0x31, 0x4A, 0x52, 0xD8, 0xE8, 0xAF,
    //        0x42, 0xF4, 0x72, 0xA1, 0x2A, 0x0D,
    //        0x97, 0xB1, 0x31, 0xB3
    //    };

    //    byte[] lv_PseudoPrime2 =
    //    {
    //        0x99, 0x98, 0xCA, 0xB8, 0x5E, 0xD7,
    //        0xE5, 0xDC, 0x28, 0x5C, 0x6F, 0x0E,
    //        0x15, 0x09, 0x59, 0x6E, 0x84, 0xF3,
    //        0x81, 0xCD, 0xDE, 0x42, 0xDC, 0x93,
    //        0xC2, 0x7A, 0x62, 0xAC, 0x6C, 0xAF,
    //        0xDE, 0x74, 0xE3, 0xCB, 0x60, 0x20,
    //        0x38, 0x9C, 0x21, 0xC3, 0xDC, 0xC8,
    //        0xA2, 0x4D, 0xC6, 0x2A, 0x35, 0x7F,
    //        0xF3, 0xA9, 0xE8, 0x1D, 0x7B, 0x2C,
    //        0x78, 0xFA, 0xB8, 0x02, 0x55, 0x80,
    //        0x9B, 0xC2, 0xA5, 0xCB
    //    };


    //    var bi_p = new BigInteger(lv_PseudoPrime1);
    //    var bi_q = new BigInteger(lv_PseudoPrime2);
    //    var bi_pq = (bi_p - 1) * (bi_q - 1);
    //    var bi_n = bi_p * bi_q;

    //    for (var count = 0; count < rounds; count++)
    //    {
    //        // генерация закрытого и открытого ключей
    //        var bi_e = bi_pq.GenCoPrime(512, rand);
    //        var bi_d = bi_e.ModInverse(bi_pq);

    //        Console.WriteLine("\ne =\n" + bi_e.ToString(10));
    //        Console.WriteLine("\nd =\n" + bi_d.ToString(10));
    //        Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

    //        // генерация данных случайной длины
    //        var t1 = 0;
    //        while (t1 == 0)
    //            t1 = (int)(rand.NextDouble() * 65);

    //        var done = false;
    //        while (!done)
    //            for (var i = 0; i < 64; i++)
    //            {
    //                val[i] = (byte)(i < t1 ? (byte)(rand.NextDouble() * 256) : 0);

    //                if (val[i] != 0)
    //                    done = true;
    //            }

    //        while (val[0] == 0)
    //            val[0] = (byte)(rand.NextDouble() * 256);

    //        Console.Write("Раунд = " + count);

    //        // шифрование и расшифрование данных
    //        var bi_data = new BigInteger(val, t1);
    //        var bi_encrypted = bi_data.ModPow(bi_e, bi_n);
    //        var bi_decrypted = bi_encrypted.ModPow(bi_d, bi_n);

    //        // сравнение
    //        if (bi_decrypted != bi_data)
    //        {
    //            Console.WriteLine("\nОшибка на раунде {0}", count);
    //            Console.WriteLine("{0}\n", bi_data);
    //            return;
    //        }
    //        Console.WriteLine(" <ПРОЙДЕНО>.");
    //    }

    //}


    //***********************************************************************
    // Проверка корректности метода sqrt()
    //***********************************************************************

    ////todo: Перенести в модульные тесты
    //public static void SqrtTest(int rounds)
    //{
    //    var rand = new Random();
    //    for (var count = 0; count < rounds; count++)
    //    {
    //        // генерация данных случайной длины
    //        var t1 = 0;
    //        while (t1 == 0)
    //            t1 = (int)(rand.NextDouble() * 1024);

    //        Console.Write("Раунд = " + count);

    //        var a = new BigInteger();
    //        a.GenRandomBits(t1, rand);

    //        var b = a.Sqrt();
    //        var c = (b + 1) * (b + 1);

    //        // проверка, что b — наибольшее целое, для которого b*b <= a
    //        if (c <= a)
    //        {
    //            Console.WriteLine("\nОшибка на раунде " + count);
    //            Console.WriteLine(a + "\n");
    //            return;
    //        }
    //        Console.WriteLine(" <ПРОЙДЕНО>.");
    //    }
    //}

    ////todo: Перенести в модульные тесты
    //public static void Main(string[] args)
    //{
    //    // Известная проблема -> эти два псевдопростых числа проходят мою реализацию
    // ReSharper disable once CommentTypo
    //    // теста простоты, но не проходят тест IsProbablePrime в JDK

    //    byte[] pseudo_prime1 =
    //{
    //    0x00, 0x85, 0x84, 0x64, 0xFD, 0x70,
    //    0x6A, 0x9F, 0xF0, 0x94, 0x0C, 0x3E,
    //    0x2C, 0x74, 0x34, 0x05, 0xC9, 0x55,
    //    0xB3, 0x85, 0x32, 0x98, 0x71, 0xF9,
    //    0x41, 0x21, 0x5F, 0x02, 0x9E, 0xEA,
    //    0x56, 0x8D, 0x8C, 0x44, 0xCC, 0xEE,
    //    0xEE, 0x3D, 0x2C, 0x9D, 0x2C, 0x12,
    //    0x41, 0x1E, 0xF1, 0xC5, 0x32, 0xC3,
    //    0xAA, 0x31, 0x4A, 0x52, 0xD8, 0xE8,
    //    0xAF, 0x42, 0xF4, 0x72, 0xA1, 0x2A,
    //    0x0D, 0x97, 0xB1, 0x31, 0xB3
    //};

    //    //        byte[] pseudoPrime2 = { (byte)0x00,
    //    //                        (byte)0x99, (byte)0x98, (byte)0xCA, (byte)0xB8, (byte)0x5E, (byte)0xD7,
    //    //                        (byte)0xE5, (byte)0xDC, (byte)0x28, (byte)0x5C, (byte)0x6F, (byte)0x0E,
    //    //                        (byte)0x15, (byte)0x09, (byte)0x59, (byte)0x6E, (byte)0x84, (byte)0xF3,
    //    //                        (byte)0x81, (byte)0xCD, (byte)0xDE, (byte)0x42, (byte)0xDC, (byte)0x93,
    //    //                        (byte)0xC2, (byte)0x7A, (byte)0x62, (byte)0xAC, (byte)0x6C, (byte)0xAF,
    //    //                        (byte)0xDE, (byte)0x74, (byte)0xE3, (byte)0xCB, (byte)0x60, (byte)0x20,
    //    //                        (byte)0x38, (byte)0x9C, (byte)0x21, (byte)0xC3, (byte)0xDC, (byte)0xC8,
    //    //                        (byte)0xA2, (byte)0x4D, (byte)0xC6, (byte)0x2A, (byte)0x35, (byte)0x7F,
    //    //                        (byte)0xF3, (byte)0xA9, (byte)0xE8, (byte)0x1D, (byte)0x7B, (byte)0x2C,
    //    //                        (byte)0x78, (byte)0xFA, (byte)0xB8, (byte)0x02, (byte)0x55, (byte)0x80,
    //    //                        (byte)0x9B, (byte)0xC2, (byte)0xA5, (byte)0xCB,
    //    //                };

    //    Console.WriteLine("Список простых чисел < 2000\n---------------------");
    //    int limit = 100, count = 0;
    //    for (var i = 0; i < 2000; i++)
    //    {
    //        if (i >= limit)
    //        {
    //            Console.WriteLine();
    //            limit += 100;
    //        }

    //        var p = new BigInteger(-i);

    //        if (!p.IsProbablePrime()) continue;
    //        Console.Write(i + ", ");
    //        count++;
    //    }
    //    Console.WriteLine("\nКоличество = " + count);


    //    var x = new BigInteger(pseudo_prime1);
    //    Console.WriteLine("\n\nПроверка простоты для\n{0}\n", x);
    //    Console.WriteLine("SolovayStrassenTest(5) = {0}", x.SolovayStrassenTest(5));
    //    Console.WriteLine("RabinMillerTest(5) = {0}", x.RabinMillerTest(5));
    //    Console.WriteLine("FermatLittleTest(5) = {0}", x.FermatLittleTest(5));
    //    Console.WriteLine("IsProbablePrime() = {0}", x.IsProbablePrime());

    //    Console.Write("\nГенерация 512-битного случайного псевдопростого числа. . .");
    //    var rand = new Random();
    //    var prime = GetPseudoPrime(512, 5, rand);
    //    Console.WriteLine("\n" + prime);

    //    //int dwStart = System.Environment.TickCount;
    //    //BigInteger.MulDivTest(100000);
    //    //BigInteger.RSATest(10);
    //    //BigInteger.RSATest2(10);
    //    //Console.WriteLine(System.Environment.TickCount - dwStart);
    //}
}
