namespace Cubic.Core.Globalization.Localization
{
  public class LocalizedString
  {
    public LocalizedString(string name, string value) : this(name, value, false)
    {

    }

    public LocalizedString(string name, string value, bool resourceNotFound)
    {
      Name = name;
      Value = value;
      ResourceNotFound = resourceNotFound;
    }

    public string Name { get; }
    public string Value { get; }

    public bool ResourceNotFound { get; }

    public static implicit operator string(LocalizedString localizedString)
    {
      return localizedString.ToString();
    }

    public override string ToString()
    {
      return Value;
    }
  }
}