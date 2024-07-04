using System.Text;

using MathCore.Hash.CRC;

namespace MathCore.IO.Compression.ZipCompression;

// https://github.com/EPPlusSoftware/EPPlus/blob/1859e7159b3a73037d61b5f05ba8d139d2644c8f/src/EPPlus/Packaging/DotNetZip/ZipCrypto.cs#L339
public class ZipCrypto
{
    private ZipCrypto() { }

    public static ZipCrypto New(string password)
    {
        if (password is not { Length: > 0 })
            throw new InvalidOperationException("Не указан пароль");

        var z = new ZipCrypto();
        z.Initialize(password);

        return z;
    }

    private byte MagicByte
    {
        get
        {
            var t = (ushort)((ushort)(_Keys[2] & 0xFFFF) | 0b10);
            return (byte)((t * (t ^ 0b1)) >> 8);
        }
    }

    public void Initialize(string password)
    {
        var password_bytes = StringToByteArray(password);
        for (var i = 0; i < password.Length; i++)
            Update(password_bytes[i]);
    }

    private void Update(byte value)
    {
        _Keys[0] = _CRC32.Compute(_Keys[0], value);
        _Keys[1] = (_Keys[1] + (byte)_Keys[0]) * 0x08088405 + 1;
        _Keys[2] = _CRC32.Compute(_Keys[2], (byte)(_Keys[1] >> 24));
    }

    private readonly uint[] _Keys = [0x12345678, 0x23456789, 0x34567890];
    private readonly CRC32 _CRC32 = new();

#if NET8_0_OR_GREATER
    private static Encoding ibm437 = Encoding.UTF8;
#else
    private static Encoding ibm437 = Encoding.GetEncoding("IBM437");
#endif
    private static Encoding utf8 = Encoding.UTF8;

    private static byte[] StringToByteArray(string value, System.Text.Encoding encoding) => encoding.GetBytes(value);

    private static byte[] StringToByteArray(string value) => StringToByteArray(value, ibm437);

    public byte[] DecryptMessage(byte[] EncryptedData, int length)
    {
        if (length > EncryptedData.NotNull().Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        var decrypted_data = new byte[length];
        for (var i = 0; i < length; i++)
        {
            var value = (byte)(EncryptedData[i] ^ MagicByte);
            Update(value);
            decrypted_data[i] = value;
        }

        return decrypted_data;
    }

    public byte[] EncryptMessage(byte[] SourceData, int length)
    {
        if (length > SourceData.NotNull().Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        var encrypted_data = new byte[length];
        for (var i = 0; i < length; i++)
        {
            var value = SourceData[i];
            encrypted_data[i] = (byte)(value ^ MagicByte);
            Update(value);
        }

        return encrypted_data;
    }
}
