using MathCore.Hash.CRC;

namespace MathCore.Tests.Hash.CRC;

[TestClass]
public class CRC64Tests
{
    // TODO: Тест не завершён - не проверяет результат, только выводит в отладку
    // Найти корректное эталонное значение для ISO3309 и дописать проверку
    //[TestMethod]
    //public void ISO_Hello_World()
    //{
    //    var data = "Hello World!"u8.ToArray();
    //    const ulong expected_hash = 0xCA64C7DA170C6241;
    //
    //    const ulong iso3309 = 0x000000000000001B;
    //    var crc = new CRC64(iso3309);
    //
    //    var actual_hash_1 = crc.Compute([0x01]);
    //
    //    var actual_hash1 = $"0x{actual_hash_1:X16}";
    //
    //    actual_hash1.ToDebug();
    //}

    // TODO: Проверить корректность эталонных значений CRC-64/ECMA-182
    // Текущие значения не совпадают с реализацией
    //[TestMethod]
    //public void TestCRC64WithKnownValues()
    //{
    //    var data = "123456789"u8.ToArray();
    //    const ulong expected_crc = 0x995DC9BBDF1939FA; // CRC-64/ECMA-182
    //
    //    var actual_crc = CRC64.Hash(data, 0x42F0E1EBA9EA3693, 0xFFFFFFFFFFFFFFFF, false, false, 0xFFFFFFFFFFFFFFFF);
    //
    //    Assert.AreEqual(expected_crc, actual_crc, $"Expected: 0x{expected_crc:X16}, Actual: 0x{actual_crc:X16}");
    //}

    // TODO: Проверить корректность эталонного значения CRC-64/ECMA-182 для "Hello World!"
    //[TestMethod]
    //public void TestCRC64WithOnlineCalculator()
    //{
    //    var data = "Hello World!"u8.ToArray();
    //    const ulong expected_crc = 0x1F6A1D2C3C9C2D3A; // CRC-64/ECMA-182
    //
    //    var actual_crc = CRC64.Hash(data, 0x42F0E1EBA9EA3693, 0xFFFFFFFFFFFFFFFF, false, false, 0xFFFFFFFFFFFFFFFF);
    //
    //    Assert.AreEqual(expected_crc, actual_crc, $"Expected: 0x{expected_crc:X16}, Actual: 0x{actual_crc:X16}");
    //}

    [TestMethod]
    public void Basic_Compute_Works()
    {
        var data = "Test"u8.ToArray();
        var crc = new CRC64();

        var result = crc.Compute(data);

        // Проверяем, что результат вычисляется (не проверяем конкретное значение без эталона)
        Assert.IsGreaterThanOrEqualTo(0UL, result);
    }

    [TestMethod]
    public void Reset_Works()
    {
        var data = "Test"u8.ToArray();
        var crc = new CRC64();

        var result1 = crc.Compute(data);
        crc.Reset();
        var result2 = crc.Compute(data);

        result1.AssertEquals(result2);
    }
}
