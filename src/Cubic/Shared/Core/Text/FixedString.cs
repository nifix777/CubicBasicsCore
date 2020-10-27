using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Text
{
  public struct FixedString : IEquatable<FixedString>, IEquatable<string>, IReadOnlyList<char>
  {
    private const int MaxLength = 10;
    private readonly char[] _text;

    public FixedString(int length = 3)
    {
      if (MaxLength < length) throw new ArgumentOutOfRangeException(nameof(length), $" Length of {nameof(length)} is longer than Max value of {MaxLength}.");
      Count = length;
      _text = new char[Count];
    }

    public FixedString(string text)
    {
      if (text is null)
      {
        throw new ArgumentNullException(nameof(text));
      }

      if (MaxLength < text.Length) throw new ArgumentOutOfRangeException(nameof(text), $" Length of {nameof(text)} is longer than Max value of {MaxLength}.");
      Count = text.Length;
      _text = text.ToCharArray();
    }

    public char this[int index] => _text[index];

    public int Count { get; }

    public bool Equals(FixedString other)
    {
      if (this.Count != other.Count) return false;
      return Enumerable.SequenceEqual(this, other);
    }

    public bool Equals(string other)
    {
      if (other is null) return false;
      if (this.Count != other.Length) return false;
      return Enumerable.SequenceEqual(this, other);
    }

    public IEnumerator<char> GetEnumerator() => _text.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override int GetHashCode()
    {
      int hashCode = 173212135;
      hashCode = hashCode * -1521134295 + Count.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(_text);
      return hashCode;
    }



    public override string ToString()
    {
      return new string(_text);
    }

    public static FixedString FromString(string value)
    {
      return new FixedString(value);
    }

    public static bool operator ==(FixedString left, FixedString right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(FixedString left, FixedString right)
    {
      return !(left == right);
    }

    public static implicit operator FixedString (string value)
    {
      return FixedString.FromString(value);
    }

    public static implicit operator string(FixedString fixedString)
    {
      return fixedString.ToString();
    }
  }
}
