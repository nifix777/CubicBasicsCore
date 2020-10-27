using System;
using System.IO;

namespace Cubic.Core.IO
{
  public class FileChangedChecker
  {
    private readonly string _filePath;

    private DateTime _lastWriteTime;

    private DateTime CurrentLastWriteTime
    {
      get
      {
        if (!File.Exists(this._filePath))
        {
          return DateTime.MaxValue;
        }
        return File.GetLastWriteTime(this._filePath);
      }
    }

    public bool IsChanged
    {
      get
      {
        DateTime currentLastWriteTime = this.CurrentLastWriteTime;
        if (this._lastWriteTime.Equals(currentLastWriteTime))
        {
          return false;
        }
        this._lastWriteTime = currentLastWriteTime;
        return true;
      }
    }

    public FileChangedChecker(string filePath)
    {
      this._filePath = filePath;
      this._lastWriteTime = this.CurrentLastWriteTime;
    }

    public void SetUnchanged()
    {
      this._lastWriteTime = this.CurrentLastWriteTime;
    }
  }
}