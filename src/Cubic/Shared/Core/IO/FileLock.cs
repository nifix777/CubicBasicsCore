using System;
using System.IO;

namespace Cubic.Core.IO
{
  public class FileLock : IDisposable
  {
    private Stream _fileStream;

    private string _filePath;
    public FileLock(string filePath)
    {
      _filePath = filePath;
    }
    public void Dispose()
    {
      _fileStream?.Dispose();
    }

    public void Open()
    {
      _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }
  }
}