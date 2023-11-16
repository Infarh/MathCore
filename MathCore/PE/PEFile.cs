using System.Text;

using MathCore.PE.Headers;

namespace MathCore.PE;

public class PEFile
{
    // #define MakePtr(Type, Base, Offset) ((Type)(DWORD(Base) + (DWORD)(Offset)))
    internal static uint MakePtr(uint Base, uint Offset) => Base + Offset;

    private readonly FileInfo _File;

    public bool Exists => _File.Exists;

    public bool IsPE
    {
        get
        {
            if (!Exists) throw new FileNotFoundException("Файл не найден", _File.FullName);


            if (_File.Length < 128) return false;

            using var file = _File.OpenRead();
            var reader = new BinaryReader(file);

            if (file.ReadByte() != 'M') return false;
            if (file.ReadByte() != 'Z') return false;

            file.Seek(0, SeekOrigin.Begin);

            var magic = reader.ReadUInt16();

            if (file.Seek(0x3c, SeekOrigin.Begin) != 0x3c) return false;
            var pe_offset = reader.ReadUInt32();

            if (file.Seek(pe_offset, SeekOrigin.Begin) != pe_offset) return false;

            if (file.ReadByte() != 'P') return false;
            if (file.ReadByte() != 'E') return false;
            if (file.ReadByte() != '\0') return false;
            if (file.ReadByte() != '\0') return false;

            return true;
        }
    }

    public PEFile(string FilePath) : this(new FileInfo(FilePath)) { }

    public PEFile(FileInfo File) => _File = File;

    public Header GetHeader()
    {
        using var file = _File.OpenRead();
        var header = Header.Load(file);
        return header;
    }

    public void ReadData()
    {
        using var data = _File.OpenRead();
        var reader = new BinaryReader(data);

        var dos_magic_bytes = new byte[2];
        _ = data.Read(dos_magic_bytes, 0, 2);
        var pe_magic = Encoding.UTF8.GetString(dos_magic_bytes);

        data.Seek(0x3c, SeekOrigin.Begin);
        var pe_header_offset = reader.ReadUInt16();

        data.Seek(pe_header_offset, SeekOrigin.Begin);

        var address0 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var pe_signature_bytes = new byte[4];
        _ = data.Read(pe_signature_bytes, 0, 4);
        var pe_signature = Encoding.UTF8.GetString(pe_signature_bytes);

        var machine = reader.ReadUInt16();
        var number_of_sections = reader.ReadUInt16();

        var address1 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var time_data_stamp = reader.ReadUInt32();
        var pointer_to_symbol_table = reader.ReadUInt32();

        var address2 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var number_of_symbol_table = reader.ReadUInt32();
        var size_of_optional_header = reader.ReadUInt16();
        var characteristic = reader.ReadUInt16();

        var address3 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var magic = reader.ReadUInt16();
        var major_linker_version = reader.ReadByte();
        var minor_linker_version = reader.ReadByte();
        var size_of_code = reader.ReadUInt32();

        var address4 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var size_of_initial_data = reader.ReadUInt32();
        var size_of_uninitialized_data = reader.ReadUInt32();

        var address5 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var address_of_entry_point = reader.ReadUInt32();
        var base_code = reader.ReadUInt32();

        var address6 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var base_data = reader.ReadUInt32();

        var image_base = reader.ReadUInt32();

        var address7 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var section_alignment = reader.ReadUInt32();
        var file_alignment = reader.ReadUInt32();

        var address8 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var major_operating_system_version = reader.ReadUInt16();
        var minor_operating_system_version = reader.ReadUInt16();
        var major_image_version = reader.ReadUInt16();
        var minor_image_version = reader.ReadUInt16();

        var address9 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var major_subsystem_version = reader.ReadUInt16();
        var minor_subsystem_version = reader.ReadUInt16();
        var win32_version_value = reader.ReadUInt32();

        var address10 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var size_of_image = reader.ReadUInt32();
        var size_of_headers = reader.ReadUInt32();

        var address11 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var checksum = reader.ReadUInt32();
        var subsystem = reader.ReadUInt16();
        var dll_characteristics = reader.ReadUInt16();

        var address12 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var size_of_stack_reserve = reader.ReadUInt64();
        var size_of_stack_commit = reader.ReadUInt64();

        var address13 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var size_of_heap_reserve = reader.ReadUInt64();
        var size_of_heap_commit = reader.ReadUInt64();

        var address14 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var loader_flags = reader.ReadUInt32();
        var loader_number_of_rva_and_sizes = reader.ReadUInt32();

        var address15 = "0x" + (data.Position - pe_header_offset).ToString("X4");
        var data_directories = new (int Index, uint Address, uint Size)[16];
        for (var i = 0; i < data_directories.Length; i++)
        {
            var address = reader.ReadUInt32();
            var size = reader.ReadUInt32();
            data_directories[i] = (i, address, size);
        }
        var address16 = "0x" + (data.Position - pe_header_offset).ToString("X4");
    }
}