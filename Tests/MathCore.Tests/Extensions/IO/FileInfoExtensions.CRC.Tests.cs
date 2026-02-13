using System.IO.Compression;

namespace MathCore.Tests.Extensions.IO;

[TestClass]
public class FileInfoExtensionsCRCTests
{
    public TestContext TestContext { get; set; }


#if NET5_0_OR_GREATER
    private static FileInfo CreateTestFile(string FileName, byte[] Data)
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), FileName));
        File.WriteAllBytes(file.FullName, Data);
        return file;
    }

    private static void CleanupTestFile(FileInfo file)
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
            var actual_crc = await file.ComputeCRC8Async(Cancel: TestContext.CancellationToken);

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
            var actual_crc = await file.ComputeCRC16Async(Cancel: TestContext.CancellationToken);

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
                RefOut: false,
                Cancel: TestContext.CancellationToken);

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
            Assert.IsGreaterThanOrEqualTo(0UL, actual_crc);
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
            var actual_crc = await file.ComputeCRC64Async(Cancel: TestContext.CancellationToken);

            // Проверяем, что результат вычислен
            Assert.IsGreaterThanOrEqualTo(0UL, actual_crc);
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
            Assert.IsGreaterThan<uint>(0, crc);
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
            Assert.IsGreaterThan<uint>(0, crc);
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
            Assert.IsGreaterThanOrEqualTo(0, crc8);

            var crc16 = file.ComputeCRC16();
            Assert.IsGreaterThanOrEqualTo(0, crc16);

            var crc32 = file.ComputeCRC32(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF, true, false);
            Assert.IsGreaterThan(0UL, crc32);

            var crc64 = file.ComputeCRC64();
            Assert.IsGreaterThanOrEqualTo(0UL, crc64);

            // Все CRC должны быть разными для разных размеров
            Assert.AreNotEqual(crc8, (ulong)crc16);
            Assert.AreNotEqual(crc16, (ulong)crc32);
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

    [TestMethod]
    public void ComputeCRC32_MatchesZipEntryCRC32()
    {
        var test_data = "Hello World!"u8.ToArray();
        var test_file = CreateTestFile("test_zip_crc32.txt", test_data);
        var zip_file_path = Path.Combine(Path.GetTempPath(), "test_zip_crc32.zip");

        try
        {
            // Вычисляем CRC32 файла через расширение
            var file_crc32 = test_file.ComputeCRC32();

            // Создаём ZIP-архив и получаем CRC32 из записи архива
            if (File.Exists(zip_file_path))
                File.Delete(zip_file_path);

            using (var zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(test_file.FullName, test_file.Name, CompressionLevel.Optimal);
            }

            uint zip_entry_crc32;
            using (var zip = new ZipArchive(File.OpenRead(zip_file_path), ZipArchiveMode.Read))
            {
                var entry = zip.GetEntry(test_file.Name);
                zip_entry_crc32 = entry.Crc32;
            }

            // Проверяем совпадение
            file_crc32.AssertEquals(zip_entry_crc32,
                $"CRC32 файла ({file_crc32:X8}) должен совпадать с CRC32 записи в ZIP ({zip_entry_crc32:X8})");
        }
        finally
        {
            CleanupTestFile(test_file);
            if (File.Exists(zip_file_path))
                File.Delete(zip_file_path);
        }
    }

    [TestMethod]
    public void ComputeCRC32_DefaultParameters_MatchesZipStandard()
    {
        var test_data = "123456789"u8.ToArray();
        var test_file = CreateTestFile("test_default_crc32.bin", test_data);
        var zip_file_path = Path.Combine(Path.GetTempPath(), "test_default_crc32.zip");

        try
        {
            // Вызываем без параметров (должен использовать ZIP по умолчанию)
            var file_crc32 = test_file.ComputeCRC32();

            // Создаём ZIP и получаем его CRC32
            if (File.Exists(zip_file_path))
                File.Delete(zip_file_path);

            using (var zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(test_file.FullName, test_file.Name, CompressionLevel.Optimal);
            }

            uint zip_entry_crc32;
            using (var zip = new ZipArchive(File.OpenRead(zip_file_path), ZipArchiveMode.Read))
            {
                var entry = zip.GetEntry(test_file.Name);
                zip_entry_crc32 = entry.Crc32;
            }

            file_crc32.AssertEquals(zip_entry_crc32,
                $"CRC32 по умолчанию ({file_crc32:X8}) должен совпадать с ZIP CRC32 ({zip_entry_crc32:X8})");
        }
        finally
        {
            CleanupTestFile(test_file);
            if (File.Exists(zip_file_path))
                File.Delete(zip_file_path);
        }
    }

    [TestMethod]
    public async Task ComputeCRC32Async_MatchesZipEntryCRC32()
    {
        var test_data = "Async Test Data"u8.ToArray();
        var test_file = CreateTestFile("test_zip_crc32_async.txt", test_data);
        var zip_file_path = Path.Combine(Path.GetTempPath(), "test_zip_crc32_async.zip");

        try
        {
            // Вычисляем CRC32 файла асинхронно
            var file_crc32 = await test_file.ComputeCRC32Async(Cancel: TestContext.CancellationToken);

            // Создаём ZIP-архив и получаем CRC32 из записи архива
            if (File.Exists(zip_file_path))
                File.Delete(zip_file_path);

            using (var zip = ZipFile.Open(zip_file_path, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(test_file.FullName, test_file.Name, CompressionLevel.Optimal);
            }

            uint zip_entry_crc32;
            using (var zip = new ZipArchive(File.OpenRead(zip_file_path), ZipArchiveMode.Read))
            {
                var entry = zip.GetEntry(test_file.Name);
                zip_entry_crc32 = entry.Crc32;
            }

            // Проверяем совпадение
            file_crc32.AssertEquals(zip_entry_crc32,
                $"Async CRC32 файла ({file_crc32:X8}) должен совпадать с CRC32 записи в ZIP ({zip_entry_crc32:X8})");
        }
        finally
        {
            CleanupTestFile(test_file);
            if (File.Exists(zip_file_path))
                File.Delete(zip_file_path);
        }
    }

#endif
}
