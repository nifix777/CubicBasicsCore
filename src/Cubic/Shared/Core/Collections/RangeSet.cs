using System;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  public class RangeSet<T> : IRangeSet<T> where T : IComparable

  {

    private readonly SortedSet<RangeInfo> _set;



    public RangeSet()

    {

      var comparer = new Comparer();

      _set = new SortedSet<RangeInfo>( comparer );

    }



    public void AddRange( T min , T max )

    {

      var comparison = min.CompareTo( max );

      if ( comparison > 0 )

        throw new ArgumentException( "Min must be less than Max" );



      _set.Add( new RangeInfo

      {

        Min = min ,

        Max = max

      } );

    }



    public bool Contains( T findValue )

    {

      return _set.Contains( new RangeInfo

      {

        FindValue = findValue ,

        IsFind = true

      } );

    }



    private struct RangeInfo

    {

      public bool IsFind;

      public T FindValue;



      public T Min;

      public T Max;

    }



    private class Comparer : IComparer<RangeInfo>

    {

      public int Compare( RangeInfo x , RangeInfo y )

      {

        if ( x.IsFind )

          return IsInRange( y , x.FindValue );



        if ( y.IsFind )

          return IsInRange( x , y.FindValue ) * -1;



        var xMaxComparedToYMin = x.Max.CompareTo( y.Min );

        var xMinComparedToYMax = x.Min.CompareTo( y.Max );



        if ( xMaxComparedToYMin != 0 && xMinComparedToYMax != 0 )

        {

          var xMaxGreaterThanYMin = xMaxComparedToYMin > 0;

          var xMinLessThanYMax = xMinComparedToYMax < 0;



          if ( xMaxGreaterThanYMin ^ xMinLessThanYMax )

            return xMaxGreaterThanYMin ? 1 : -1;

        }



        throw new InvalidOperationException( "Cannot have overlapping ranges" );

      }



      private static int IsInRange( RangeInfo rangeInfo , T find )

      {

        var isGreaterThanMin = find.CompareTo( rangeInfo.Min ) >= 0;

        var isLessThanMax = find.CompareTo( rangeInfo.Max ) <= 0;



        if ( isGreaterThanMin && isLessThanMax )

          return 0;



        return isGreaterThanMin ? 1 : -1;

      }

    }

  }
}