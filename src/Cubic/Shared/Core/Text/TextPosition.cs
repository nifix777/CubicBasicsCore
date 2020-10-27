using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text
{
  public struct TextPosition : IEquatable<TextPosition>
  {
    public TextPosition(int line, int position) : this()
    {
      Line = line;
      Position = position;
    }

    public int Line { get; set; }

    public int Position { get; set; }

    public override bool Equals(object obj)
    {
      return obj is TextPosition && Equals((TextPosition)obj);
    }

    public bool Equals(TextPosition other)
    {
      return Line == other.Line &&
             Position == other.Position;
    }

    public override int GetHashCode()
    {
      var hashCode = 1760694153;
      hashCode = hashCode * -1521134295 + Line.GetHashCode();
      hashCode = hashCode * -1521134295 + Position.GetHashCode();
      return hashCode;
    }
  }
}
