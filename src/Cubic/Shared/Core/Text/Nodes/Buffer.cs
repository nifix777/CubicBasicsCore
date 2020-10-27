using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cubic.Core.Collections;

/// <summary>
/// 
/// </summary>
namespace Cubic.Core.Text.Nodes
{
  public class BufferNode
  {
    private int _lineIndex;
    private int[] _lineStarts;

    public BufferNode(char[] buffer)
    {
      Data = new char[buffer.Length];
      buffer.FastCopy(Data);
      _lineStarts = new int[0];
      ScanLines();
    }
    public BufferNode(long size = 64)
    {
      Data = new char[size];
    }
    public char[] Data { get; }
    public string Value => new string(Data);

    public int[] LineStarts => _lineStarts;

    private void ScanLines()
    {
      for (int i = 0; i < Data.Length; i++)
      {
        char second = i + 1 >= Data.Length - 1 ? char.MinValue : Data[i + 1];
        string chars = new string(new []{Data[i], second});
        if (chars == Constants.Lf.ToString() || chars == Environment.NewLine )
        {
          ArrayExtensions.Append(ref _lineStarts, _lineIndex, i + 2, 0);
          _lineIndex++;
        }
      }
    }

  }

  public class PieceTable
  {
    private BufferNode[] _bufferNodes;

    private int index;
    public PieceTable()
    {
      _bufferNodes = new BufferNode[64];
    }

    public BufferNode[] BufferNodes
    {
      get { return _bufferNodes; }
    }

    public void Append(BufferNode node)
    {
      if (_bufferNodes.Length ==  index +1)
      {
        Array.Resize(ref _bufferNodes, BufferNodes.Length + 64);
      }


      _bufferNodes[index] = node;
      index++;
    }

    public Node RootNode { get; }

  }

  public class Node
  {
    public int BufferIndex { get; set; }

    public int Start { get; set; }

    public int End { get; set; }
  }



  public class PieceTableProvider : IDisposable
  {
    private Stream _stream;

    private ArrayPool<char> _pool; 
    public PieceTableProvider(string file, ArrayPool<char> arrayPool ) : this(new FileStream(file, FileMode.Open), arrayPool )
    {
      
    }

    public PieceTableProvider(Stream stream, ArrayPool<char> arrayPool)
    {
      _stream = stream;
      _pool = arrayPool;
    }

    public async Task<PieceTable> ReadAsync(Encoding encoding, int tableSize = 64)
    {
      PieceTable table = new PieceTable();

      using (var bufferdReader = new BufferdReader(_stream, encoding))
      {
        var buffer = _pool.Rent(tableSize);

        while (await bufferdReader.ReadBlockAsync(buffer, tableSize))
        {
          var bufferNode = new BufferNode(buffer);
          _pool.Free(buffer);
          table.Append(bufferNode);
        }
      }

      return table;
    }

    public PieceTable Read(Encoding encoding, int tableSize = 64)
    {
    
      PieceTable table = new PieceTable();

      using (var bufferdReader = new BufferdReader(_stream, encoding))
      {

        var buffer = _pool.Rent(tableSize);
        while (bufferdReader.ReadBlock(buffer, tableSize))
        {

          var bufferNode = new BufferNode(buffer);
          _pool.Free(buffer);
          table.Append(bufferNode);
          buffer = _pool.Rent(tableSize);
        }

        _pool.Free(buffer);
      }

      return table;
    }

    public void Dispose()
    {
      _stream?.Dispose();
      _stream = null;
    }
  }
}