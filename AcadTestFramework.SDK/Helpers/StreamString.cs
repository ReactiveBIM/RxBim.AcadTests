namespace AcadTestFramework.SDK.Helpers;

using System.IO;
using System.Text;

/// <summary>
/// Defines the data protocol for reading and writing strings on our stream
/// </summary>
public class StreamString
{
    private readonly Stream _ioStream;
    private readonly UnicodeEncoding _streamEncoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamString"/> class.
    /// </summary>
    /// <param name="ioStream">stream</param>
    public StreamString(Stream ioStream)
    {
        _ioStream = ioStream;
        _streamEncoding = new UnicodeEncoding();
    }

    /// <summary>
    /// comment
    /// </summary>
    /// <returns></returns>
    public string ReadString()
    {
        int len = 0;

        len = _ioStream.ReadByte() * 256;
        len += _ioStream.ReadByte();
        var inBuffer = new byte[len];
        _ = _ioStream.Read(inBuffer, 0, len);

        return _streamEncoding.GetString(inBuffer);
    }

    /// <summary>
    /// comment
    /// </summary>
    /// <param name="outString"></param>
    /// <returns></returns>
    public int WriteString(string outString)
    {
        var outBuffer = _streamEncoding.GetBytes(outString);
        var len = outBuffer.Length;
        if (len > ushort.MaxValue)
        {
            len = ushort.MaxValue;
        }

        _ioStream.WriteByte((byte)(len / 256));
        _ioStream.WriteByte((byte)(len & 255));
        _ioStream.Write(outBuffer, 0, len);
        _ioStream.Flush();

        return outBuffer.Length + 2;
    }
}