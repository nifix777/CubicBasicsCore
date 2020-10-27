using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Collections
{
  public static class EnumerableExtensions
  {
    #region IEnumerable

    public static string Join<T>(this IEnumerable<T> source, string seperator)
    {
      return string.Join(seperator, source);
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
      // Enumerable.Any<T> underneath doesn't cast to ICollection, 
      // like it does with many of the other LINQ methods.
      // Below is significantly (4x) when mainly working with ICollection
      // sources and a little slower if working with mainly IEnumerable<T> 
      // sources.

      if (source == null) return true;

      // Cast to ICollection instead of ICollection<T> for performance reasons.
      ICollection collection = source as ICollection;
      if (collection != null)
      {
        return collection.Count < 1;
      }

      return !source.Any();
    }

    /// <summary>
    /// Ases the observable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static ObservableCollection<T> AsObservable<T>(this IEnumerable<T> source)
    {
      return new ObservableCollection<T>(source);
    }

    /// <summary>
    /// Determines whether the specified source is valid. Test if source == null or Empty.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns><count>true</count> if the specified source is valid; otherwise, <count>false</count>.</returns>
    public static bool IsValid(this IEnumerable source)
    {
      //DO NOT ADD CONTRACT HERE 
      return source != null && source.Count() > 0;
    }

    /// <summary>
    /// Counts the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>System.Int32.</returns>
    public static int Count(this IEnumerable source)
    {
      Guard.ArgumentNull(source, nameof(source));

      var enumerable = source as ICollection<object> ?? source.Cast<object>().ToList();

      return enumerable.Count;
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] values)
    {
      Guard.ArgumentNull(source, nameof(source));

      foreach (var v in source)
      {
        yield return v;
      }

      foreach (var value in values)
      {
        yield return value;
      }
    }

    public static IEnumerable<T> Pipe<T>(this IEnumerable<T> source, Action<T> pipe)
    {
      Guard.ArgumentNull(source, nameof(source));

      foreach (var v in source)
      {
        pipe(v);
        yield return v;
      }
    }

    public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
    {
      Guard.ArgumentNull(source, nameof(source));

      return new ReadOnlyCollection<T>(source.ToList());
    }

    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this T[] source)
    {
      Guard.ArgumentNull(source, nameof(source));

      return new ReadOnlyArray<T>(source);
    }

    /// <summary>
    /// Liefert die Elemente einer Auflsitung zurück, die zwischen oder gleich dem angegebenen Minimum\Maximum liegen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Die Auflsitung.</param>
    /// <param name="min">Das Minimum.</param>
    /// <param name="max">Das Maximum.</param>
    /// <returns></returns>
    public static IEnumerable<T> Between<T>(this IEnumerable<T> collection, T min, T max) where T : IComparable
    {
      return collection.Where(v => v.IsBetween(min, max));
    }

    /// <summary>
    /// Verflacht eine hierarchische Objekt-Struktur.
    /// </summary>
    /// <typeparam name="T">Der Typ der Elemente in der Hierachie</typeparam>
    /// <param name="item">Das Root-Objekt dessen Sub-Elemente verflacht werden sollen. Dieses wird als erstes in die Rückgabeliste gegeben.</param>
    /// <param name="selector">Eine Funktion zum Auswählen der Unter-Elemente unter einem Element.</param>
    /// <returns>Eine Auflistung aller Elemente unter <paramref name="item"/> inklusive <paramref name="item"/> selbst</returns>
    public static IEnumerable<T> Flatten<T>(this T item, Func<T, IEnumerable<T>> selector)
    {
      yield return item;
      foreach (var subSubItem in selector(item).SelectMany(subItem => Flatten(subItem, selector)))
      {
        yield return subSubItem;
      }
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequence)
    {
      Guard.ArgumentNull(sequence, nameof(sequence));
      return sequence.SelectMany(s => s);
    }

    /// <summary>
    /// Splits a collection into two lists based on the given true/false evaluation.
    /// 
    /// Note: This method resolves the query at once for performance considerations.
    ///       It does not use delayed execution!
    /// </summary>
    /// <typeparam name="T">the type of the list items</typeparam>
    /// <param name="inList">the original list</param>
    /// <param name="predicate">the function delegate to the evaluator</param>
    /// <param name="trueList">the true results</param>
    /// <param name="falseList">the false results</param>
    public static void SplitUp<T>(this IEnumerable<T> inList
        , Func<T, Boolean> predicate
        , out IEnumerable<T> trueList
        , out IEnumerable<T> falseList)
    {
      //The .ToList() is deliberately here
      //otherwise the predicate would always be checked twice.
      //First when the user of the method iterates over the trueList
      //Second when the user of the method iterates over the falseList
      trueList = inList.Where(predicate).ToList();
      falseList = inList.Except(trueList).ToList();
    }


    /// <summary>
    /// Splits a collection into two lists based on the given true/false evaluation
    /// </summary>
    /// <typeparam name="T">the type of the list items</typeparam>
    /// <param name="inList">the original list</param>
    /// <param name="predicate">the function delegate to the evaluator</param>
    /// <param name="trueList">the true results</param>
    /// <param name="falseList">the false results</param>
    public static void SplitDelayed<T>(this IEnumerable<T> inList
        , Func<T, Boolean> predicate
        , out IEnumerable<T> trueList
        , out IEnumerable<T> falseList)
    {
      trueList = inList.Where(predicate);
      falseList = inList.Except(trueList);
    }


    /// <summary>
    /// Entfernt Duplikate aus einem <see cref="System.Collections.Generic.IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Der Typ der aufzulistenden Objekte.
    /// </typeparam>
    /// <param name="collection">
    /// Die Auflistung.
    /// </param>
    /// <returns>
    /// Auflistung ohne Duplikate.
    /// </returns>
    public static List<T> RemoveDuplicates<T>(this IEnumerable<T> collection)
    {
      return new HashSet<T>(collection).ToList();
    }

    //---------------------------------------------------------------------
    /// <summary>
    /// Entfernt Duplikate aus einem <see cref="System.Collections.Generic.IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Der Typ der aufzulistenden Objekte.
    /// </typeparam>
    /// <param name="collection">
    /// Die Auflistung.
    /// </param>
    /// <param name="comparer">
    /// Die <see cref="System.Collections.Generic.IEqualityComparer{T}"/>-Implementierung, die 
    /// zum Vergleichen von Schlüsseln verwendet werden soll, oder null, 
    /// wenn der Standard-<see cref="System.Collections.Generic.EqualityComparer{T}"/> für diesen 
    /// Schlüsseltyp verwendet werden soll.
    /// </param>
    /// <returns>
    /// Auflistung ohne Duplikate.
    /// </returns>
    public static List<T> RemoveDuplicates<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer)
    {
      return new HashSet<T>(collection, comparer).ToList();
    }

    //---------------------------------------------------------------------
    /// <summary>
    /// Entfernt Duplikate aus einem <see cref="System.Collections.Generic.IEnumerable{T}"/>. Die
    /// Reihenfolge der Elemente wird dabei beibehalten.
    /// </summary>
    /// <typeparam name="T">
    /// Der Typ der aufzulistenden Objekte.
    /// </typeparam>
    /// <param name="collection">
    /// Die Auflistung.
    /// </param>
    /// <returns>
    /// Auflistung ohne Duplikate.
    /// </returns>
    public static List<T> RemoveDuplicatesKeepOrder<T>(this IEnumerable<T> collection)
    {
      HashSet<T> hashSet = new HashSet<T>();
      List<T> result = new List<T>(collection.Count());

      foreach (T item in collection)
        if (hashSet.Add(item))
          result.Add(item);

      return result;
    }

    //---------------------------------------------------------------------
    /// <summary>
    /// Entfernt Duplikate aus einem <see cref="System.Collections.Generic.IEnumerable{T}"/>. Die
    /// Reihenfolge der Elemente wird dabei beibehalten.
    /// </summary>
    /// <typeparam name="T">
    /// Der Typ der aufzulistenden Objekte.
    /// </typeparam>
    /// <param name="collection">
    /// Die Auflistung.
    /// </param>
    /// <param name="comparer">
    /// Die <see cref="System.Collections.Generic.IEqualityComparer{T}"/>-Implementierung, die 
    /// zum Vergleichen von Schlüsseln verwendet werden soll, oder null, 
    /// wenn der Standard-<see cref="System.Collections.Generic.EqualityComparer{T}"/> für diesen 
    /// Schlüsseltyp verwendet werden soll.
    /// </param>
    /// <returns>
    /// Auflistung ohne Duplikate.
    /// </returns>
    public static List<T> RemoveDuplicatesKeepOrder<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer)
    {
      HashSet<T> hashSet = new HashSet<T>(comparer);
      List<T> result = new List<T>(collection.Count());

      foreach (T item in collection)
        if (hashSet.Add(item))
          result.Add(item);

      return result;
    }



    /// <summary>
    /// Überprüft ob die Auflistung eine Mindestzahl an Elementen aufweißt.
    /// </summary>
    /// <typeparam name="TSource">Der Typ der Elemente in der Auflistung.</typeparam>
    /// <param name="list">Die Auflistung.</param>
    /// <param name="count">Die Anzahl, bei der geprüft werden soll, ob mindestens so viele in <paramref name="list"/> enthalten sind.</param>
    /// <param name="predicate">Eine Funktion, mit der geprüft wird, ob ein Element mit gezählt wird.</param>
    /// <returns><c>True</c>, wenn <paramref name="list"/> nicht <c>NULL</c> ist mindestens <paramref name="count"/> Elemente enthält. Andernfalls <c>False</c>.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Wird ausgelöst, wenn <paramref name="count"/> kleiner als 0 ist.</exception>
    /// <exception cref="System.ArgumentNullException">Wird ausgelöst, wenn <paramref name="predicate"/> nicht zugewiesen wurde.</exception>
    public static bool HasCount<TSource>(this IEnumerable<TSource> list, int count, Func<TSource, bool> predicate)
    {
      if (list == null)
        return false;
      if (count < 0)
        throw new ArgumentOutOfRangeException("count");
      if (predicate == null)
        throw new ArgumentNullException("predicate");
      if (count == 0)
        return true;

      using (var enumerator = list.GetEnumerator())
      {
        while (count > 0 && enumerator.MoveNext())
          if (predicate(enumerator.Current))
            --count;
      }
      if (count == 0)
        return true;
      return false;
    }

    /// <summary>
    /// Evaluate if a Element exits in the current Collection that matches the given query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="query">The query.</param>
    /// <returns></returns>
    public static bool Exits<T>(this IEnumerable<T> collection, Predicate<T> query)
    {
      return collection.FirstOrDefault(query.Invoke) != null;
    }

    /// <summary>
    /// Splits the source in multiple collections of the given size.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
              this IEnumerable<TSource> source, int size)
    {
      TSource[] bucket = null;
      var count = 0;

      foreach (var item in source)
      {
        if (bucket == null)
          bucket = new TSource[size];

        bucket[count++] = item;
        if (count != size)
          continue;

        yield return bucket;

        bucket = null;
        count = 0;
      }

      if (bucket != null && count > 0)
        yield return bucket.Take(count);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      Guard.ArgumentNull(source, nameof(source));
      Guard.ArgumentNull(action, nameof(action));

      // perf optimization. try to not use enumerator if possible
      if (source is IList<T> list)
      {
        for (int i = 0, count = list.Count; i < count; i++)
        {
          action(list[i]);
        }
      }
      else
      {
        foreach (var value in source)
        {
          action(value);
        }
      }

      //return source;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
    where T : class
    {
      if (source == null)
      {
        return Enumerable.Empty<T>();
      }

      return source.Where(t => !t.IsNull());
    }

    public static bool All(this IEnumerable<bool> source)
    {
      Guard.ArgumentNull(source, nameof(source));

      return source.All(b => b);
    }

    /// <summary>
    /// Firsts the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static object First(this IEnumerable source)
    {
      var index = source.GetEnumerator();
      if (index.MoveNext())
      {
        return index.Current;
      }

      return null;
    }


    /// <summary>
    /// Gets the type of the enumerated object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static T GetEnumeratedType<T>(this IEnumerable<T> collection)
    {
      return default(T);
    }


    /// <summary>
    /// Does a Left join on two Collections.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TInner">The type of the inner.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="inner">The inner collection.</param>
    /// <param name="pk">The primary key of the soruce Collection.</param>
    /// <param name="fk">The foreign key of the inner Collection.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static IEnumerable<TResult> LeftJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                 IEnumerable<TInner> inner,
                                                 Func<TSource, TKey> pk,
                                                 Func<TInner, TKey> fk,
                                                 Func<TSource, TInner, TResult> result)
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from s in source
                join i in inner
                on pk(s) equals fk(i) into joinData
                from left in joinData.DefaultIfEmpty()
                select result(s, left);

      return _result;
    }


    /// <summary>
    /// Does a Right join on two Collections.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TInner">The type of the inner.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="inner">The inner collection.</param>
    /// <param name="pk">The primary key of the soruce Collection.</param>
    /// <param name="fk">The foreign key of the inner Collection.</param>
    /// <param name="result">The result.</param>
    /// <example>

    /*
     * var resultJoint = Person.BuiltPersons().RightJoin(                   /// Source Collection
                    Address.BuiltAddresses() ,                        /// Inner Collection
                    p => p.IdAddress ,                                /// PK
                    a => a.IdAddress ,                                /// FK
                    ( p , a ) => new { MyPerson = p , MyAddress = a } )   /// Result Collection
                    .Select( a => new
                    {
                      Name = ( a.MyPerson != null ? a.MyPerson.Name : "Null-Value" ) ,
                      Age = ( a.MyPerson != null ? a.MyPerson.Age : -1 ) ,
                      PersonIdAddress = ( a.MyPerson != null ? a.MyPerson.IdAddress : -1 ) ,
                      AddressIdAddress = a.MyAddress.IdAddress ,
                      Street = a.MyAddress.Street
                    } );
 
    */

    /// </example>
    /// <returns></returns>
    public static IEnumerable<TResult> RightJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                  IEnumerable<TInner> inner,
                                                  Func<TSource, TKey> pk,
                                                  Func<TInner, TKey> fk,
                                                  Func<TSource, TInner, TResult> result)
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from i in inner
                join s in source
                on fk(i) equals pk(s) into joinData
                from right in joinData.DefaultIfEmpty()
                select result(right, i);

      return _result;
    }




    /// <summary>
    /// Does a Full Outer Join on two Collections.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TInner">The type of the inner.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="inner">The inner collection.</param>
    /// <param name="pk">The primary key of the soruce Collection.</param>
    /// <param name="fk">The foreign key of the inner Collection.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static IEnumerable<TResult> FullOuterJoinJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                          IEnumerable<TInner> inner,
                                                          Func<TSource, TKey> pk,
                                                          Func<TInner, TKey> fk,
                                                          Func<TSource, TInner, TResult> result)
    {

      var left = source.LeftJoin(inner, pk, fk, result).ToList();
      var right = source.RightJoin(inner, pk, fk, result).ToList();

      return left.Union(right);
    }


    public static IEnumerable<TResult> LeftExcludingJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                          IEnumerable<TInner> inner,
                                                          Func<TSource, TKey> pk,
                                                          Func<TInner, TKey> fk,
                                                          Func<TSource, TInner, TResult> result)
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from s in source
                join i in inner
                on pk(s) equals fk(i) into joinData
                from left in joinData.DefaultIfEmpty()
                where left == null
                select result(s, left);

      return _result;
    }


    public static IEnumerable<TResult> RightExcludingJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source,
                                                        IEnumerable<TInner> inner,
                                                        Func<TSource, TKey> pk,
                                                        Func<TInner, TKey> fk,
                                                        Func<TSource, TInner, TResult> result)
    {
      IEnumerable<TResult> _result = Enumerable.Empty<TResult>();

      _result = from i in inner
                join s in source
                on fk(i) equals pk(s) into joinData
                from right in joinData.DefaultIfEmpty()
                where right == null
                select result(right, i);

      return _result;
    }

    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
    {
      Guard.AgainstNull(source, nameof(source));
      return AppendIterator<TSource>(source, element);
    }

    private static IEnumerable<TSource> AppendIterator<TSource>(IEnumerable<TSource> source, TSource element)
    {
      foreach (TSource e1 in source) yield return e1;
      yield return element;
    }

    public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
    {
      Guard.AgainstNull(source, nameof(source));
      return PrependIterator<TSource>(source, element);
    }

    private static IEnumerable<TSource> PrependIterator<TSource>(IEnumerable<TSource> source, TSource element)
    {
      yield return element;
      foreach (TSource e1 in source) yield return e1;
    }





    //public static T[] ToArray<T>(this IEnumerable<T> enumerable)
    //{
    //  T[] array = enumerable as T[];

    //  if (array != null)
    //  {
    //    return array;
    //  }

    //  return Enumerable.ToArray(enumerable);
    //}

    public static List<T> AsList<T>(this IEnumerable<T> enumerable)
    {
      List<T> list = enumerable as List<T>;

      if (list != null)
      {
        return list;
      }

      return enumerable.ToList();
    }

    public static IEnumerable<T> Emit<T>(this T item)
    {
      yield return item;
    }

    public static IEnumerable<T> Diff<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
      Guard.AgainstNull(source, nameof(source));
      Guard.AgainstNull(other, nameof(other));

      var sourceList = source.ToArray();
      var otherList = other.ToArray();


      Guard.Ensure<InvalidOperationException>(() => sourceList.Length == otherList.Length, "Collections must have the same number of items");

      for (int i = 0; i < sourceList.Length; i++)
      {
        if (sourceList[i].Equals(otherList[i])) yield return sourceList[i];
      }
    }

    public static IEnumerable<T> Diff<T>(this IEnumerable<T> source, IEnumerable<T> other, IComparer<T> comparer)
    {
      Guard.AgainstNull(source, nameof(source));
      Guard.AgainstNull(other, nameof(other));

      var sourceList = source.ToArray();
      var otherList = other.ToArray();


      Guard.Ensure<InvalidOperationException>(() => sourceList.Length == otherList.Length, "Collections must have the same number of items");

      for (int i = 0; i < sourceList.Length; i++)
      {
        if (comparer.Compare(sourceList[i], otherList[i]) != 0) yield return sourceList[i];
      }
    }


    #endregion
  }
}