using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Shared.Core.Tools
{
  class BinaryData
  {

    private const int CopyToBufferSize = 81920;

    /// <summary>
    /// The backing store for the <see cref="BinaryData"/> instance.
    /// </summary>
    private readonly ReadOnlyMemory<byte> _bytes;

    public BinaryData(byte[] data)
    {
      if (data == null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      _bytes = data;
    }

    public BinaryData(string data)
    {
      if (data == null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      _bytes = Encoding.UTF8.GetBytes(data);
    }

    public BinaryData(object serializable, ObjectSerializer serialiser, Type type = default)
    {
      using (var ms = new MemoryStream(CopyToBufferSize))
      {
        serialiser.Serialize( ms, serializable, type ?? serialiser?.GetType() ?? typeof(object), default);
        _bytes = ms.ToArray();
      }

    }

    public static BinaryData FromString(string data) => new BinaryData(data);

    public static BinaryData FromBytes(byte[] data) => new BinaryData(data);

    public static BinaryData FromObject(object serializable, ObjectSerializer serializer, Type type = default) => new BinaryData(serializable, serializer, type);

    public static BinaryData FromStream(Stream stream)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }


      return FromStreamAsync(stream, false).GetAwaiter().GetResult();
    }

    public static Task<BinaryData> FromStreamAsync( Stream stream, CancellationToken cancellationToken = default)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      return FromStreamAsync(stream, true, cancellationToken);
    }

    private static async Task<BinaryData> FromStreamAsync( Stream stream, bool async, CancellationToken cancellationToken = default)
    {
      int streamLength = 0;
      if (stream.CanSeek)
      {
        long longLength = stream.Length - stream.Position;
        if (longLength > int.MaxValue)
        {
          throw new ArgumentOutOfRangeException(
              nameof(stream),
              "Stream length must be less than Int32.MaxValue");
        }
        streamLength = (int)longLength;
      }

      using (MemoryStream memoryStream = stream.CanSeek ? new MemoryStream(streamLength) : new MemoryStream())
      {
        if (async)
        {
          await stream.CopyToAsync(memoryStream, CopyToBufferSize, cancellationToken).ConfigureAwait(false);
        }
        else
        {
          stream.CopyTo(memoryStream);
        }

        //return new BinaryData(memoryStream.GetBuffer().AsMemory(0, (int)memoryStream.Position));
        return new BinaryData(memoryStream.GetBuffer());
      }

    }
  }
}
