namespace MathCore.PE.Headers;

public struct Header
{
    public static Header Load(Stream DataStream)
    {
        var dos = DOS.Load(DataStream);
        if (dos.Magic != DOS.CorrectMagicValue)
            throw new FormatException($"Некорректная сигнатура файла. Первыми двумя байтами должно быть 0x{DOS.CorrectMagicValue:X4} (MZ)");

        if (DataStream.Seek(dos.lfanew, SeekOrigin.Begin) != dos.lfanew)
            throw new InvalidOperationException();

        var nt = NT.Load(DataStream);
        if (nt.Signature != NT.CorrectSignatureValue)
            throw new FormatException($"Некорректный формат файла. По смещению 0x{dos.lfanew:X2} должно находиться значение 0x50450000 (PE\\0\\0)");

        var sections = DataStream.EnumStructures<SectionHeader>()
           .Take(nt.FileHeader.NumberOfSections)
           .ToArray();

        return new()
        {
            DOS = dos,
            NT = nt,
            Sections = sections
        };
    }

    public DOS DOS { get; init; }

    public NT NT { get; init; }

    public IReadOnlyList<SectionHeader> Sections { get; init; }
}