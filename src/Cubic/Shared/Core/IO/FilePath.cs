using Cubic.Core.Diagnostics;

namespace Cubic.Core.IO
{

  /// <summary>
  /// Struct for 
  /// <example>
  /// var rootPath = new FilePath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
  /// var path = rootPath / ".nuget" / "packages";
  /// </example>
  /// </summary>
  public struct FilePath
  {
    public string Path { get; }

    public FilePath(string path)
    {
      Guard.AgainstNullOrEmpty(path, nameof(path));
      Path = path;
    }

    public static FilePath operator /(FilePath left, FilePath right)
    {
      return new FilePath(System.IO.Path.Combine(left.Path, right.Path));
    }

    public static FilePath operator /(FilePath left, string right)
    {
      return new FilePath(System.IO.Path.Combine(left.Path, right));
    }

    public override string ToString()
    {
      return Path;
    }
  }
}