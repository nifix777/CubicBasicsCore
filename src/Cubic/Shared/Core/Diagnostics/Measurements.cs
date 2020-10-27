using System;
using System.Diagnostics;

namespace Cubic.Core.Diagnostics
{
  public static class Measurements
  {
    public static int Repetitions { get; set; } = 100;

    public static void Measure<T>( string description , Func<T> initialization , Action<T> action , Action<T> cleanup = null )
    {
      var t = initialization();
      action( t ); // warmup

      var sw = Stopwatch.StartNew();

      for ( int i = 0 ; i < Repetitions ; ++i )
      {
        action( t );
      }

      sw.Stop();

      cleanup?.Invoke( t );

      Trace.WriteLine( $"{description} took {sw.ElapsedMilliseconds / Repetitions}ms" );
    }

    public static void Measure<T>( string description , Func<T> action )
    {
      action(); // warmup

      var sw = Stopwatch.StartNew();

      for ( int i = 0 ; i < Repetitions ; ++i )
      {
        action();
      }

      sw.Stop();
      Trace.WriteLine( $"{description} took {sw.ElapsedMilliseconds / Repetitions}ms" );
    }
  }
}