namespace Cubic.Core.Globalization.Localization
{
  public class Resource
  {
    public string Culture { get; }

    public string Key { get; }

    public string Value { get; }

    public Resource(string culture, string key, string value)
    {
      Culture = culture;
      Key = key;
      Value = value;
    }
  }
}