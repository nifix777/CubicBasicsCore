using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cubic.Core.Text
{
  public class ParameterParser : IDisposable, ICollection<BasicParameter>
  {
    public IList<BasicParameter> _parameters = new List<BasicParameter>();

    public char ParameterSeperator { get; set; } = Constants.Semicolon.ToChar();
    public char ParameterAssignSymbol { get; set; } = Constants.Equal.ToChar();

    public virtual void Parse(params string[] args)
    {
      StringBuilder builder = new StringBuilder();

      foreach (var s in args)
      {
        builder.Append(s);
      }

      var parsingString = builder.ToString();

      if(string.IsNullOrEmpty(parsingString)) return;

      var parameterAssignments = parsingString.Split(ParameterSeperator);

      foreach (var parameterAssignment in parameterAssignments)
      {
        var keyValue = parameterAssignment.Split(ParameterAssignSymbol);

        if(keyValue.Length < 2) return;

        var key = keyValue[0];
        var value = keyValue[1];

        _parameters.Add(new BasicParameter(key, value));
      }
    }

    public IEnumerator<BasicParameter> GetEnumerator()
    {
      return _parameters.GetEnumerator();
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();

      foreach ( var para in _parameters )
      {
        builder.AppendFormat( "{0}{1}{2}{3}", para.Name, ParameterAssignSymbol, para.Value, ParameterSeperator );
      }

      return builder.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) _parameters).GetEnumerator();
    }

    public void Dispose()
    {
      _parameters.Clear();
    }

    public void Add(BasicParameter item)
    {
      _parameters.Add(item);
    }

    public void Clear()
    {
      _parameters.Clear();
    }

    public bool Contains(BasicParameter item)
    {
      return _parameters.Contains(item);
    }

    public void CopyTo(BasicParameter[] array, int arrayIndex)
    {
      _parameters.CopyTo(array, arrayIndex);
    }

    public bool Remove(BasicParameter item)
    {
      return _parameters.Remove(item);
    }

    public int Count => _parameters.Count;

    public bool IsReadOnly => _parameters.IsReadOnly;
  }
}