using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Diagnostics
{
  public static class DebugUtils
  {
    public static string BriefStackTrace
    {
      get
      {
        return DebugUtils.GetBriefStackTrace(0);
      }
    }

    private static string GetBriefStackTrace(int framesToSkip)
    {
      string str;
      try
      {
        StackFrame[] frames = (new StackTrace(framesToSkip + 1, true)).GetFrames();
        StringBuilder stringBuilder = new StringBuilder((int)frames.Length * 80);
        StackFrame[] stackFrameArray = frames;
        for (int i = 0; i < (int)stackFrameArray.Length; i++)
        {
          StackFrame stackFrame = stackFrameArray[i];
          MethodBase method = stackFrame.GetMethod();
          if (method.ReflectedType != null)
          {
            if (method.ReflectedType.DeclaringType != null)
            {
              CultureInfo invariantCulture = CultureInfo.InvariantCulture;
              object[] name = new object[] { method.ReflectedType.DeclaringType.Name };
              stringBuilder.AppendFormat(invariantCulture, " {0}.", name);
            }
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            object[] objArray = new object[] { method.ReflectedType.Name };
            stringBuilder.AppendFormat(cultureInfo, "{0}.", objArray);
          }
          stringBuilder.Append(method.Name);
          string fileName = stackFrame.GetFileName();
          if (!string.IsNullOrEmpty(fileName))
          {
            int fileLineNumber = stackFrame.GetFileLineNumber();
            CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
            object[] fileName1 = new object[] { Path.GetFileName(fileName), fileLineNumber };
            stringBuilder.AppendFormat(invariantCulture1, " in {0}: line {1}", fileName1);
          }
          stringBuilder.AppendLine();
        }
        str = stringBuilder.ToString();
      }
      catch (Exception exception)
      {
        Trace.TraceError("Got exception while creating stack trace: {0}", new object[] { exception });
        throw;
      }
      return str;
    }
  }
}
