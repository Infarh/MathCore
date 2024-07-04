using System.Text;

namespace MathCore.IO.Compression.TarCompression;

public partial class TarGZip
{
    /// <summary>Элемент архива</summary>
    public class Entry
    {
        /// <summary>Размер буфера чтения должен соответствовать размеру блока архива</summary>
        const int __BufferLength = 512;

        /// <summary>Значения флага типа элемента архива</summary>
        public enum EntryTypeFlag : byte
        {
            /// <summary>Элемент является обычным файлом</summary>
            RegularFile = 0,
            /// <summary>Элемент является ссылкой</summary>
            Link = 1,
            Reversed = 2,
            CaracterSpecial = 3,
            BlockSpecial = 4,
            /// <summary>Элемент является директорией</summary>
            Directory = 5,
            FIFOSpecial = 6,
            Reserved = 7,
            ExtendeHeader = (byte)'x',        //120
            GlobalExtendedHeader = (byte)'g', //103
        }

        /// <summary>Поток данных элемента архива в памяти</summary>
        private MemoryStream _Data;

        /// <summary>Объект чтения данных архива</summary>
        private BinaryReader _Reader;

        /// <summary>Имя элемента архива</summary>
        public string Name { get; }

        /// <summary>Режим сжатия</summary>
        public ulong Mode { get; }

        public ulong UserID { get; }

        public ulong GroupID { get; }

        public int Size { get; } = 0;

        public DateTime Time { get; }

        public ulong CheckSum { get; }

        public EntryTypeFlag TypeFlag { get; }

        public string LinkName { get; }

        public string Magic { get; }

        public ushort Version { get; }

        public string UserName { get; }

        public string GroupName { get; }

        public ulong DevMajor { get; }

        public ulong DevMinor { get; }

        public string Prefix { get; }

        public MemoryStream Data => ReadData();

#if !NET8_0_OR_GREATER
        internal Entry(BinaryReader reader)
        {
            _Reader = reader;
            var buffer = new byte[__BufferLength];
            if (reader.Read(buffer, 0, __BufferLength) != __BufferLength)
                throw new FormatException("Ошибка формата заголовка элемента архива");

            var encoding = Encoding.ASCII;
            Name = encoding.GetString(buffer, 0, 100).TrimEnd('\0');
            Mode = BitConverter.ToUInt64(buffer, 100);
            UserID = BitConverter.ToUInt64(buffer, 108);
            GroupID = BitConverter.ToUInt64(buffer, 116);
            var size = int.Parse(encoding.GetString(buffer, 124, 12).TrimEnd('\0')).FromOctalBase();
            Size = size;
            // Time
            CheckSum = BitConverter.ToUInt64(buffer, 148);
            TypeFlag = (EntryTypeFlag)buffer[156];
            LinkName = encoding.GetString(buffer, 157, 100);
            var magic = encoding.GetString(buffer, 257, 6);

            if (magic != "ustar\x00") throw new FormatException("Ошибка магического числа заголовка элемента архива");

            Magic = magic;
            Version = BitConverter.ToUInt16(buffer, 263);
            UserName = encoding.GetString(buffer, 265, 32).Trim('\0');
            GroupName = encoding.GetString(buffer, 297, 32).Trim('\0');
            DevMajor = BitConverter.ToUInt64(buffer, 329);
            DevMinor = BitConverter.ToUInt64(buffer, 337);
            Prefix = encoding.GetString(buffer, 345, 155).Trim('\0');
        }
#else
        internal Entry(BinaryReader reader)
        {
            _Reader = reader;
            Span<byte> buffer = stackalloc byte[__BufferLength];
            if (reader.Read(buffer) != __BufferLength)
                throw new FormatException("Ошибка формата заголовка элемента архива");

            var encoding = Encoding.ASCII;
            Name = encoding.GetString(buffer[..100].TrimEnd((byte)0));

            var buffer_ulong = buffer[100..].Cast<ulong>();
            Mode = buffer_ulong[0];
            UserID = buffer_ulong[1];
            GroupID = buffer_ulong[2];

            Size = int.Parse(encoding.GetString(buffer.Slice(124, 12).TrimEnd((byte)0))).FromOctalBase();

            buffer_ulong = buffer[148..].Cast<ulong>();
            // Time
            CheckSum = buffer_ulong[0];
            TypeFlag = (EntryTypeFlag)buffer[156];
            LinkName = encoding.GetString(buffer.Slice(157, 100).TrimEnd((byte)0));

            var magic = encoding.GetString(buffer.Slice(257, 6));
            if(buffer.Slice(257, 6) != "ustar\0"u8) throw new FormatException("Ошибка магического числа заголовка элемента архива");
            //if (magic != "ustar\x00") throw new FormatException("Ошибка магического числа заголовка элемента архива");

            Magic = magic;

            var buffer_ushort = buffer[263..].Cast<ushort>();
            Version = buffer_ushort[0];
            UserName = encoding.GetString(buffer.Slice(265, 32).TrimEnd((byte)0));
            GroupName = encoding.GetString(buffer.Slice(297, 32).TrimEnd((byte)0));


            buffer_ulong = buffer[329..].Cast<ulong>();
            DevMajor = buffer_ulong[0];
            DevMinor = buffer_ulong[1];
            Prefix = encoding.GetString(buffer.Slice(345, 155).TrimEnd((byte)0));
        }
#endif

        public MemoryStream ReadData()
        {
            if (_Data != null || _Reader is null) return _Data;

            var size = Size;
            var data = new MemoryStream(size);
            var buffer = new byte[__BufferLength];
            while (size >= __BufferLength)
            {
                if (_Reader.Read(buffer, 0, __BufferLength) != __BufferLength)
                    throw new FormatException("Неожиданный конец архива");

                data.Write(buffer, 0, __BufferLength);
                size -= __BufferLength;
            }

            if (size > 0)
            {
                if (_Reader.Read(buffer, 0, __BufferLength) != __BufferLength)
                    throw new FormatException("Неожиданный конец архива");

                data.Write(buffer, 0, size);
            }
            data.Seek(0, SeekOrigin.Begin);
            _Reader = null;
            return _Data = data;
        }

        public void Skip()
        {
            if (_Data != null || _Reader is null) return;

            var size = Size;
            var buffer = new byte[__BufferLength];
            while (size >= __BufferLength)
            {
                if (_Reader.Read(buffer, 0, __BufferLength) != __BufferLength)
                    throw new FormatException("Неожиданный конец архива");

                size -= __BufferLength;
            }

            if (size > 0)
                if (_Reader.Read(buffer, 0, __BufferLength) != __BufferLength)
                    throw new FormatException("Неожиданный конец архива");

            _Reader = null;
        }
    }
}