using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using static MathCore.PE.Headers.NT.ImageOptionalHeader.ImageDataDirectory;

namespace MathCore.PE.Headers;

public readonly partial struct NT
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ImageOptionalHeader
    {
        public const int Length = 112 + ImageDataDirectory.Length;

        /// <summary>Состояние файла изображения</summary>
        public MagicValue Magic { get; init; } // 2

        public enum MagicValue : ushort
        {
            //[Description("IMAGE_NT_OPTIONAL_HDR_MAGIC")]NT_HDR,
            /// <summary>Файл представляет собой исполняемый образ</summary>
            [Description("IMAGE_NT_OPTIONAL_HDR32_MAGIC")] NT_HDR32 = 0x10b,
            /// <summary>Файл представляет собой исполняемый образ</summary>
            [Description("IMAGE_NT_OPTIONAL_HDR64_MAGIC")] NT_HDR64 = 0x20b,
            /// <summary>Файл представляет собой образ ПЗУ</summary>
            [Description("IMAGE_ROM_OPTIONAL_HDR_MAGIC")] ROM_HDR = 0x107
        }

        /// <summary>Номер версии компоновщика</summary>
        public VersionValueByte LinkerVersion { get; init; } // 2

        [StructLayout(LayoutKind.Explicit)]
        public readonly struct VersionValueByte // 2
        {
            [FieldOffset(0)] private readonly byte _Major;
            [FieldOffset(1)] private readonly byte _Minor;

            [FieldOffset(0)] private readonly ushort _Version;

            public byte Major => _Major;
            public byte Minor => _Minor;

            public ushort Version { get => _Version; init => _Version = value; }

            public override string ToString() => $"Major:{_Major} Minor:{_Minor}";

            public static implicit operator VersionValueByte(ushort Version) => new() { Version = Version };
        }

        /// <summary>Размер раздела кода в байтах или сумма всех таких разделов, если имеется несколько разделов кода</summary>
        public uint SizeOfCode { get; init; } // 4

        /// <summary>Размер раздела инициализированных данных в байтах или сумма всех таких разделов, если имеется несколько разделов инициализированных данных</summary>
        public uint SizeOfInitializedData { get; init; } // 4

        /// <summary>Размер неинициализированного раздела данных в байтах или сумма всех таких разделов, если имеется несколько неинициализированных разделов данных</summary>
        public uint SizeOfUninitializedData { get; init; } // 4

        /// <summary>Указатель на функцию точки входа относительно базового адреса изображения</summary>
        /// <remarks>
        /// Для исполняемых файлов это начальный адрес.
        /// Для драйверов устройств это адрес функции инициализации.
        /// Функция точки входа является необязательной для DLL.
        /// Если точка входа отсутствует, этот элемент равен нулю.
        /// </remarks>
        public uint AddressOfEntryPoint { get; init; } // 4

        /// <summary>Указатель на начало раздела кода относительно основы изображения</summary>
        public uint BaseOfCode { get; init; } // 4

        /// <summary>Указатель на начало раздела данных относительно базы изображений</summary>
        public uint BaseOfData { get; init; } // 4

        /// <summary>Предпочтительный адрес первого байта изображения при его загрузке в память.</summary>
        /// <remarks>
        /// Это значение кратно 64 КБ байт.
        /// Значение по умолчанию для библиотек DLL равно <c>0x10000000</c>.
        /// Значение по умолчанию для приложений равно <c>0x00400000</c>,
        /// за исключением Windows CE, где оно равно <c>0x00010000</c>.
        /// </remarks>
        public uint ImageBase { get; init; } // 4

        /// <summary>Выравнивание разделов, загруженных в память, в байтах</summary>
        /// <remarks>
        /// Это значение должно быть больше или равно члену <see cref="FileAlignment"/>.<br/>
        /// Значением по умолчанию является размер страницы для системы.
        /// </remarks>
        public uint SectionAlignment { get; init; } // 4

        /// <summary>Выравнивание необработанных данных разделов в файле изображения в байтах</summary>
        /// <remarks>
        /// Значение должно быть степенью 2 от 512 до 64 КБ (включительно).
        /// Значение по умолчанию равно 512.
        /// Если размер элемента <see cref="SectionAlignment"/> меньше размера системной страницы,
        /// этот элемент должен быть таким же, как SectionAlignment.
        /// </remarks>
        public uint FileAlignment { get; init; } // 4

        [StructLayout(LayoutKind.Explicit)]
        public readonly struct VersionValueShort // 4
        {
            [FieldOffset(0)] private readonly short _Major;
            [FieldOffset(2)] private readonly short _Minor;

            [FieldOffset(0)] private readonly uint _Version;

            public short Major => _Major;
            public short Minor => _Minor;

            public uint Version { get => _Version; init => _Version = value; }

            public override string ToString() => $"Major:{_Major} Minor:{_Minor}";

            public static implicit operator VersionValueShort(uint Version) => new() { Version = Version };
        }

        /// <summary>Номер версии требуемой операционной системы</summary>
        public VersionValueShort OperatingSystemVersion { get; init; } // 4

        /// <summary>Номер версии изображения</summary>
        public VersionValueShort ImageVersion { get; init; } // 4

        /// <summary>Номер версии подсистемы</summary>
        public VersionValueShort SubsystemVersion { get; init; } // 4

        /// <summary>Этот элемент зарезервирован и должен быть равен 0</summary>
        public uint Win32VersionValue { get; init; } // 4

        /// <summary>Размер изображения в байтах, включая все заголовки</summary>
        /// <remarks>Должно быть кратно <see cref="SectionAlignment"/></remarks>
        public uint SizeOfImage { get; init; } // 4

        /// <summary>
        /// Общий размер следующих элементов, округленный до кратного значения, указанного в элементе <see cref="FileAlignment"/><br/>
        /// - <see cref="DOS.lfanew"/><br/>
        /// - подпись размером 4 байта<br/>
        /// - размер <see cref="ImageFileHeader"/> = 20 байт<br/>
        /// - размер необязательного заголовка<br/>
        /// - размер всех заголовков разделов<br/>
        /// </summary>
        public uint SizeOfHeaders { get; init; } // 4

        /// <summary>Контрольная сумма файла изображения</summary>
        /// <remarks>
        /// Во время загрузки проверяются следующие файлы:<br/>
        /// - все драйверы, любая библиотека DLL,<br/>
        /// - загруженная во время загрузки, и любая библиотека DLL,<br/>
        /// - загруженная в критический системный процесс.
        /// </remarks>
        public uint CheckSum { get; init; } // 4

        /// <summary>Подсистема, необходимая для запуска этого образа</summary>
        public SubsystemValue Subsystem { get; init; } // 2

        public enum SubsystemValue : ushort
        {
            /// <summary>Неизвестная подсистема</summary>
            [Description("IMAGE_SUBSYSTEM_UNKNOWN")]
            Unknown = 0,
            /// <summary>Подсистема не требуется (драйверы устройств и собственные системные процессы)</summary>
            [Description("IMAGE_SUBSYSTEM_NATIVE")]
            Native = 1,
            /// <summary>Подсистема графического пользовательского интерфейса (GUI) Windows</summary>
            [Description("IMAGE_SUBSYSTEM_WINDOWS_GUI")]
            WindowsGUI = 2,
            /// <summary>Подсистема пользовательского интерфейса (CUI) в символьном режиме Windows</summary>
            [Description("IMAGE_SUBSYSTEM_WINDOWS_CUI")]
            WindowsCUI = 3,
            /// <summary>Подсистема CUI OS / 2</summary>
            [Description("IMAGE_SUBSYSTEM_OS2_CUI")]
            OS2CUI = 5,
            /// <summary>Подсистема POSIX CUI</summary>
            [Description("IMAGE_SUBSYSTEM_POSIX_CUI")]
            PosixCUI = 7,
            /// <summary>Система Windows CE</summary>
            [Description("IMAGE_SUBSYSTEM_WINDOWS_CE_GUI")]
            WindowsCEGUI = 9,
            /// <summary>Приложение с расширяемым интерфейсом встроенного программного обеспечения (EFI)</summary>
            [Description("IMAGE_SUBSYSTEM_EFI_APPLICATION")]
            EFIApplication = 10,
            /// <summary>Драйвер EFI со службами загрузки</summary>
            [Description("IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER")]
            EFIBootServiceDriver = 11,
            /// <summary>Драйвер EFI со службами времени выполнения</summary>
            [Description("IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER")]
            EFIRuntimeDriver = 12,
            /// <summary>Образ EFI ROM</summary>
            [Description("IMAGE_SUBSYSTEM_EFI_ROM")]
            EFIROM = 13,
            /// <summary>Система Xbox</summary>
            [Description("IMAGE_SUBSYSTEM_XBOX")]
            Xbox = 14,
            /// <summary>Загрузчик приложения</summary>
            [Description("IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION")]
            WindowsBootApplication = 16
        }

        /// <summary>Характеристики DLL образа</summary>
        public DllCharacteristicsValue DllCharacteristics { get; init; } // 2

        [Flags]
        public enum DllCharacteristicsValue : ushort
        {
            Reserved1 = 0x0001,
            Reserved2 = 0x0002,
            Reserved4 = 0x0004,
            Reserved8 = 0x0008,
            /// <summary>ASLR с 64-разрядным адресным пространством</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_HIGH_ENTROPY_VA")]
            HighEntropyVA = 0x0020,
            /// <summary>Библиотека DLL может быть перемещена во время загрузки</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE")]
            DynamicBase = 0x0040,
            /// <summary>Проверка целостности кода выполняется принудительно</summary>
            /// <remarks>
            /// Если вы установили этот флаг и раздел содержит только неинициализированные данные,
            /// установите для элемента PointerToRawData элемента <see cref="SectionHeader"/>
            /// для этого раздела значение ноль;
            /// в противном случае загрузка изображения не удастся,
            /// поскольку невозможно проверить цифровую подпись.
            /// </remarks>
            [Description("IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY")]
            ForceIntegrity = 0x0080,
            /// <summary>Изображение совместимо с предотвращением выполнения данных (DEP)</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_NX_COMPAT")]
            NxCompat = 0x0100,
            /// <summary>Изображение поддерживает изоляцию, но не должно быть изолировано</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_NO_ISOLATION")]
            NoIsolation = 0x0200,
            /// <summary>Изображение не использует структурированную обработку исключений (SEH)</summary>
            /// <remarks>В этом изображении нельзя вызывать обработчики</remarks>
            [Description("IMAGE_DLL_CHARACTERISTICS_NO_SEH")]
            NoSeh = 0x0400,
            /// <summary>Не привязывайте изображение</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_NO_BIND")]
            NoBind = 0x0800,
            /// <summary>Изображение должно выполняться в AppContainer</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_APPCONTAINER")]
            AppContainer = 0x1000,
            /// <summary>Драйвер WDM</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_WDM_DRIVER")]
            WdmDriver = 0x2000,
            /// <summary>Изображение поддерживает защиту потока управления</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_GUARD_CF")]
            GuardCF = 0x4000,
            /// <summary>Изображение поддерживает сервер терминалов</summary>
            [Description("IMAGE_DLL_CHARACTERISTICS_TERMINAL_SERVER_AWARE")]
            ServerAware = 0x8000,
        }

        /// <summary>Количество байтов, которые необходимо зарезервировать для стека</summary>
        /// <remarks>
        /// Во время загрузки выделяется только память, указанная членом <see cref="SizeOfStack"/>.<see cref="SizeOf.Commit"/>;
        /// остальная часть становится доступной по одной странице за раз,
        /// пока не будет достигнут этот резервный размер.
        /// </remarks>
        public SizeOf SizeOfStack { get; init; } // 16

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct SizeOf // 16
        {
            /// <summary>Количество байт зарезервировано</summary>
            public ulong Reserve { get; init; } // 8

            /// <summary>Количество байт для фиксации</summary>
            public ulong Commit { get; init; } // 8

            public override string ToString() => $"Reserve:{Reserve} Commit:{Commit}";
        }

        /// <summary>Количество байтов, которые необходимо зарезервировать для локальной кучи</summary>
        /// <remarks>
        /// Во время загрузки выделяется только память, указанная членом <see cref="SizeOfHeap"/>.<see cref="SizeOf.Commit"/>;
        /// остальная часть становится доступной по одной странице за раз,
        /// пока не будет достигнут этот резервный размер.
        /// </remarks>
        public SizeOf SizeOfHeap { get; init; } // 16

        /// <summary>Этот элемент устарел</summary>
        public uint LoaderFlags { get; init; } // 4

        /// <summary>Количество записей каталога в оставшейся части необязательного заголовка</summary>
        /// <remarks>Каждая запись описывает местоположение и размер</remarks>
        public uint NumberOfRvaAndSizes { get; init; } // 4

        public ImageDataDirectory DataDirectory { get; init; } // 128

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct ImageDataDirectory : IEnumerable<ImageDataDirectoryValue> // 128
        {
            public const int Length = 16 * ImageDataDirectoryValue.Length;

            /// <summary>Каталог экспортируемых объектов</summary>
            public ImageDataDirectoryValue Export { get; init; } // 0
            /// <summary>Каталог импортируемых объектов</summary>
            public ImageDataDirectoryValue Import { get; init; } // 1
            /// <summary>Каталог ресурсов</summary>
            public ImageDataDirectoryValue Resource { get; init; } // 2
            /// <summary>Каталог исключений</summary>
            public ImageDataDirectoryValue Exception { get; init; } // 3
            /// <summary>Каталог безопасности</summary>
            public ImageDataDirectoryValue Security { get; init; } // 4
            /// <summary>Таблица переадресации</summary>
            public ImageDataDirectoryValue BaseReloc { get; init; } // 5
            /// <summary>Отладочный каталог</summary>
            public ImageDataDirectoryValue Debug { get; init; } // 6
            /// <summary>Строки описания</summary>
            public ImageDataDirectoryValue Copyright { get; init; } // 7
            public ImageDataDirectoryValue GlobalPtr { get; init; } // 8
            /// <summary>Каталог TLS (Thread local storage - локальная память потоков)</summary>
            public ImageDataDirectoryValue TLS { get; init; } // 9
            public ImageDataDirectoryValue LoadConfig { get; init; } // 10
            public ImageDataDirectoryValue BoundImport { get; init; } // 11
            public ImageDataDirectoryValue IAT { get; init; } // 12
            public ImageDataDirectoryValue DelayImport { get; init; } // 13
            /// <summary>Информация COM-объектов</summary>
            public ImageDataDirectoryValue Descriptor { get; init; } // 14
            public ImageDataDirectoryValue Reserved { get; init; } // 15

            public IEnumerable<ImageDataDirectoryValue> Values
            {
                get
                {
                    yield return Export;
                    yield return Import;
                    yield return Resource;
                    yield return Exception;
                    yield return Security;
                    yield return BaseReloc;
                    yield return Debug;
                    yield return Copyright;
                    yield return GlobalPtr;
                    yield return TLS;
                    yield return LoadConfig;
                    yield return BoundImport;
                    yield return IAT;
                    yield return DelayImport;
                    yield return Descriptor;
                    yield return Reserved;
                }
            }

            /// <summary>IMAGE_DATA_DIRECTORY</summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct ImageDataDirectoryValue // 8
            {
                public const int Length = 8;

                /// <summary>Относительный виртуальный адрес таблицы</summary>
                public uint VirtualAddress { get; init; } // 4

                /// <summary>Размер таблицы в байтах</summary>
                public uint Size { get; init; } // 4

                public override string ToString() => $"VirtualAddress:{VirtualAddress}; Size:{Size}";

            }

            public IEnumerator<ImageDataDirectoryValue> GetEnumerator() => Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}