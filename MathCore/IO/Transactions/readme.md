# Инструменты для работы с транзакциями NTFS

## Публичный API

- `NtfsTransactions` — фабрика создания транзакций
- `NtfsTransaction` — объект транзакции с методами файловых и каталоговых операций
- `NtfsTransactionOptions` — параметры создания транзакции
- `NtfsTransactionState` — состояние транзакции
- `NtfsTransactionException` — специализированная ошибка транзакционных операций

## Ограничение платформы

Транзакционные API помечены атрибутом `SupportedOSPlatform("windows")` и предназначены только для Windows

## Пример

```csharp
using MathCore.IO.Transactions;

using var tx = NtfsTransactions.Begin(new NtfsTransactionOptions
{
    Description = "Обновление файловой структуры",
    TimeoutMilliseconds = 5000,
});

tx.CreateDirectory(@"D:\Data\Temp");
tx.CopyFile(@"D:\Data\source.txt", @"D:\Data\Temp\copy.txt");
tx.MoveFile(@"D:\Data\Temp\copy.txt", @"D:\Data\result.txt", ReplaceExisting: true);

tx.Commit();
```