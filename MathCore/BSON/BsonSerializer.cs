using System.Collections;
using System.Text;

namespace MathCore.BSON;

public static class BsonSerializer
{
    public static void Serialize<T>(T obj, Stream stream)
    {
        var type = typeof(T);

        var utf8 = Encoding.UTF8;
        var writer = new BinaryWriter(stream, Encoding.UTF8);
        writer.Write(0);

        var buffer = new byte[1024];

        if (type.IsGenericType)
        {
            var generic_type = type.GetGenericTypeDefinition();

            if (generic_type == typeof(Dictionary<,>) && type.GenericTypeArguments[0] == typeof(string))
            {
                var dictionary = (IDictionary)obj;
                foreach (string value_name in dictionary.Keys) 
                    SerializeValue(value_name, dictionary[value_name], stream, ref buffer, utf8);
            }
        }

        writer.Write((byte)0x00);

        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
            writer.Write((int)stream.Length);
            stream.Seek(0, SeekOrigin.End);
        }
    }

    private static void SerializeValue(string ValueName, object Value, Stream stream, ref byte[] buffer, Encoding utf8)
    {
        switch (Value)
        {
            case string str:

                buffer[0] = 2;
                stream.Write(buffer, 0, 1);

                var str_bytes_count = utf8.GetByteCount(ValueName);
                if (buffer.Length < str_bytes_count + 1)
                    buffer = new byte[str_bytes_count + 1];

                utf8.GetBytes(ValueName, 0, ValueName.Length, buffer, 0);
                buffer[str_bytes_count + 1] = 0;
                stream.Write(buffer, 0, str_bytes_count + 1);

                str_bytes_count = utf8.GetByteCount(str);
                IntToBuffer(str_bytes_count + 1, buffer);
                stream.Write(buffer, 0, 4);

                if (buffer.Length < str_bytes_count + 1)
                    buffer = new byte[str_bytes_count + 1];

                utf8.GetBytes(str, 0, str.Length, buffer, 0);
                buffer[str_bytes_count + 1] = 0;
                stream.Write(buffer, 0, str_bytes_count + 1);

                return;
        }
    }

    private static void IntToBuffer(int value, byte[] buffer)
    {
        for (var (v, i) = ((uint)value, 0); i < 4; v >>= 8, i++)
            buffer[i] = (byte)v;
    }
}
