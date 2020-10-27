using System;
using System.IO;

namespace Cubic.Core.Execution.Transient
{
  public class TransientFile : IDisposable
  {
    public TransientFile(string filepath, Action<StreamWriter> fillFileAction)
    {
      FilePath = filepath;
      if (!Path.IsPathRooted(FilePath))
      {
        FilePath = Path.Combine(Environment.CurrentDirectory, FilePath);
      }

      using (var stream = File.Create(FilePath))
      {
        using (var writer = new StreamWriter(stream))
        {
          fillFileAction?.Invoke(writer);
        }
      }
    }

    public string FilePath { get; }

    public FileStream ReadableStream => File.OpenRead(FilePath);

    public FileStream WriteableStream => File.OpenWrite(FilePath);

    public void Dispose()
    {
      File.Delete(FilePath);
    }
  }
}