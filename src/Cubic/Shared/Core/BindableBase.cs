using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Cubic.Core
{
  public class BindableBase : INotifyPropertyChanged, INotifyPropertyChanging
  {
    #region Change Notification

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    public void NotifyPropertyChanged(string propertyName)
    {

      if (ThrowOnInvalidPropertyName)
      {
        this.VerifyPropertyName(propertyName);
      }

      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }

    public void Set<T>(ref T property, T value, [CallerMemberName] string name = "")
    {
      if (!EqualityComparer<T>.Default.Equals(property, value))
      {
        NotifyPropertyChanging(name);

        property = value;

        NotifyPropertyChanged(name);
      }
    }

    protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertySelector)
    {
      var propertyChanged = PropertyChanged;
      if (propertyChanged != null)
      {
        string propertyName = GetPropertyName(propertySelector);
        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }


    #endregion // INotifyPropertyChanged Members 

    protected string GetPropertyName(LambdaExpression expression)
    {
      var memberExpression = expression.Body as MemberExpression;
      if (memberExpression == null)
      {
        throw new InvalidOperationException();
      }

      return memberExpression.Member.Name;
    }

    #region Debugging

    /// <summary>
    /// Warns the developer if this object does not have
    /// a public property with the specified name. This 
    /// method does not exist in a Release build.
    /// </summary>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName)
    {
      // Verify that the property name matches a real,  
      // public, instance property on this object.
      if (TypeDescriptor.GetProperties(this)[propertyName] == null)
      {
        string msg = "Invalid property name: " + propertyName;

        if (this.ThrowOnInvalidPropertyName)
          throw new Exception(msg);
        else
          Debug.Fail(msg);
      }
    }

    /// <summary>
    /// Returns whether an exception is thrown, or if a Debug.Fail() is used
    /// when an invalid property name is passed to the VerifyPropertyName method.
    /// The default value is false, but subclasses used by unit tests might 
    /// override this property's getter to return true.
    /// </summary>
    protected virtual bool ThrowOnInvalidPropertyName { get; set; }

    #endregion // Debugging Aides

    public event PropertyChangingEventHandler PropertyChanging;


    public void NotifyPropertyChanging(string propname)
    {
      PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propname));
    }
  }
}