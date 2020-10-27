namespace Cubic.Core
{
  public struct ValueResult
  {
    public readonly object Value;

    public readonly string ErrorMessage;
    public bool Success {get { return string.IsNullOrEmpty(ErrorMessage); } }

    public ValueResult( object value, string errormessage = "")
    {
      Value = value;
      ErrorMessage = errormessage;
    }
  }

  public struct ValueResult<T>
  {
    public readonly T Value;

    public readonly string ErrorMessage;
    public bool Success { get { return string.IsNullOrEmpty( ErrorMessage ); } }

    public ValueResult( T value , string errormessage = "" )
    {
      Value = value;
      ErrorMessage = errormessage;
    }
  }
}