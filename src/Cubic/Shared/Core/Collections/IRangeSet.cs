using System;

namespace Cubic.Core.Collections
{


  public interface IRangeSet<in T> where T : IComparable
  {

    void AddRange( T min , T max );

    bool Contains( T findValue );

  }
}