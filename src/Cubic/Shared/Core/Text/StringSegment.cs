using System;
using System.Collections;
using System.Collections.Generic;
using Cubic.Core.Collections;

namespace Cubic.Core.Text
{
  /// <summary>
  /// Represents an allocation free readonly Segment of a string. Internally a refernece to the original string is hold.
  /// </summary>
  /// <seealso cref="char" />
  /// <seealso cref="StringSegment" />
  /// <seealso cref="string" />
  /// <remarks> <b>NOT</b> compliant to <see cref="Segment{T}"/> because <see cref="string"/> does not implement <see cref="IReadOnlyList{T}"/> </remarks>
  public struct StringSegment : IEnumerable<char>, IEquatable<StringSegment>, IEquatable<string>, ISegment<char>
  {
    public static StringSegment Empty = new StringSegment(string.Empty, 0, 0);

    private readonly string _source;

    private int _offset;

    private int _length;
    public StringSegment(string source, int offset = 0, int length = 1)
    {
      _source = source;
      _offset = offset;
      _length = length;
    }
    public int Offset => _offset;
    public int Length => _length;
    public int End => Offset + Length;
    public bool IsEmpty => this.Length == 0;
    public bool Contains(int position)
    {
      return unchecked(( uint ) ( position - Offset ) < ( uint ) Length);
    }

    public bool Contains(ISegment<char> segment)
    {
      return segment.Offset >= this.Offset && segment.End <= this.End;
    }

    public bool OverlapsWith(ISegment<char> segment)
    {
      int overlapStart = Math.Max( Offset , segment.Offset );
      int overlapEnd = Math.Min( this.End , segment.End );

      return overlapStart < overlapEnd;
    }

    public char this[int index]
    {
      get
      {
        var internalIndex = Offset + index;
        if(internalIndex > (Offset + Length)) throw new IndexOutOfRangeException();
        return _source[internalIndex];
      }
    }
    public IEnumerator<char> GetEnumerator()
    {
      return new StringSegmentEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Equals(StringSegment other)
    {
      if ( this.IsNull() || other.IsNull() ) return false;
      if (this.Offset != other.Offset) return false;

      if (this.Length != other.Length) return false;  

      for (int i = 0; i < Length -1; i++)
      {
        if (this[i] != other[i]) return false;
      }

      return true;
    }

    public int IndexOf(char c)
    {
      for (int i = 0; i < Length; i++)
      {
        if (c.Equals(this[i])) return i;
      }

      return -1;
    }

    public bool Equals(string other)
    {
      if ( this.IsNull() || other.IsNull() ) return false;
      if ( this.Length != other.Length ) return false;

      for ( int i = 0 ; i <= Length - 1 ; i++ )
      {
        if ( this[i] != other[i] ) return false;
      }

      return true;
    }

    public bool Equals(ISegment<char> other)
    {
      if ( this.IsNull() || other.IsNull() ) return false;
      if ( this.Offset != other.Offset ) return false;

      if ( this.Length != other.Length ) return false;

      for ( int i = 0 ; i < Length - 1 ; i++ )
      {
        if ( !this[i].Equals( other[i] ) ) return false;
      }

      return true;
    }

    public bool Equals(IReadOnlyList<char> other)
    {
      var segment = new Segment<char>(other, Offset, Length);
      return this.Equals(segment);
    }

    public override string ToString()
    {
      return _source.Substring(Offset, Length);
    }

    public override bool Equals(object obj)
    {
      if (this.IsNull() || obj.IsNull()) return false;
      if (obj is StringSegment) return this.Equals((StringSegment) obj);
      if (obj is string) return this.Equals((string) obj);
      if (obj is ISegment<char>) return this.Equals(( ISegment<char>) obj);
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      int hash = 0;

      foreach (char c in this)
      {
        hash = hash + c.GetHashCode();
      }
      return hash ^ 2;
    }

    public int Count => this.Length;
  }

  internal class StringSegmentEnumerator : IEnumerator<char>
  {
    private StringSegment _segment;

    private int _max;

    private int _current = 0;

    public StringSegmentEnumerator(StringSegment stringSegment)
    {
      _segment = stringSegment;
      _max = _segment.Offset + _segment.Length;
    }
    public void Dispose()
    {

    }

    public bool MoveNext()
    {
      if (_segment.Length == 0) return false;
      if (_current == (_max)) return false;

      _current++;

      return true;
    }

    public void Reset()
    {
      _current = 0;
    }

    public char Current => _segment[_current];

    object IEnumerator.Current
    {
      get { return Current; }
    }
  }
}