namespace Cubic.Core.Text
{
  public interface ILineParser
  {
    void ParseLine(char[] lineBuffer);

    bool IsLineValid(char[] lineBuffer);
  }
}