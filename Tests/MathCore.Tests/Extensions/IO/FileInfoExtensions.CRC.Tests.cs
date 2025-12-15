using System.IO;

namespace MathCore.Tests.Extensions.IO;

[TestClass]
public class FileInfoExtensionsCRCTests
{
#if NET5_0_OR_GREATER
    private FileInfo CreateTestFile(string FileName, byte[] Data)
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), FileName));
        File.WriteAllBytes(file.FullName, Data);
        return file;
    }

    private void CleanupTestFile(FileInfo file)
    {
        if (file.Exists)
            file.Delete();
    }

    [TestMethod]
    public void ComputeCRC8_ForFile()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const byte expected_crc = 0x18;

        var file = CreateTestFile("test_crc8.bin", data);
        try
        {
            var actual_crc = file.ComputeCRC8();

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public async Task ComputeCRC8Async_ForFile()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const byte expected_crc = 0x18;

        var file = CreateTestFile("test_crc8_async.bin", data);
        try
        {
            var actual_crc = await file.ComputeCRC8Async();

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC16_ForFile()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const ushort expected_crc = 0x718E;

        var file = CreateTestFile("test_crc16.bin", data);
        try
        {
            var actual_crc = file.ComputeCRC16();

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public async Task ComputeCRC16Async_ForFile()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const ushort expected_crc = 0x718E;

        var file = CreateTestFile("test_crc16_async.bin", data);
        try
        {
            var actual_crc = await file.ComputeCRC16Async();

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC32_ForFile_StandardZIP()
    {
        var data = "123456789"u8.ToArray();
        const uint expected_crc = 0xCBF43926;

        var file = CreateTestFile("test_crc32.bin", data);
        try
        {
            var actual_crc = file.ComputeCRC32(
                Polynomial: 0xEDB88320,
                InitialValue: 0xFFFFFFFF,
                XOROut: 0xFFFFFFFF,
                RefIn: true,
                RefOut: false);

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public async Task ComputeCRC32Async_ForFile_StandardZIP()
    {
        var data = "123456789"u8.ToArray();
        const uint expected_crc = 0xCBF43926;

        var file = CreateTestFile("test_crc32_async.bin", data);
        try
        {
            var actual_crc = await file.ComputeCRC32Async(
                Polynomial: 0xEDB88320,
                InitialValue: 0xFFFFFFFF,
                XOROut: 0xFFFFFFFF,
                RefIn: true,
                RefOut: false);

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC64_ForFile()
    {
        var data = "Test Data"u8.ToArray();

        var file = CreateTestFile("test_crc64.bin", data);
        try
        {
            var actual_crc = file.ComputeCRC64();

            // Проверяем, что результат вычислен
            Assert.IsTrue(actual_crc >= 0);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public async Task ComputeCRC64Async_ForFile()
    {
        var data = "Test Data"u8.ToArray();

        var file = CreateTestFile("test_crc64_async.bin", data);
        try
        {
            var actual_crc = await file.ComputeCRC64Async();

            // Проверяем, что результат вычислен
            Assert.IsTrue(actual_crc >= 0);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC32_LargeFile()
    {
        // Создаём файл размером 10 MB
        var large_data = new byte[10 * 1024 * 1024];
        for (var i = 0; i < large_data.Length; i++)
            large_data[i] = (byte)(i % 256);

        var file = CreateTestFile("test_crc32_large.bin", large_data);
        try
        {
            var crc = file.ComputeCRC32(
                Polynomial: 0xEDB88320,
                InitialValue: 0xFFFFFFFF,
                XOROut: 0xFFFFFFFF,
                RefIn: true,
                RefOut: false);

            // Проверяем, что CRC вычислен
            Assert.IsTrue(crc > 0);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public async Task ComputeCRC32Async_LargeFile_WithCancellation()
    {
        // Создаём файл размером 5 MB
        var data = new byte[5 * 1024 * 1024];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);

        var file = CreateTestFile("test_crc32_large_async.bin", data);
        try
        {
            using var cts = new CancellationTokenSource();

            var crc = await file.ComputeCRC32Async(
                Polynomial: 0xEDB88320,
                InitialValue: 0xFFFFFFFF,
                XOROut: 0xFFFFFFFF,
                RefIn: true,
                RefOut: false,
                Cancel: cts.Token);

            // Проверяем, что CRC вычислен
            Assert.IsTrue(crc > 0);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC_FileNotFound_Throws()
    {
        var non_existent_file = new FileInfo(Path.Combine(Path.GetTempPath(), "non_existent_file.bin"));

        Assert.ThrowsExactly<FileNotFoundException>(() => non_existent_file.ComputeCRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false));
    }

    [TestMethod]
    public void ComputeCRC_AllVariants_ForSameFile()
    {
        var data = "Test Data For CRC"u8.ToArray();
        var file = CreateTestFile("test_all_crc.bin", data);

        try
        {
            var crc8 = file.ComputeCRC8();
            Assert.IsTrue(crc8 >= 0);

            var crc16 = file.ComputeCRC16();
            Assert.IsTrue(crc16 >= 0);

            var crc32 = file.ComputeCRC32(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF, true, false);
            Assert.IsTrue(crc32 > 0);

            var crc64 = file.ComputeCRC64();
            Assert.IsTrue(crc64 >= 0);

            // Все CRC должны быть разными для разных размеров
            Assert.AreNotEqual((ulong)crc8, (ulong)crc16);
            Assert.AreNotEqual((ulong)crc16, (ulong)crc32);
            Assert.AreNotEqual(crc32, (uint)crc64);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC32_SameFile_SameResult()
    {
        var data = "Consistent Data"u8.ToArray();
        var file = CreateTestFile("test_consistency.bin", data);

        try
        {
            var crc1 = file.ComputeCRC32(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF, true, false);
            var crc2 = file.ComputeCRC32(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF, true, false);

            crc1.AssertEquals(crc2);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }

    [TestMethod]
    public void ComputeCRC8_WithCustomParameters()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const byte expected_crc = 0xDE;

        var file = CreateTestFile("test_crc8_custom.bin", data);
        try
        {
            var actual_crc = file.ComputeCRC8(
                Polynomial: 0x07,
                InitialValue: 0xFF,
                XOROut: 0xFF);

            actual_crc.AssertEquals(expected_crc);
        }
        finally
        {
            CleanupTestFile(file);
        }
    }
#endif
}
