using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cubic.Core.Reflection;

namespace Cubic.Core.Collections
{
  public static class DictionaryExtensions
  {
    #region IDictionary



    public static void ForEach(this IDictionary collection, Action<object> action)
    {
      foreach (var obj in collection)
      {
        action(obj);
      }
    }

    public static void ForEach<K, V>(this IDictionary<K, V> collection, Action<KeyValuePair<K, V>> action)
    {
      foreach (var obj in collection)
      {
        action(obj);
      }
    }

    public static void TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
      if (dictionary.ContainsKey(key)) return;

      dictionary.Add(key, value);
    }

    /// <summary>
    /// Gets the value or <see cref="defaultValue"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static object GetValueOrDefault(this IDictionary dictionary, object key, object defaultValue)
    {
      if (dictionary.Contains(key)) return dictionary[key];

      return defaultValue;
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValue)
    {
      if(!dictionary.ContainsKey(key))
      {
        dictionary[key] = defaultValue();
      }

      return dictionary[key];
    }

    public static object GetOrAdd(this IDictionary dictionary, object key, Func<object> defaultValue)
    {
      if (!dictionary.Contains(key))
      {
        dictionary[key] = defaultValue();
      }

      return dictionary[key];
    }

    /// <summary>
    /// Gets the value or default-value of the Type.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : (TValue)typeof(TValue).GetDefault();
    }

    public static string ToCollectionString(this IDictionary dictionary, bool inline = false)
    {
      StringBuilder buffer = new StringBuilder();

      if (inline)
      {
        buffer.Append("{");
        foreach (DictionaryEntry entry in dictionary)
        {
          buffer.Append(string.Format("Key:{0} Value:{1}", entry.Key, entry.Value));
        }
        buffer.Append("}");
      }
      else
      {
        buffer.AppendLine("{");
        foreach (DictionaryEntry entry in dictionary)
        {
          buffer.AppendLine(string.Format("Key:{0} Value:{1}", entry.Key, entry.Value));
        }
        buffer.AppendLine("}");
      }

      return buffer.ToString();
    }

    public static string ToCollectionString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool inline = false)
    {
      StringBuilder buffer = new StringBuilder();

      if (inline)
      {
        buffer.Append("{");
        foreach (var entry in dictionary)
        {
          buffer.Append(string.Format("Key:{0} Value:{1}", entry.Key, entry.Value));
        }
        buffer.Append("}");
      }
      else
      {
        buffer.AppendLine("{");
        foreach (var entry in dictionary)
        {
          buffer.AppendLine(string.Format("Key:{0} Value:{1}", entry.Key, entry.Value));
        }
        buffer.AppendLine("}");
      }

      return buffer.ToString();
    }

    #endregion

  }
}