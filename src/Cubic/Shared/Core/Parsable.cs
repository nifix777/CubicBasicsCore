using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core
{
  public class Parsable<T> where T : struct
  {
    private delegate string ToStringMethod(in T @this, string format);

    //private static readonly Func<string, T> parseMethod = Type<T>.Method.Require<Func<string, T>>("Parse", MethodLookup.Static);
    private static readonly Func<string, T> parseMethod = (Func<string, T>)typeof(T).GetMethod("Parse", new[] { typeof(string)} ).CreateDelegate(typeof(Func<string, T>));
    //private static readonly ToStringMethod toStringMethod = Type<T>.Method.Require<ToStringMethod>("ToString", MethodLookup.Instance);
    private static readonly ToStringMethod toStringMethod = (ToStringMethod)typeof(T).GetMethod("ToString", new[] { typeof(string) }).CreateDelegate(typeof(ToStringMethod));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Parse(string text) => parseMethod(text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(in T @this, string format) => toStringMethod(@this, format);
  }
}
