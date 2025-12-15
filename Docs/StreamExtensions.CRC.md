# Методы-расширения для вычисления CRC из потоков

Добавлены методы-расширения для класса `Stream`, позволяющие вычислять CRC различных размеров непосредственно из потока данных без загрузки всех данных в память.

## Методы

### CRC-8

```csharp
// Синхронное вычисление CRC-8
byte crc = stream.ComputeCRC8(
    Polynomial: 0x07,      // Полином (по умолчанию 0x07)
    InitialValue: 0x00,    // Начальное значение (по умолчанию 0x00)
    XOROut: 0x00,          // XOR с результатом (по умолчанию 0x00)
    RefIn: false,          // Отражение входных байтов (по умолчанию false)
    RefOut: false          // Отражение выходного значения (по умолчанию false)
);

// Асинхронное вычисление CRC-8
byte crc = await stream.ComputeCRC8Async(
    Polynomial: 0x07,
    InitialValue: 0x00,
    XOROut: 0x00,
    RefIn: false,
    RefOut: false,
    Cancel: cancellationToken
);
```

### CRC-16

```csharp
// Синхронное вычисление CRC-16
ushort crc = stream.ComputeCRC16(
    Polynomial: 0x1021,    // Полином XMODEM (по умолчанию)
    InitialValue: 0x0000,  // Начальное значение (по умолчанию 0x0000)
    XOROut: 0x0000,        // XOR с результатом (по умолчанию 0x0000)
    RefIn: false,          // Отражение входных байтов (по умолчанию false)
    RefOut: false          // Отражение выходного значения (по умолчанию false)
);

// Асинхронное вычисление CRC-16
ushort crc = await stream.ComputeCRC16Async(
    Polynomial: 0x1021,
    InitialValue: 0x0000,
    XOROut: 0x0000,
    RefIn: false,
    RefOut: false,
    Cancel: cancellationToken
);
```

### CRC-32

```csharp
// Синхронное вычисление CRC-32 (стандартный ZIP/ISO-HDLC)
uint crc = stream.ComputeCRC32(
    Polynomial: 0xEDB88320,   // Отражённый полином для ZIP
    InitialValue: 0xFFFFFFFF, // Начальное значение (по умолчанию 0xFFFFFFFF)
    XOROut: 0xFFFFFFFF,       // XOR с результатом (по умолчанию 0xFFFFFFFF)
    RefIn: true,              // Отражение входных байтов
    RefOut: false             // Отражение выходного значения
);

// Асинхронное вычисление CRC-32
uint crc = await stream.ComputeCRC32Async(
    Polynomial: 0xEDB88320,
    InitialValue: 0xFFFFFFFF,
    XOROut: 0xFFFFFFFF,
    RefIn: true,
    RefOut: false,
    Cancel: cancellationToken
);
```

### CRC-64

```csharp
// Синхронное вычисление CRC-64
ulong crc = stream.ComputeCRC64(
    Polynomial: 0x000000000000001B,  // Полином ISO3309 (по умолчанию)
    InitialValue: 0x0000000000000000, // Начальное значение (по умолчанию 0x00)
    XOROut: 0x0000000000000000,       // XOR с результатом (по умолчанию 0x00)
    RefIn: false,                     // Отражение входных байтов (по умолчанию false)
    RefOut: false                     // Отражение выходного значения (по умолчанию false)
);

// Асинхронное вычисление CRC-64
ulong crc = await stream.ComputeCRC64Async(
    Polynomial: 0x000000000000001B,
    InitialValue: 0x0000000000000000,
    XOROut: 0x0000000000000000,
    RefIn: false,
    RefOut: false,
    Cancel: cancellationToken
);
```

## Примеры использования

### Пример 1: Вычисление CRC-32 для файла

```csharp
using System.IO;

// Синхронный вариант
using (var fileStream = File.OpenRead("file.dat"))
{
    uint crc32 = fileStream.ComputeCRC32(
        Polynomial: 0xEDB88320,
        InitialValue: 0xFFFFFFFF,
        XOROut: 0xFFFFFFFF,
        RefIn: true,
        RefOut: false);
    
    Console.WriteLine($"CRC-32: 0x{crc32:X8}");
}

// Асинхронный вариант
await using (var fileStream = File.OpenRead("file.dat"))
{
    uint crc32 = await fileStream.ComputeCRC32Async(
        Polynomial: 0xEDB88320,
        InitialValue: 0xFFFFFFFF,
        XOROut: 0xFFFFFFFF,
        RefIn: true,
        RefOut: false);
    
    Console.WriteLine($"CRC-32: 0x{crc32:X8}");
}
```

### Пример 2: Вычисление CRC-16 с дефолтными параметрами

```csharp
using (var stream = new MemoryStream(data))
{
    ushort crc16 = stream.ComputeCRC16(); // Используются значения по умолчанию
    Console.WriteLine($"CRC-16: 0x{crc16:X4}");
}
```

### Пример 3: Вычисление нескольких CRC для одного потока

```csharp
var data = File.ReadAllBytes("file.dat");

using (var stream = new MemoryStream(data))
{
    var crc8 = stream.ComputeCRC8();
    stream.Position = 0; // Сброс позиции потока
    
    var crc16 = stream.ComputeCRC16();
    stream.Position = 0;
    
    var crc32 = stream.ComputeCRC32(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF, true, false);
    
    Console.WriteLine($"CRC-8:  0x{crc8:X2}");
    Console.WriteLine($"CRC-16: 0x{crc16:X4}");
    Console.WriteLine($"CRC-32: 0x{crc32:X8}");
}
```

### Пример 4: Асинхронное вычисление с отменой

```csharp
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(30)); // Таймаут 30 секунд

try
{
    await using var fileStream = File.OpenRead("large_file.dat");
    uint crc32 = await fileStream.ComputeCRC32Async(
        Polynomial: 0xEDB88320,
        InitialValue: 0xFFFFFFFF,
        XOROut: 0xFFFFFFFF,
        RefIn: true,
        RefOut: false,
        Cancel: cts.Token);
    
    Console.WriteLine($"CRC-32: 0x{crc32:X8}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Операция отменена по таймауту");
}
```

## Важные замечания

1. **Позиция потока**: После вычисления CRC позиция потока изменится (поток будет прочитан до конца). Если требуется повторное чтение, необходимо сбросить позицию потока с помощью `stream.Position = 0` или `stream.Seek(0, SeekOrigin.Begin)`.

2. **Память**: Методы читают данные блоками (буферами по 8192 байта), поэтому могут обрабатывать потоки любого размера без загрузки всех данных в память.

3. **Производительность**: Асинхронные методы рекомендуется использовать для больших файлов или сетевых потоков, чтобы не блокировать поток выполнения.

4. **Доступность**: Методы доступны только для .NET 5.0 и выше (проверка через `#if NET5_0_OR_GREATER`).

5. **Параметры по умолчанию**: Все методы имеют параметры по умолчанию, соответствующие наиболее распространённым стандартам:
   - CRC-8: полином 0x07 (стандартный CRC-8)
   - CRC-16: полином 0x1021 (XMODEM)
   - CRC-32: полином 0x04C11DB7 (стандартный, но для ZIP нужен отражённый вариант)
   - CRC-64: полином 0x000000000000001B (ISO3309)

## Стандартные конфигурации

### CRC-32 (ZIP/ISO-HDLC)
```csharp
uint crc = stream.ComputeCRC32(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF, true, false);
```

### CRC-16 (XMODEM)
```csharp
ushort crc = stream.ComputeCRC16(0x1021, 0x0000, 0x0000, false, false);
```

### CRC-8 (стандартный)
```csharp
byte crc = stream.ComputeCRC8(0x07, 0x00, 0x00, false, false);
```
