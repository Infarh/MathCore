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

        Assert.Instance.Value(seek_actual).IsEqual(expected_seek);
        Assert.Instance.Value(actual_position).IsEqual(expected_seek);
        Assert.Instance.Value(actual_source_position).IsEqual(stream_offset + expected_seek);
    }

    [TestMethod, Ignore]
    public void Seek_Origin_Begin_LessLimit_Throw_IOException()
    {
        var source = CreateSourceStream();

        const int  stream_offset = 5;
        const int  stream_length = 10;
        const long offset        = -2;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var error = Assert.ThrowsExactly<IOException>(() => limited.Seek(offset, SeekOrigin.Begin));

        Assert.Instance.Value(error.Message)
           .IsEqual("An attempt was made to move the position before the beginning of the stream.");
        Assert.Instance.Value(error.Data["offset"]).IsEqual(offset);
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

        Assert.Instance.Value(actual_seek).IsEqual(expected_seek);
        Assert.Instance.Value(actual_position).IsEqual(expected_seek);
        Assert.Instance.Value(actual_source_position).IsEqual(expected_source_position);
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

        Assert.Instance.Value(seek_actual).IsEqual(expected_position);
        Assert.Instance.Value(actual_position).IsEqual(expected_position);
        Assert.Instance.Value(source.Position).IsEqual(expected_source_position);
    }

    [TestMethod, Ignore]
    public void Seek_Origin_End_LessLimit_Throw_IOException()
    {
        var source = CreateSourceStream();
            
        const int  stream_offset = 5;
        const int  stream_length = 10;
        const long offset        = -12;

        var limited = new LimitedStream(source, stream_offset, stream_length);

        var error = Assert.ThrowsExactly<IOException>(() => limited.Seek(offset, SeekOrigin.End));

        Assert.Instance.Value(error.Message)
           .IsEqual("An attempt was made to move the position before the beginning of the stream.");
        Assert.Instance.Value(error.Data["offset"]).IsEqual(offset);
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

    //    Assert.Instance.Value(actual_seek).IsEqual(expected_seek);
    //    Assert.Instance.Value(actual_position).IsEqual(expected_seek);
    //    Assert.Instance.Value(actual_source_position).IsEqual(expected_source_position);
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
    //        Assert.Instance.Value(buffer[i]).IsEqual((byte)(i + offset));
    //}
}