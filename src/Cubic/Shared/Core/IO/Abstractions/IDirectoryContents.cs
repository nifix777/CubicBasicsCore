using System.Collections.Generic;

namespace Cubic.Core.IO.Abstractions
{
  /// <summary>
  /// Represents a directory's content in the file provider.
  /// </summary>
  public interface IDirectoryContents : IEnumerable<IFileInfo>
  {
    /// <summary>
    /// True if a directory was located at the given path.
    /// </summary>
    bool Exists { get; }
  }
}