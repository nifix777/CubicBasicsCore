using System;
using System.Diagnostics;
using Cubic.Core.Annotations;

namespace Cubic.Core
{
  public static class Disposable
  {
    /// <summary>
    /// Occurs when [not disposed object].
    /// </summary>
    public static event EventHandler<MessageEventArgs> NotDisposedObject;

    /// <summary>
    /// If the specified object implements <see cref="IDisposable"/>, it will be disposed, else nothing is done.
    /// </summary>
    /// <param name="item">The object to dispose.</param>
    /// <returns><c>true</c> if the object has been disposed.</returns>
    public static bool Dispose([CanBeNull] object item)
    {
      if (item is IDisposable disposable)
      {
        disposable.Dispose();
        return true;
      }

      return false;
    }


    public static void ReportNotDisposedObject([NotNull] this IDisposable obj)
    {

      var objectType = obj.GetType();
      // ReSharper disable once PossibleNullReferenceException
      var message = "Object not disposed: " + objectType.Name;

      var eventHandler = NotDisposedObject;
      if (eventHandler != null)
      {
        eventHandler(obj, new MessageEventArgs(message));
      }
      else if (Debugger.IsAttached)
      {
        throw new InvalidOperationException(message);
      }
    }

    #region IDisposable

    /// <summary>
    /// Disposes all IDisposable fields in the given <see cref="instance"/>.
    /// </summary>
    /// <param name="instance">The instance.</param>
    public static void DisposeFields(this IDisposable instance)
    {
      foreach (var disposableField in Reflection.RefelctionUtils.GetFieldsOfType<IDisposable>(instance.GetType(), instance))
      {
        disposableField?.Dispose();
      }
    }

    #endregion
  }
}