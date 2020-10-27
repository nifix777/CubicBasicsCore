using System;
using System.Collections.Generic;

namespace Cubic.Core.Text
{
  public struct LineAndColumn : IEquatable<LineAndColumn>
  {
    public LineAndColumn(int line, int column)
    {
      Line = line;
      Column = column;
    }

    public int Line { get; }

    public int Column { get; }


    public bool Equals(LineAndColumn other)
    {
      return Line == other.Line && Column == other.Column;
    }

    public override bool Equals(object other)
    {
      //if (ReferenceEquals(null, other)) return false;
      //return other is LineAndColumn && Equals((LineAndColumn) other);

      return EqualityComparer<LineAndColumn?>.Default.Equals(this, other as LineAndColumn?);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Line * 397) ^ Column;
      }
    }
  }
}