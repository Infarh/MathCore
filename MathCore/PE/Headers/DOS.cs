using System.Runtime.InteropServices;

namespace MathCore.PE.Headers;

[StructLayout(LayoutKind.Sequential)]
/// <summary>DOS-заголовок 64-битного исполняемого файла PE</summary>
public readonly struct DOS
{
    /// <summary>Чтение DOS-заголовка из потока</summary>
    /// <param name="DataStream">Поток данных</param>
    /// <returns>Прочитанный DOS-заголовок</returns>
    public static DOS Load(Stream DataStream)
    {
        var dos = DataStream.ReadStructure<DOS>();
        return dos;
    }

    /// <summary>Размер заголовка 62 байта</summary>
    public const int Length = 62;

    /// <summary>Должно быть равно 'M', 'Z' = 4D5A</summary>
    public ushort Magic { get; init; } // 0x00

    /// <summary>Корректное значение магического числа (MZ)</summary>
    public const ushort CorrectMagicValue = 'M' | 'Z' << 8;

    /// <summary>Строковое представление магического числа</summary>
    public string MagicStr => $"{(char)(Magic & 0xFF)}{(char)(Magic >> 8)}";

    /// <summary>Количество байт в последней странице файла</summary>
    public ushort cblp { get; init; } // 0x02

    /// <summary>Количество страниц в файле</summary>
    public ushort cp { get; init; } // 0x04

    /// <summary>Количество элементов таблицы перемещения</summary>
    public ushort crlc { get; init; } // 0x06

    /// <summary>Размер заголовка в параграфах</summary>
    public ushort cparhdr { get; init; } // 0x08

    /// <summary>Минимальное количество дополнительных параграфов</summary>
    public ushort MinAlloc { get; init; } // 0x0a

    /// <summary>Максимальное количество дополнительных параграфов</summary>
    public ushort MaxAlloc { get; init; } // 0x0c

    /// <summary>Начальное значение регистра SS</summary>
    public ushort ss { get; init; } // 0x0e

    /// <summary>Начальное значение регистра SP</summary>
    public ushort sp { get; init; } // 0x10

    /// <summary>Контрольная сумма</summary>
    public ushort csum { get; init; } // 0x12

    /// <summary>Начальное значение регистра IP</summary>
    public ushort ip { get; init; } // 0x14

    /// <summary>Начальное значение регистра CS</summary>
    public ushort cs { get; init; } // 0x16

    /// <summary>Смещение таблицы перемещения</summary>
    public ushort lfarlc { get; init; } // 0x18

    /// <summary>Номер оверлея</summary>
    public ushort ovno { get; init; } // 0x1a

    /// <summary>Зарезервированное слово 0</summary>
    public ushort res0 { get; init; } // 0x1c

    /// <summary>Зарезервированное слово 1</summary>
    public ushort res1 { get; init; } // 0x1e

    /// <summary>Зарезервированное слово 2</summary>
    public ushort res2 { get; init; } // 0x20

    /// <summary>Зарезервированное слово 3</summary>
    public ushort res3 { get; init; } // 0x22

    /// <summary>Идентификатор OEM</summary>
    public ushort OemId { get; init; } // 0x24

    /// <summary>Информация OEM</summary>
    public ushort OemInfo { get; init; } // 0x26

    /// <summary>Зарезервированное слово 20</summary>
    public ushort res20 { get; init; } // 0x28
    /// <summary>Зарезервированное слово 21</summary>
    public ushort res21 { get; init; } // 0x2a
    /// <summary>Зарезервированное слово 22</summary>
    public ushort res22 { get; init; } // 0x2c
    /// <summary>Зарезервированное слово 23</summary>
    public ushort res23 { get; init; } // 0x2e
    /// <summary>Зарезервированное слово 24</summary>
    public ushort res24 { get; init; } // 0x30
    /// <summary>Зарезервированное слово 25</summary>
    public ushort res25 { get; init; } // 0x32
    /// <summary>Зарезервированное слово 26</summary>
    public ushort res26 { get; init; } // 0x34
    /// <summary>Зарезервированное слово 27</summary>
    public ushort res27 { get; init; } // 0x36
    /// <summary>Зарезервированное слово 28</summary>
    public ushort res28 { get; init; } // 0x38
    /// <summary>Зарезервированное слово 29</summary>
    public ushort res29 { get; init; } // 0x3a

    /// <summary>Смещение PE-заголовка</summary>
    public ushort lfanew { get; init; } // 0x3c
}