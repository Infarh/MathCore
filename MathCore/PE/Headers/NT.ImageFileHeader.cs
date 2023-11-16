using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MathCore.PE.Headers;

public readonly partial struct NT
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ImageFileHeader
    {
        public const int Length = 20;

        public MachineType Machine { get; init; }

        public enum MachineType : ushort
        {
            /// <summary>x86</summary>
            I386 = 0x014c,
            /// <summary>Intel Itanium</summary>
            IA64 = 0x0200,
            /// <summary>x64</summary>
            AMD64 = 0x8664
        }

        /// <summary>Количество секций</summary>
        /// <remarks>
        /// Это значение указывает на размер таблицы разделов, которая сразу следует за заголовками.<br/>
        /// Обратите внимание, что загрузчик Windows ограничивает количество разделов 96.
        /// </remarks>
        public ushort NumberOfSections { get; init; }

        /// <summary>
        /// Младшие 32 бита временной метки изображения.
        /// Это представляет собой дату и время, когда изображение было создано компоновщиком.
        /// Значение представлено в количестве секунд, прошедших с полуночи
        /// (00:00:00) 1 января 1970 года по UTC, согласно системным часам.
        /// </summary>
        public uint TimeDateStamp { get; init; }

        public DateTime TimeDate => new DateTime(1970, 1, 1).AddSeconds(TimeDateStamp);

        /// <summary>Смещение таблицы символов в байтах или ноль, если таблица символов COFF не существует</summary>
        public uint PointerToSymbolTable { get; init; }

        /// <summary>Количество символов в таблице символов</summary>
        public uint NumberOfSymbols { get; init; }

        /// <summary>Размер необязательного заголовка в байтах. Это значение должно быть равно 0 для объектных файлов.</summary>
        public ushort SizeOfOptionalHeader { get; init; }

        /// <summary>Характеристики изображения</summary>
        public CharacteristicsValue Characteristics { get; init; }

        /// <summary>Характеристики изображения</summary>
        [Flags]
        public enum CharacteristicsValue : ushort
        {
            /// <summary>
            /// Информация о перемещении была удалена из файла.
            /// Файл должен быть загружен по его предпочтительному базовому адресу.
            /// Если базовый адрес недоступен, загрузчик сообщает об ошибке.
            /// </summary>
            [Description("IMAGE_FILE_RELOCS_STRIPPED")]
            RelocsStripped = 0x0001,
            /// <summary>Файл является исполняемым (нет неразрешенных внешних ссылок)</summary>
            [Description("IMAGE_FILE_EXECUTABLE_IMAGE")]
            ExecutableImage = 0x0002,
            /// <summary>Номера строк COFF были удалены из файла</summary>
            [Description("IMAGE_FILE_LINE_NUMS_STRIPPED")]
            LineNumsStripped = 0x0004,
            /// <summary>Записи таблицы символов COFF были удалены из файла</summary>
            [Description("IMAGE_FILE_LOCAL_SYMS_STRIPPED")]
            LocalSymsStripped = 0x0008,
            /// <summary>Агрессивно обрезайте рабочий набор (устарело)</summary>
            [Description("IMAGE_FILE_AGGRESIVE_WS_TRIM")]
            AggresiveWsTrim = 0x0010,
            /// <summary>Приложение может обрабатывать адреса размером более 2 ГБ</summary>
            [Description("IMAGE_FILE_LARGE_ADDRESS_AWARE")]
            LargeAddressAware = 0x0020,
            /// <summary>Байты слова меняются местами (устарело)</summary>
            [Description("IMAGE_FILE_BYTES_REVERSED_LO")]
            BytesReversedLo = 0x0080,
            /// <summary>Компьютер поддерживает 32-разрядные слова</summary>
            [Description("IMAGE_FILE_32BIT_MACHINE")]
            Is32BitMachine = 0x0100,
            /// <summary>Отладочная информация была удалена и сохранена отдельно в другом файле</summary>
            [Description("IMAGE_FILE_DEBUG_STRIPPED")]
            DebugStripped = 0x0200,
            /// <summary>Если изображение находится на съемном носителе, скопируйте его и запустите из файла подкачки</summary>
            [Description("IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP")]
            RemovableRunFromSwap = 0x0400,
            /// <summary>Если изображение находится в сети, скопируйте его и запустите из файла подкачки</summary>
            [Description("IMAGE_FILE_NET_RUN_FROM_SWAP")]
            NetRunFromSwap = 0x0800,
            /// <summary>Изображение представляет собой системный файл</summary>
            [Description("IMAGE_FILE_SYSTEM")]
            System = 0x1000,
            /// <summary>
            /// Изображение представляет собой DLL-файл.
            /// Хотя это исполняемый файл, его нельзя запустить напрямую.
            /// </summary>
            [Description("IMAGE_FILE_DLL")]
            Dll = 0x2000,
            /// <summary>Файл должен запускаться только на однопроцессорном компьютере</summary>
            [Description("IMAGE_FILE_UP_SYSTEM_ONLY")]
            UpSystemOnly = 0x4000,
            /// <summary>Байты слова меняются местами (устарело)</summary>
            [Description("IMAGE_FILE_BYTES_REVERSED_HI")]
            BytesReversedHi = 0x8000
        }
    }
}