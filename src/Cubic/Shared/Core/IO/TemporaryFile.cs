using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cubic.Shared.Core.Cubic.Shared.Core.IO
{
  public class TemporaryFile : IDisposable
  {
    string _path;

    public TemporaryFile() : this(true)
    {
    }

    public TemporaryFile(bool shortLived) : this(shortLived, null)
    {
    }

    public TemporaryFile(string extension) : this(true, extension)
    {
    }
    public TemporaryFile(bool shortLived, string extension)
    {
      _path = System.IO.Path.GetTempFileName();
      if (!string.IsNullOrWhiteSpace(extension))
      {
        string origPath = _path;
        if (extension[0] == '.') _path += extension;
        else _path += '.' + extension;
        File.Move(origPath, _path);
      }
      if (shortLived) File.SetAttributes(_path, FileAttributes.Temporary);
    }


    public void Dispose()
    {
      DeleteFile();
    }
    public void Detach()
    {
      var p = _path;
      if (p == null) throw new ObjectDisposedException("TemporaryFile");
      _path = string.Empty;
    }

    public bool IsDetached
    {
      get { return Path.Length == 0; }
    }

    public string Path
    {
      get
      {
        var p = _path;
        if (p == null) throw new ObjectDisposedException("TemporaryFile");
        return p;
      }
    }

    private bool DeleteFile()
    {
      var p = _path;
      if (p != null)
      {
        if (p.Length == 0) _path = null;
        else
        {
          try { File.Delete(p); _path = null; }
          catch { return false; }
        }
        return true;
      }
      return false;
    }
  }
}
