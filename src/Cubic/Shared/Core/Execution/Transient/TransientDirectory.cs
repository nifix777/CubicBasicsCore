using System;
using System.IO;

namespace Cubic.Core.Execution.Transient
{
  public class TransientDirectory : TransientContext
  {
    private DirectoryInfo _directory;

    public string FolderPath => _directory.FullName;

    public TransientDirectory(string directoryPath)
    {
      var dir = directoryPath;
      if (!Path.IsPathRooted(dir))
      {
        dir = Path.Combine(Environment.CurrentDirectory, dir);
      }

      _directory = Directory.CreateDirectory(dir);
    }

    public TransientFile CreateFile(string filename)
    {
      return this.CreateFile(filename, null);
    }

    public TransientFile CreateFile(string filename, Action<StreamWriter> writeCallback)
    {
      
      var file = new TransientFile(Path.Combine(FolderPath, filename), writeCallback);
      this.Register(file);
      return file;
    }

    public override void Dispose()
    {
      base.Dispose();
      _directory.Delete(true);    
    }
  }
}