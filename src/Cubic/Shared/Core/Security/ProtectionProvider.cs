using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cubic.Core.Security
{
  public static class ProtectionProvider
  {
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void CheckInternalCall(Assembly callingAssembly, int callerCode)
    {
      if (string.CompareOrdinal(callingAssembly.GetName().FullName, Assembly.GetExecutingAssembly().GetName().FullName) != 0)
      {
        throw ProtectionProvider.GetUnauthorizedCodeAccessException(callingAssembly.GetName().FullName, "Aufruf über Reflection", callerCode);
      }
    }

    private static Exception GetUnauthorizedCodeAccessException(string callingAssemblyName, string additionalInfo, int callerCode)
    {
      object[] parameters = new object[] { Assembly.GetCallingAssembly().GetName().FullName, callingAssemblyName, callerCode, additionalInfo };
      return ProtectionProvider.TraceException(new UnauthorizedCodeAccessException(string.Format("Nicht autorisierter Zugriff auf die Assembly [{0}] von [{1}]. Code: [{2}] Bitte prüfen Sie die Installation. ({3})", parameters)));
    }

    private static Exception TraceException(UnauthorizedCodeAccessException unauthorizedCodeAccessException)
    {
      Trace.TraceError(unauthorizedCodeAccessException.ToString());
      return unauthorizedCodeAccessException;
    }
  }
}