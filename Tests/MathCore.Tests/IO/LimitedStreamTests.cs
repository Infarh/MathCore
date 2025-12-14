using MathCore.IO;

namespace MathCore.Tests.IO;

[TestClass]
public class LimitedStreamTests
{
    private static Stream CreateSourceStream(int Length = 255)
    {
        var source = new byte[Length];
        for (var i = 0; i < Length; i++)
            source[i] = (byte)i;
        return new MemoryStream(source);
    }

    [TestMethod]
    public void Constructor_NegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        var source = CreateSourceStream();
        
        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new LimitedStream(source, -5, 10));
        
        Assert.That.Value(error.ParamName).IsEqual("Offset");
    }

    [TestMethod]
    public void Constructor_NegativeDataLength_ThrowsArgumentOutOfRangeException()
    {
        var source = CreateSourceStream();
        
        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new LimitedStream(source, 5, -10));
        
        Assert.That.Value(error.ParamName).IsEqual("DataLength");
    }

    [TestMethod]
    public void Seek_Origin_Begin_InLimit()
    {
        var source = CreateSourceStream();

        const int  stream_offset = 5;
        const int  stream_length = 10;
        const long seek          = 2;
        const long expected_seek = seek;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var seek_actual            = limited.Seek(seek, SeekOrigin.Begin);
        var actual_position        = limited.Position;
        var actual_source_position = source.Position;

        Assert.That.Value(seek_actual).IsEqual(expected_seek);
        Assert.That.Value(actual_position).IsEqual(expected_seek);
        Assert.That.Value(actual_source_position).IsEqual(stream_offset + expected_seek);
    }

    [TestMethod]
    public void Seek_Origin_Begin_LessLimit_Throw_ArgumentOutOfRangeException()
    {
        var source = CreateSourceStream();

        const int  stream_offset = 5;
        const int  stream_length = 10;
        const long offset        = -2;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => limited.Seek(offset, SeekOrigin.Begin));

        Assert.That.Value(error.Message).Contains("Выполнена попытка позиционирования до начала потока");
        Assert.That.Value(error.ParamName).IsEqual("offset");
    }

    [TestMethod]
    public void Seek_Origin_Begin_GreaterLimit()
    {
        var source = CreateSourceStream();

        const int  stream_offset            = 5;
        const int  stream_length            = 10;
        const long offset                   = 12;
        const long expected_seek            = offset;
        const long expected_source_position = stream_offset + offset;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var actual_seek            = limited.Seek(offset, SeekOrigin.Begin);
        var actual_position        = limited.Position;
        var actual_source_position = source.Position;

        Assert.That.Value(actual_seek).IsEqual(expected_seek);
        Assert.That.Value(actual_position).IsEqual(expected_seek);
        Assert.That.Value(actual_source_position).IsEqual(expected_source_position);
    }

    [TestMethod]
    public void Seek_Origin_End_InLimit()
    {
        var source = CreateSourceStream();
            
        const int  stream_offset            = 5;
        const int  stream_length            = 10;
        const long offset                   = -2;
        const long expected_position        = stream_length + offset;
        const long expected_source_position = stream_length + offset + stream_offset;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var seek_actual     = limited.Seek(offset, SeekOrigin.End);
        var actual_position = limited.Position;

        Assert.That.Value(seek_actual).IsEqual(expected_position);
        Assert.That.Value(actual_position).IsEqual(expected_position);
        Assert.That.Value(source.Position).IsEqual(expected_source_position);
    }

    [TestMethod]
    public void Seek_Origin_End_LessLimit_Throw_ArgumentOutOfRangeException()
    {
        var source = CreateSourceStream();
            
        const int  stream_offset = 5;
        const int  stream_length = 10;
        const long offset        = -12;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => limited.Seek(offset, SeekOrigin.End));

        Assert.That.Value(error.Message).Contains("Выполнена попытка позиционирования до начала потока");
        Assert.That.Value(error.ParamName).IsEqual("offset");
    }

    [TestMethod]
    public void WriteByte_WritesToCorrectPosition()
    {
        var source = new MemoryStream(new byte[20]);
        const int  stream_offset = 5;
        const int  stream_length = 10;
        const byte test_value    = 42;

        var limited = new LimitedStream(source, stream_offset, stream_length) { CanExpand = false };
        
        limited.WriteByte(test_value);
        
        Assert.That.Value(limited.Position).IsEqual(1);
        Assert.That.Value(source.Position).IsEqual(stream_offset + 1);
        Assert.That.Value(source.ToArray()[stream_offset]).IsEqual(test_value);
    }

    //[TestMethod]
    //public void Seek_Origin_End_GreaterLimit()
    //{
    //    var source = CreateSourceStream();

    //    const int stream_offset = 5;
    //    const int stream_length = 10;
    //    const long offset = 12;
    //    const long expected_seek = offset;
    //    const long expected_source_position = stream_offset + offset;

    //    var limited = new LimitedStream(source, stream_offset, stream_length);

    //    var actual_seek = limited.Seek(offset, SeekOrigin.End);
    //    var actual_position = limited.Position;
    //    var actual_source_position = source.Position;

    //    Assert.That.Value(actual_seek).IsEqual(expected_seek);
    //    Assert.That.Value(actual_position).IsEqual(expected_seek);
    //    Assert.That.Value(actual_source_position).IsEqual(expected_source_position);
    //}

    //[TestMethod]
    //public void Substream()
    //{
    //    var source = CreateSourceStream();

    //    const int offset = 5;
    //    const int length = 10;

    //    var limited = new LimitedStream(source, offset, length);

    //    var buffer = new byte[length];
    //    limited.Write(buffer);

    //    for (var i = 0; i < length; i++)
    //        Assert.That.Value(buffer[i]).IsEqual((byte)(i + offset));
    //}
}