using System;

namespace Cubic.Core.Diagnostics
{
  [Serializable]
  public class CallStackInfo
  {
    public string Filename { get; set; }

    public string Method { get; set; }

    public int LineNumber { get; set; }
  }
}
