#nullable enable
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MathCore.Algorithms.Strings;

public static class Encryptor
{
    public static byte[] GenerateRandomSalt() => RandomNumberGenerator.GetBytes(32);

    private static readonly byte[] aes_KEY_IV = { 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58, 0x56, 0x2e };

    public static string Encryptaes(string Str, string Key, Encoding? Encoding = null)
    {
        var key = (Encoding ??= Encoding.UTF8).GetBytes(Key);
        var str_bytes = Encoding.GetBytes(Str);

        using var aes = Aes.Create();
        //Генерируем соль
        aes.GenerateIV();
        aes.Key = key;
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        var result = new byte[16];
        encryptor.TransformBlock(key, 0, key.Length, result, 0);

        throw new NotImplementedException();
    }

    public static void EncryptRSA(string Str, string Key, Encoding? Encoding = null)
    {
        Encoding ??= Encoding.UTF8;

        var csp = new RSACryptoServiceProvider(2048);

        var private_key = csp.ExportParameters(true);
        var public_key = csp.ExportParameters(false);

        var public_key_string = JsonSerializer.Serialize(public_key);

        public_key = JsonSerializer.Deserialize<RSAParameters>(public_key_string);

        csp = new RSACryptoServiceProvider();
        csp.ImportParameters(public_key);

        var str_bytes = Encoding.GetBytes(Str);

        var encrypted_bytes = csp.Encrypt(str_bytes, false);
        var encrypted_str = Convert.ToBase64String(encrypted_bytes);

        var received_encrypted_bytes = Convert.FromBase64String(encrypted_str);

        var receiver_csp = new RSACryptoServiceProvider();
        receiver_csp.ImportParameters(private_key);

        var decrypted_bytes = csp.Decrypt(received_encrypted_bytes, false);
        var decrypted_str = Encoding.Unicode.GetString(decrypted_bytes);
    }

    private static void FileEncrypt(string inputFile, string password)
    {
        var salt = GenerateRandomSalt();

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Padding = PaddingMode.PKCS7;

        var password_bytes = Encoding.UTF8.GetBytes(password);
        var key = new Rfc2898DeriveBytes(password_bytes, salt, 50000, HashAlgorithmName.SHA1);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);
        aes.Mode = CipherMode.CFB;

        try
        {
            using var destination_stream = new FileStream($"{inputFile}.aes", FileMode.Create);
            destination_stream.Write(salt, 0, salt.Length);

            using var crypto_stream = new CryptoStream(destination_stream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var source_stream = new FileStream(inputFile, FileMode.Open);

            source_stream.CopyTo(crypto_stream);
        }
        catch (Exception error)
        {
            Console.WriteLine("Error: {0}", error.Message);
        }
    }

    private static void FileDecrypt(string InputFile, string OutputFile, string Password)
    {
        var salt = new byte[32];

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;

        var password_bytes = Encoding.UTF8.GetBytes(Password);

        using var source_file = new FileStream(InputFile, FileMode.Open);
        _ = source_file.Read(salt, 0, salt.Length);

        var key = new Rfc2898DeriveBytes(password_bytes, salt, 50000);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CFB;

        try
        {
            var crypto_stream = new CryptoStream(source_file, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var result_stream = new FileStream(OutputFile, FileMode.Create);
            crypto_stream.CopyTo(result_stream);
        }
        catch (CryptographicException error)
        {
            Console.WriteLine("CryptographicException error: {0}", error.Message);
        }
        catch (Exception error)
        {
            Console.WriteLine("Error: {0}", error.Message);
        }
    }
}
