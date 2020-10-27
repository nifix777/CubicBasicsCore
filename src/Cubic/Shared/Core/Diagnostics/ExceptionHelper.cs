using System;


namespace Cubic.Core.Diagnostics
{
  public static class ExceptionHelper
  {

    public static TException Create<TException>(string message, params object[] args) where TException : Exception
    {
        string msg = args == null ? message : string.Format(message, args);
        return (TException)Activator.CreateInstance(typeof(TException),  msg);
    }

    public static Exception Create(string message, object[] args = null)
    {
      string msg = args == null ? message : string.Format(message, args);
      return new Exception(msg);
    }

    public static Exception Create(string message, object[] args, Exception innerException)
    {
      string msg = args == null ? message : string.Format(message, args);
      return new Exception(msg, innerException);
    }

    public static void Throw(Exception exception)
    {
      throw exception;
    }

    internal static void ThrowObjectDisposedException(object disposedObject)
    {
      throw new ObjectDisposedException(disposedObject.ToString());
    }
  }
}