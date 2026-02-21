using System.Runtime.InteropServices;

namespace MathCore.PE.Tables;

/// <summary>Директория ресурсов (IMAGE_RESOURCE_DIRECTORY)</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct IMAGE_RESOURCE_DIRECTORY
{
    /// <summary>Размер структуры в байтах</summary>
    public const int Length = 16;

    /// <summary>Характеристики ресурса</summary>
    public uint Characteristics { get; init; }

    /// <summary>Дата и время создания ресурса</summary>
    public uint TimeDateStamp { get; init; }

    /// <summary>Старший номер версии</summary>
    public ushort MajorVersion { get; init; }

    /// <summary>Младший номер версии</summary>
    public ushort MinorVersion { get; init; }

    /// <summary>Количество именованных записей</summary>
    public ushort NumberOfNamedEntries { get; init; }

    /// <summary>Количество записей с идентификатором</summary>
    public ushort NumberOfIdEntries { get; init; }
}

/// <summary>Запись в директории ресурсов (IMAGE_RESOURCE_DIRECTORY_ENTRY)</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct IMAGE_RESOURCE_DIRECTORY_ENTRY
{
    /// <summary>Размер структуры в байтах</summary>
    public const int Length = 8;

    /// <summary>Имя или идентификатор ресурса</summary>
    public uint NameId { get; init; }

    /// <summary>RVA следующей директории или данных ресурса (с флагом 0x80000000 для поддиректорий)</summary>
    public uint OffsetToData { get; init; }

    /// <summary>Указывает, является ли смещение поддиректорией</summary>
    public bool IsSubdirectory => (OffsetToData & 0x80000000) != 0;

    /// <summary>Получает RVA поддиректории (без флага)</summary>
    public uint SubdirectoryRVA => OffsetToData & 0x7FFFFFFF;

    /// <summary>Получает RVA данных (без флага)</summary>
    public uint DataRVA => OffsetToData & 0x7FFFFFFF;
}

/// <summary>Данные ресурса (IMAGE_RESOURCE_DATA_ENTRY)</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct IMAGE_RESOURCE_DATA_ENTRY
{
    /// <summary>Размер структуры в байтах</summary>
    public const int Length = 16;

    /// <summary>RVA данных ресурса</summary>
    public uint RVA { get; init; }

    /// <summary>Размер данных ресурса в байтах</summary>
    public uint Size { get; init; }

    /// <summary>Кодовая страница</summary>
    public uint CodePage { get; init; }

    /// <summary>Зарезервировано, всегда равно 0</summary>
    public uint Reserved { get; init; }
}

/// <summary>Типы ресурсов в PE-файле</summary>
public enum ResourceType : uint
{
    /// <summary>Таблица курсоров</summary>
    CursorGroup = 14,
    /// <summary>Растровое изображение</summary>
    Bitmap = 2,
    /// <summary>Группа иконок</summary>
    IconGroup = 14,
    /// <summary>Иконка</summary>
    Icon = 3,
    /// <summary>Меню</summary>
    Menu = 4,
    /// <summary>Диалоговое окно</summary>
    Dialog = 5,
    /// <summary>Строковая таблица</summary>
    String = 6,
    /// <summary>Таблица шрифтов</summary>
    FontDir = 7,
    /// <summary>Шрифт</summary>
    Font = 8,
    /// <summary>Звуковой ресурс (WAV)</summary>
    Sound = 16,
    /// <summary>Пользовательский ресурс</summary>
    Custom = 256
}
