using Cubic.Core.Text;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Tools
{
  [Serializable]
  public class ApplicationStateInfoItem
  {
    public string Description { get; set; }

    public string Detail { get; set; }

    public Guid Id { get; }

    public int Number { get; set; }

    public MessageSeverity Severity { get; }

    public string Source { get; set; }

    public ApplicationStateInfoItem() : this(MessageSeverity.Error)
    {
    }

    public ApplicationStateInfoItem(MessageSeverity severityType) : this(0, string.Empty, string.Empty, severityType)
    {
    }

    public ApplicationStateInfoItem(int number, string description, string source, MessageSeverity severityType)
    {
      this.Number = number;
      this.Description = description;
      this.Source = source;
      this.Severity = severityType;
      this.Id = Guid.NewGuid();
    }

    public override string ToString()
    {
      var text = new StringBuilder();

      if(Number == 0)
      {
        text.AppendFormat("{0}: {1}", ApplicationStateInfo.GetLevelName(Severity), Description);
      }
      else
      {
        text.AppendFormat("{0}: {1}: {2}", ApplicationStateInfo.GetLevelName(Severity), Number, Description);
      }



      if (!Detail.IsNullOrEmpty())
      {
        text.Append($" : {Detail}");
      }

      if (!Source.IsNullOrEmpty())
      {
        text.Append($" in {Source}");
      }

      return text.ToString();
    }
  }
}
