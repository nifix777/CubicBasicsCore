using System.Text;

namespace Cubic.Core.Console
{
  public interface IAutoCompletionHandler
  {
    string GetSuggestion(StringBuilder builder, string input);
  }
}