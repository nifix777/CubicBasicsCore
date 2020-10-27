using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cubic.Core.Text;


namespace Cubic.Core.Collections
{
  /// <summary>
  /// Represents an allocation free readonly Segment of a <see cref="IReadOnlyList{T}"/>. Internally a refernece to the original collection is hold.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="Cubic.Core.Collections.ISegment{T}" />
  [DebuggerDisplay( "Offset = {Offset}, Length = {Length}" )]
  public struct Segment<T> : ISegment<T>
  {
    public static ISegment<T> Empty = new Segment<T>(new List<T>());

    private readonly IReadOnlyList<T> _source;

    private readonly int _offset;

    private readonly int _length;

    public Segment(IReadOnlyList<T> source, int offset = 0, int length = 0)
    {
      _source = source;
      _offset = offset;
      _length = length;
    }

    public bool Contains(int position)
    {
      return unchecked(( uint ) ( position - Offset ) < ( uint ) Length);
    }

    public int IndexOf(T item)
    {
      for (int i = 0; i < Length; i++)
      {
        if (item.Equals(this[i])) return i;
      }

      return -1;
    }

    public bool Contains(ISegment<T> segment )
    {
      return segment.Offset >= this.Offset && segment.End <= this.End;
    }

    /// <summary>
    /// Determines whether <paramref name="segment"/> overlaps this span. Two spans are considered to overlap 
    /// if they have positions in common and neither is empty. Empty spans do not overlap with any 
    /// other span.
    /// </summary>
    /// <param name="segment">
    /// The span to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the spans overlap, otherwise <c>false</c>.
    /// </returns>
    public bool OverlapsWith( ISegment<T> segment )
    {
      int overlapStart = Math.Max( Offset , segment.Offset );
      int overlapEnd = Math.Min( this.End , segment.End );

      return overlapStart < overlapEnd;
    }

    public T this[int index]
    {
      get
      {
        var internalIndex = Offset + index;
        if (internalIndex > (Offset + Length)) throw new IndexOutOfRangeException();
        return _source[internalIndex];
      }
    }


    public IEnumerator<T> GetEnumerator()
    {
      return new SegmentEnumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Equals(Segment<T> other)
    {
      return this.Equals((ISegment<T>)other);
    }

    public bool Equals(ISegment<T> other)
    {
      if (this.IsNull() || other.IsNull()) return false;
      if (this.Offset != other.Offset) return false;

      if (this.Length != other.Length) return false;

      for (int i = 0; i < Length - 1; i++)
      {
        if (!this[i].Equals(other[i])) return false;
      }

      return true;
    }

    public int Offset => _offset;
    public int Length => _length;
    public int End => Offset + Length;
    public bool IsEmpty => this.Length == 0;

    public bool Equals(IReadOnlyList<T> other)
    {
      var segment = new Segment<T>(other, Offset, Length);

      return this.Equals(segment);
    }

    /// <summary>
    /// Provides a string representation for <see cref="StringSegment"/>.
    /// </summary>
    public override string ToString()
    {
      return $"[{Offset}..{End})";
    }

    public static IEnumerable<int> GetDiffs(ISegment<T> left, ISegment<T> right)
    {
      if (left.IsEmpty || right.IsEmpty) return Enumerable.Empty<int>();

      IList<int> diffIndexes = new List<int>();

      if ( left.Length == right.Length )
      {
        for ( int i = 0 ; i < left.Length ; i++ )
        {
          if ( !left[i].Equals( right[i] ) )
          {
            diffIndexes.Add( i );
          }
        } 
      }
      else if(left.Length > right.Length)
      {
        for ( int i = 0 ; i < left.Length ; i++ )
        {
          if ( i >= right.Length )
          {
            diffIndexes.Add(i);
            continue;
          }

          if ( !left[i].Equals( right[i] ) )
          {
            diffIndexes.Add( i );
          }
        }
      }
      else
      {
        for ( int i = 0 ; i < right.Length ; i++ )
        {
          if ( i >= left.Length )
          {
            diffIndexes.Add(i);
            continue;
          }

          if ( !left[i].Equals( right[i] ) )
          {
            diffIndexes.Add( i );
          }
        }
      }

      return diffIndexes;
    }

    public int Count => Length;
  }
}