using System.IO;
using System.Text;
using System.Threading;
using Cubic.Core.Collections;

namespace Cubic.Core.Text
{
  public class LineProcessor
  {
    private readonly StreamReader _reader;

    private readonly char _lineSeperator;

    private readonly ILineParser _lineParser;
    private readonly ProgressToken _progress;

    public LineProcessor(StreamReader reader, ILineParser lineParser, ProgressToken progress, char lineSeperator = Constants.Cr)
    {
      _reader = reader;
      _lineSeperator = lineSeperator;
      _lineParser = lineParser;
      _progress = progress;
    }

    public void Parse(CancellationToken cancellation = default (CancellationToken))
    {
      var line = new StringBuilder();
      var charPool = ArrayPool<char>.Shared;
      char[] buffer = null;

      try
      {
        bool eof = false;

        while (!_reader.EndOfStream)
        {

          var readByte = _reader.Read();

          if (readByte == -1)
          {
            eof = true;
            break;
          }

          var character = (char)readByte;

          if (character == _lineSeperator)
          {
            continue;
          }

          // this line has ended
          if (character == Constants.Lf)
          {
            _progress.Inkrement();
            break;
          }

          line.Append(character);

          if (eof)
          {
            break;
          }

          buffer = charPool.Rent(line.Length);

          for (int index = 0; index < line.Length; index++)
          {
            buffer[index] = line[index];
          }

          if (_lineParser.IsLineValid(buffer))
          {
            _lineParser.ParseLine(buffer);
          }


        }

        _progress.Complete();
      }
      finally
      {
        charPool.Free(buffer);
      }
    }
  }
}