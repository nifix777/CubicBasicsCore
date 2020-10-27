using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text.Nodes
{
  public class BufferdReader : StreamReader
  {

    #region Constructors
    public BufferdReader(Stream stream) : base(stream)
    {
    }

    public BufferdReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks)
    {
    }

    public BufferdReader(Stream stream, Encoding encoding) : base(stream, encoding)
    {
    }

    public BufferdReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks)
    {
    }

    public BufferdReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
    {
    }

    public BufferdReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
    {
    }

    public BufferdReader(string path) : base(path)
    {
    }

    public BufferdReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks)
    {
    }

    public BufferdReader(string path, Encoding encoding) : base(path, encoding)
    {
    }

    public BufferdReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
    {
    }

    public BufferdReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
    {
    } 
    #endregion

    public bool ReadBlock(char[] buffer, int blockSize = 64)
    {
      return base.Read(buffer, 0, blockSize) > 0;
    }

    public async Task<bool> ReadBlockAsync(char[] buffer, int blockSize = 64)
    {
      return await base.ReadAsync(buffer, 0, blockSize) > 0;
    }
  }
}