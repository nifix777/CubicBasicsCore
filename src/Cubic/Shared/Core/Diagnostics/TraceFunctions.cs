using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Cubic.Core.Reflection;
using Cubic.Core.Runtime;

namespace Cubic.Core.Diagnostics
{
  public static class TraceFunctions
  {
    public static TextWriter DumpOutput { get; set; } = System.Console.Out;

    public static void Traceing<T>( string description , Func<T> action, bool useWarmUp = false )
    {
      // warmup
      if (useWarmUp)
      {
        action();
      }


      var sw = Stopwatch.StartNew();

      action();

      sw.Stop();
      Trace.WriteLine( $"{description} took {sw.ElapsedMilliseconds}ms" );
    }

    public static void Dump(this object value)
    {
      if (value.GetType().IsSimple())
      {
        DumpOutput.WriteLine(value);
        return;
      }

      if (value is IEnumerable)
      {
        DumpCollection((IEnumerable)value);
        return;
      }

      DumpComplex(value);

    }

    public static void DumpCollection(IEnumerable collection)
    {
      var msg = new StringBuilder();

      msg.Append("{ ");

      foreach (var item in collection)
      {
        msg.Append(item);
        msg.Append(",");
      }
      msg.Remove(msg.Length - 1, 1);
      msg.Append(" }");
    }

    public static void DumpComplex(object complexObject)
    {
      var type = complexObject.GetType();

      foreach (var property in type.GetPublicProperties())
      {
        var value = property.GetValue(complexObject);
        DumpOutput.WriteLine(string.Concat(property.Name, ":", value));
      }
    }
  }
}