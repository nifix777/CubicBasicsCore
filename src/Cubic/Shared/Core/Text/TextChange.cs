using System;
using System.Collections.Generic;
using System.Linq;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Text
{
  public struct TextChange : IEquatable<TextChange>
  {
    public ISegment<char> TextSegment { get; } 

    public ISegment<char> NewTextSegment { get; }

    public TextChange(ISegment<char> segment, ISegment<char> newtext )
    {
      Guard.ArgumentNull(newtext, nameof(newtext));

      this.TextSegment = segment;
      this.NewTextSegment = newtext;
      ChangedIndexes = Segment<char>.GetDiffs( TextSegment , NewTextSegment );
    }

    public bool Equals( TextChange other )
    {
      if (!this.TextSegment.Equals(other.TextSegment ) ) return false;

      return this.NewTextSegment.Equals(other.NewTextSegment );
    }

    public IEnumerable<int> ChangedIndexes { get; }

    /// <summary>
    /// An empty set of changes.
    /// </summary>
    public static IEnumerable<TextChange> NoChanges => Enumerable.Empty<TextChange>();
  }
}