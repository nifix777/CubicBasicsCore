namespace Cubic.Core.Execution
{
  public interface IExecutionResult
  {
    bool IsError { get; }
    
    string Error { get; }
    
    object Result { get; } 
  }
}