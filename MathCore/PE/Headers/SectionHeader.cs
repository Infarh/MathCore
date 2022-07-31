using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MathCore.PE.Headers;

/// <summary>IMAGE_SECTION_HEADER</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct SectionHeader
{
    public const int Length = 44;

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SectionNameValue
    {
        private readonly byte _Name0;
        private readonly byte _Name1;
        private readonly byte _Name2;
        private readonly byte _Name3;
        private readonly byte _Name4;
        private readonly byte _Name5;
        private readonly byte _Name6;
        private readonly byte _Name7;

        public byte[] Bytes => new[] { _Name0, _Name1, _Name2, _Name3, _Name4, _Name5, _Name6, _Name7 };

        public char[] Chars => new[] { (char)_Name0, (char)_Name1, (char)_Name2, (char)_Name3, (char)_Name4, (char)_Name5, (char)_Name6, (char)_Name7 };

        public string Value => new(Chars);

        public override string ToString()
        {
            var chars = Chars;
            var result = new StringBuilder(chars.Length * 2);
            foreach (var c in chars)
                if (c == '\0')
                    result.Append("\\0");
                else
                    result.Append(c);

            return result.ToString();
        }

        public static implicit operator string(SectionNameValue v) => v.Value;
    }

    //private readonly byte _Name0;
    //private readonly byte _Name1;
    //private readonly byte _Name2;
    //private readonly byte _Name3;
    //private readonly byte _Name4;
    //private readonly byte _Name5;
    //private readonly byte _Name6;
    //private readonly byte _Name7;

    /// <summary>8-байтовая строка UTF-8 с заполнением нулем</summary>
    /// <remarks>
    /// Завершающий нулевой символ отсутствует,
    /// если длина строки составляет ровно восемь символов.
    /// Для более длинных имен этот элемент содержит косую черту (/),
    /// за которой следует представление десятичного числа в формате ASCII,
    /// которое является смещением в таблице строк.
    /// Исполняемые образы не используют таблицу строк
    /// и не поддерживают имена разделов длиной более восьми символов.
    /// </remarks>
    //public string Name => Encoding.UTF8.GetString(new[] { _Name0, _Name1, _Name2, _Name3, _Name4, _Name5, _Name6, _Name7 });
    public SectionNameValue Name { get; init; }

    private readonly uint _PhysicalAddressOrVirtualSize;

    /// <summary>Адрес файла</summary>
    public uint PhysicalAddress => _PhysicalAddressOrVirtualSize;

    /// <summary>Общий размер раздела при загрузке в память, в байтах</summary>
    /// <remarks>
    /// Если это значение больше, чем элемент <see cref="SizeOfRawData"/>, раздел заполняется нулями.
    /// Это поле действительно только для исполняемых образов и должно быть установлено в 0
    /// для объектных файлов
    /// </remarks>
    public uint VirtualSize => _PhysicalAddressOrVirtualSize;

    /// <summary>Адрес первого байта раздела при загрузке в память относительно базы изображений</summary>
    /// <remarks>Для объектных файлов это адрес первого байта перед применением перемещения</remarks>
    public uint VirtualAddress { get; init; }

    /// <summary>Размер инициализированных данных на диске в байтах</summary>
    /// <remarks>
    /// Это значение должно быть кратно члену <see cref="NT.ImageOptionalHeader.FileAlignment"/>
    /// структуры <see cref="NT.ImageOptionalHeader"/>.
    /// Если это значение меньше, чем элемент <see cref="VirtualSize"/>,
    /// оставшаяся часть раздела заполняется нулями.
    /// Если раздел содержит только неинициализированные данные, член равен нулю.
    /// </remarks>
    public uint SizeOfRawData { get; init; }

    /// <summary>Указатель файла на первую страницу в файле COFF</summary>
    /// <remarks>
    /// Это значение должно быть кратно члену <see cref="NT.ImageOptionalHeader.FileAlignment"/>
    /// структуры <see cref="NT.ImageOptionalHeader"/>.
    /// Если раздел содержит только неинициализированные данные,
    /// установите для этого элемента значение ноль.
    /// </remarks>
    public uint PointerToRawData { get; init; }

    /// <summary>Указатель файла на начало записей перемещения для раздела</summary>
    /// <remarks>Если перемещений нет, это значение равно нулю</remarks>
    public uint PointerToRelocations { get; init; }

    /// <summary>Указатель файла на начало записей с номерами строк для раздела</summary>
    /// <remarks>Если номера строк COFF отсутствуют, это значение равно нулю</remarks>
    public uint PointerToLineNumbers { get; init; }

    /// <summary>Количество записей перемещения для раздела</summary>
    /// <remarks>Это значение равно нулю для исполняемых образов</remarks>
    public ushort NumberOfRelocations { get; init; }

    /// <summary>Количество записей с номерами строк для раздела</summary>
    public ushort NumberOfLineNumbers { get; init; }

    /// <summary>Характеристики изображения</summary>
    public CharacteristicsValue Characteristics { get; init; }

    /// <summary>Характеристики изображения</summary>
    [Flags]
    public enum CharacteristicsValue : uint
    {
        Reserved0000 = 0x0000_0000,
        Reserved0001 = 0x0000_0001,
        Reserved0002 = 0x0000_0002,
        Reserved0004 = 0x0000_0004,
        /// <summary>Раздел не следует дополнять до следующей границы</summary>
        /// <remarks> Этот флаг устарел и заменяется на <see cref="Align1Bytes"/></remarks>
        [Description("IMAGE_SCN_TYPE_NO_PAD")]
        TypeNoPad = 0x0000_0008,

        Reserved0010 = 0x0000_0010,

        /// <summary>Раздел содержит исполняемый код</summary>
        [Description("IMAGE_SCN_CNT_CODE")]
        CntCode = 0x0000_0020,
        /// <summary>Раздел содержит инициализированные данные</summary>
        [Description("IMAGE_SCN_CNT_INITIALIZED_DATA")]
        CntInitializedData = 0x0000_0040,
        /// <summary>Раздел содержит неинициализированные данные</summary>
        [Description("IMAGE_SCN_CNT_UNINITIALIZED_DATA")]
        CntUninitializedData = 0x0000_0080,
        /// <summary>Зарезервировано</summary>
        [Description("IMAGE_SCN_LNK_OTHER")]
        LnkOther = 0x0000_0100,
        /// <summary>Раздел содержит комментарии или другую информацию</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_LNK_INFO")]
        LnkInfo = 0x0000_0200,

        Reserved0400 = 0x0000_0400,

        /// <summary>Раздел не станет частью изображения</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_LNK_REMOVE")]
        LnkRemove = 0x0000_0800,
        /// <summary>Раздел содержит данные COMDAT</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_LNK_COMDAT")]
        LnkComdat = 0x0000_1000,

        Reserved2000 = 0x0000_2000,

        /// <summary>Сбросьте биты обработки спекулятивных исключений в записях TLB для этого раздела</summary>
        [Description("IMAGE_SCN_NO_DEFER_SPEC_EXC")]
        NoDeferSpecExc = 0x0000_4000,
        /// <summary>Раздел содержит данные, на которые ссылается глобальный указатель</summary>
        [Description("IMAGE_SCN_GPREL")]
        Gprel = 0x0000_8000,

        Reserved10000 = 0x0001_0000,

        /// <summary>Зарезервировано</summary>
        [Description("IMAGE_SCN_MEM_PURGEABLE")]
        MemPurgeable = 0x0002_0000,
        /// <summary>Зарезервировано</summary>
        [Description("IMAGE_SCN_MEM_LOCKED")]
        MemLocked = 0x0004_0000,
        /// <summary>Зарезервировано</summary>
        [Description("IMAGE_SCN_MEM_PRELOAD")]
        MemPreload = 0x0008_0000,

        /// <summary>Выровнять данные по границе в 1 байт</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_1BYTES")]
        Align1Bytes = 0x0010_0000,
        /// <summary>Выровняйте данные по 2-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_2BYTES")]
        Align2Bytes = 0x0020_0000,
        /// <summary>Выровняйте данные по 4-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_4BYTES")]
        Align4Bytes = 0x0030_0000,
        /// <summary>Выровняйте данные по 8-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_8BYTES")]
        Align8Bytes = 0x0040_0000,
        /// <summary>Выровняйте данные по 16-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_16BYTES")]
        Align16Bytes = 0x0050_0000,
        /// <summary>Выровняйте данные по 32-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_32BYTES")]
        Align32Bytes = 0x0060_0000,
        /// <summary>Выровняйте данные по 64-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_64BYTES")]
        Align64Bytes = 0x0070_0000,
        /// <summary>Выровняйте данные по 128-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_128BYTES")]
        Align128Bytes = 0x0080_0000,
        /// <summary>Выровняйте данные по 256-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_256BYTES")]
        Align256Bytes = 0x0090_0000,
        /// <summary>Выровняйте данные по 512-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_512BYTES")]
        Align512Bytes = 0x00A0_0000,
        /// <summary>Выровняйте данные по 1024-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_1024BYTES")]
        Align1024Bytes = 0x00B0_0000,
        /// <summary>Выровняйте данные по 2048-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_2048BYTES")]
        Align2048Bytes = 0x00C0_0000,
        /// <summary>Выровняйте данные по 4096-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_4096BYTES")]
        Align4096Bytes = 0x00D0_0000,
        /// <summary>Выровняйте данные по 8192-байтовой границе</summary>
        /// <remarks>Это действительно только для объектных файлов</remarks>
        [Description("IMAGE_SCN_ALIGN_8192BYTES")]
        Align8192Bytes = 0x00E0_0000,

        /// <summary>Раздел содержит расширенные перемещения</summary>
        /// <remarks>
        /// Количество перемещений для раздела превышает 16 бит,
        /// зарезервированных для него в заголовке раздела.
        /// Если поле <see cref="NumberOfRelocations"/> в заголовке раздела равно <c>0xFFFF</c>,
        /// фактическое количество перемещений сохраняется в поле <see cref="VirtualAddress"/>
        /// первого перемещения.
        /// Это ошибка, если установлено значение <see cref="LnkNrelocOvfl"/>
        /// и в разделе меньше перемещений <c>0xFFFF</c>.
        /// </remarks>
        [Description("IMAGE_SCN_LNK_NRELOC_OVFL")]
        LnkNrelocOvfl = 0x0100_0000,
        /// <summary>Раздел может быть удален по мере необходимости</summary>
        [Description("IMAGE_SCN_MEM_DISCARDABLE")]
        MemDiscardable = 0x0200_0000,
        /// <summary>Раздел не может быть кэширован</summary>
        [Description("IMAGE_SCN_MEM_NOT_CACHED")]
        MemNotCached = 0x0400_0000,
        /// <summary>Раздел не может быть выгружен</summary>
        [Description("IMAGE_SCN_MEM_NOT_PAGED")]
        MemNotPaged = 0x0800_0000,
        /// <summary>Раздел может быть общим в памяти</summary>
        [Description("IMAGE_SCN_MEM_SHARED")]
        MemShared = 0x1000_0000,
        /// <summary>Раздел может быть выполнен в виде кода</summary>
        [Description("IMAGE_SCN_MEM_EXECUTE")]
        MemExecute = 0x2000_0000,
        /// <summary>Раздел можно прочитать</summary>
        [Description("IMAGE_SCN_MEM_READ")]
        MemRead = 0x4000_0000,
        /// <summary>В раздел можно записать</summary>
        [Description("IMAGE_SCN_MEM_WRITE")]
        MemWrite = 0x8000_0000,
    }

    public override string ToString() => $"Section name: {Name}";
}