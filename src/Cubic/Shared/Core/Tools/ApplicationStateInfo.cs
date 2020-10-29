using Cubic.Core.Diagnostics;
using Cubic.Core.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Cubic.Core.Tools
{
  [Serializable]
  public class ApplicationStateInfo : IList<ApplicationStateInfoItem>, ICollection<ApplicationStateInfoItem>, IEnumerable<ApplicationStateInfoItem>, IFreezable
  {
    private int firstWarningIndex = -1;

    private readonly List<ApplicationStateInfoItem> _internalList;

    private readonly int _maxEntriesForDescription;
    private readonly bool _exceptionStackframe;

    private bool _isReadOnly;

    private ApplicationStateInfo _parent;

    public ApplicationStateInfo() : this(8, true)
    {

    }

    public ApplicationStateInfo(int maxEntriesForDescription, bool exceptionStackframe)
    {
      this._maxEntriesForDescription = maxEntriesForDescription;
      this._exceptionStackframe = exceptionStackframe;

      _internalList = new List<ApplicationStateInfoItem>();
    }

    public int Count
    {
      get
      {
        return this._internalList.Count;
      }
    }

    public IEnumerable<ApplicationStateInfoItem> Errors
    {
      get
      {
        int num = 0;
        int numberOfErrors = this.NumberOfErrors;
        while (num < numberOfErrors)
        {
          yield return this._internalList[num];
          num++;
        }
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return _isReadOnly;
      }
    }

    public ApplicationStateInfoItem this[int index]
    {
      get
      {
        return this._internalList[index];
      }
      set
      {
        throw new InvalidOperationException();
      }
    }

    public int NumberOfErrors
    {
      get
      {
        if (this.firstWarningIndex != -1)
        {
          return this.firstWarningIndex;
        }
        return this._internalList.Count;
      }
    }

    public int NumberOfWarnings
    {
      get
      {
        if (this.firstWarningIndex == -1)
        {
          return 0;
        }
        return this._internalList.Count - this.firstWarningIndex;
      }
    }

    public IEnumerable<ApplicationStateInfoItem> Warnings
    {
      get
      {
        int num = 0;
        int numberOfWarnings = this.NumberOfWarnings;
        while (num < numberOfWarnings)
        {
          yield return this._internalList[num + this.firstWarningIndex];
          num++;
        }
      }
    }

    public void Add(ApplicationStateInfoItem item)
    {

      if (IsReadOnly) return;

      if (item.Severity != MessageSeverity.Error)
      {
        this._internalList.Add(item);
        if (this.firstWarningIndex == -1)
        {
          this.firstWarningIndex = this._internalList.IndexOf(item);
        }
      }
      else
      {
        if (this.firstWarningIndex > -1)
        {
          this.firstWarningIndex++;
        }
        this._internalList.Insert(0, item);
      }
      this.OnAppended(item);
      if (this._parent != null)
      {
        this._parent.Add(item);
      }
    }

    public void Add(IEnumerable<ApplicationStateInfoItem> itemCollection)
    {

      if (IsReadOnly) return;

      foreach (ApplicationStateInfoItem applicationStateInfoItem in itemCollection)
      {
        this.Add(applicationStateInfoItem);
      }
    }

    public ApplicationStateInfoItem AppendError(int number, string description, params object[] descriptionArgs)
    {
      ApplicationStateInfoItem applicationStateInfoItem = new ApplicationStateInfoItem(number, ApplicationStateInfo.FormatDescription(description, descriptionArgs), string.Empty, MessageSeverity.Error);
      this.Add(applicationStateInfoItem);
      return applicationStateInfoItem;
    }

    public ApplicationStateInfoItem AppendErrorWithSource(int number, string description, string source, params object[] descriptionArgs)
    {
      ApplicationStateInfoItem applicationStateInfoItem = new ApplicationStateInfoItem(number, ApplicationStateInfo.FormatDescription(description, descriptionArgs), source, MessageSeverity.Error);
      this.Add(applicationStateInfoItem);
      return applicationStateInfoItem;
    }

    public ApplicationStateInfoItem AppendException(Exception ex)
    {
      int exceptionErrorNumber = ApplicationStateInfo.GetExceptionErrorNumber(ex);
      ApplicationStateInfoItem applicationStateInfoItem = new ApplicationStateInfoItem(exceptionErrorNumber, ex.Message, ApplicationStateInfo.FormatExceptionSource(ex, _exceptionStackframe), MessageSeverity.Error);
      this.Add(applicationStateInfoItem);
      return applicationStateInfoItem;
    }

    public ApplicationStateInfoItem AppendException(Exception ex, string description, params object[] descriptionArgs)
    {
      int exceptionErrorNumber = ApplicationStateInfo.GetExceptionErrorNumber(ex);
      ApplicationStateInfoItem applicationStateInfoItem = new ApplicationStateInfoItem(exceptionErrorNumber, ApplicationStateInfo.FormatDescription(description, descriptionArgs), ApplicationStateInfo.FormatExceptionSource(ex, _exceptionStackframe), MessageSeverity.Error);
      this.Add(applicationStateInfoItem);
      return applicationStateInfoItem;
    }

    //public ApplicationStateInfoItem AppendVB6Error(int number, string description, string source)
    //{
    //  ApplicationStateInfoItem applicationStateInfoItem = new ApplicationStateInfoItem(MessageSeverity.Error);
    //  if (number < 0)
    //  {
    //    applicationStateInfoItem.Number = number - number;
    //    applicationStateInfoItem.Description = description;
    //    applicationStateInfoItem.Source = source;
    //  }
    //  else if (!string.IsNullOrEmpty(source))
    //  {
    //    applicationStateInfoItem.Number = number;
    //    applicationStateInfoItem.Source = source;
    //    string errorMessageWithSource = Resources.ErrorMessageWithSource;
    //    object[] objArray = new object[] { number, description, source };
    //    applicationStateInfoItem.Description = ApplicationStateInfo.FormatDescription(errorMessageWithSource, objArray);
    //  }
    //  else
    //  {
    //    applicationStateInfoItem.Number = number;
    //    string errorMessageWithoutSource = Resources.ErrorMessageWithoutSource;
    //    object[] objArray1 = new object[] { number, description };
    //    applicationStateInfoItem.Description = ApplicationStateInfo.FormatDescription(errorMessageWithoutSource, objArray1);
    //  }
    //  this.Add(applicationStateInfoItem);
    //  return applicationStateInfoItem;
    //}

    public ApplicationStateInfoItem AppendWarning(int number, string description, params object[] descriptionArgs)
    {
      return this.AppendWarningWithSource(number, description, string.Empty, descriptionArgs);
    }

    public ApplicationStateInfoItem AppendWarningWithSource(string description, string source)
    {
      return this.AppendWarningWithSource(0, description, source, null);
    }

    public ApplicationStateInfoItem AppendWarningWithSource(string description, string source, params object[] descriptionArgs)
    {
      return this.AppendWarningWithSource(0, description, source, descriptionArgs);
    }

    public ApplicationStateInfoItem AppendWarningWithSource(int number, string description, string source, params object[] descriptionArgs)
    {
      ApplicationStateInfoItem applicationStateInfoItem = new ApplicationStateInfoItem(number, ApplicationStateInfo.FormatDescription(description, descriptionArgs), source, MessageSeverity.Warning);
      this.Add(applicationStateInfoItem);
      return applicationStateInfoItem;
    }

    public void Attach(ApplicationStateInfo parent)
    {
      this.Detach();
      this._parent = parent;
      if (this._parent != null)
      {
        this._parent.Removed += new EventHandler<ApplicationStateInfoEventArgs>(this.Parent_Removed);
      }
    }

    public void Clear()
    {
      if (IsReadOnly) return;

      while (this._internalList.Count != 0)
      {
        this.RemoveAt(0);
      }
      this.firstWarningIndex = 0;
    }

    public bool Contains(ApplicationStateInfoItem item)
    {
      return this._internalList.Contains(item);
    }

    public void CopyTo(ApplicationStateInfo other)
    {
      foreach (ApplicationStateInfoItem applicationStateInfoItem in this._internalList)
      {
        other.Add(applicationStateInfoItem);
      }
    }

    public void CopyTo(ApplicationStateInfoItem[] array, int arrayIndex)
    {
      this._internalList.CopyTo(array, arrayIndex);
    }

    public void Detach()
    {
      if (this._parent != null)
      {
        this._parent.Removed -= new EventHandler<ApplicationStateInfoEventArgs>(this.Parent_Removed);
      }
      this._parent = null;
    }

    private static string FormatDescription(string description, params object[] descriptionArgs)
    {
      if (descriptionArgs != null)
      {
        //description = StringHelper.Format(description, descriptionArgs);

        if (descriptionArgs.Length > 0)
        {
          description = string.Format(description, descriptionArgs);
          //description = description.Replace("$&$", ConversionHelper.ToString(descriptionArgs[0]));
        }
      }
      return description.Replace("|", Environment.NewLine);
    }

    private static string FormatExceptionSource(Exception ex, bool useStackTrace = true)
    {
      string empty;

      try
      {

        if(useStackTrace)
        {
          StackTrace stackTrace = new StackTrace(ex, true);
          MethodBase method = stackTrace.GetFrame(0).GetMethod();
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          object[] source = new object[] { ex.Source, method.DeclaringType, method.Name };
          empty = string.Format(invariantCulture, "In:{0} at:{1}.{2}", source);
        }
        else
        {
          empty = ex.GetAllMessages();
        }


      }
      catch (Exception exception)
      {
        empty = string.Empty;
      }
      return empty;
    }

    public static string GetLevelName(MessageSeverity severity)
    {
      switch (severity)
      {
        case MessageSeverity.Verbose:
          return "TRACE";
        case MessageSeverity.Information:
          return "INFO";
        case MessageSeverity.Warning:
          return "WARNING";
        case MessageSeverity.Error:
          return "ERROR";
        case MessageSeverity.Critical:
          return "CRITICAL";
        default:
          return string.Empty;
      }
    }

    public string GetDescriptionSummary()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Count == 1)
      {
        return this[0].Description;
      }
      int num = 0;
      int num1 = Math.Min(_maxEntriesForDescription, this.Count);
      while (num < num1)
      {
        if (num > 0)
        {
          stringBuilder.Append(Environment.NewLine);
        }
        if(!this[num].Source.IsNullOrEmpty())
        {

        }
        else
        {
          stringBuilder.AppendFormat("-> {0}", this[num].Description);
        }

        num++;
      }
      if (this.Count > _maxEntriesForDescription)
      {
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append("...");
      }
      return stringBuilder.ToString();
    }

    public IEnumerator<ApplicationStateInfoItem> GetEnumerator()
    {
      return this._internalList.GetEnumerator();
    }

    public ApplicationStateInfoItem GetError(int index)
    {
      return this._internalList[index];
    }

    private static int GetExceptionErrorNumber(Exception ex)
    {
      return ex.HResult;
    }

    public ApplicationStateInfoItem GetWarning(int index)
    {
      return this._internalList[index + this.firstWarningIndex];
    }

    public int IndexOf(ApplicationStateInfoItem item)
    {
      return this._internalList.IndexOf(item);
    }

    public void MoveTo(ApplicationStateInfo other)
    {
      this.CopyTo(other);

      if (IsReadOnly) return;

      this.Clear();
    }

    protected void OnAppended(ApplicationStateInfoItem item)
    {
      this.Appended?.Invoke(this, new ApplicationStateInfoEventArgs(item));
    }

    protected void OnRemoved(ApplicationStateInfoItem item)
    {
      this.Removed?.Invoke(this, new ApplicationStateInfoEventArgs(item));
    }

    private void Parent_Removed(object sender, ApplicationStateInfoEventArgs e)
    {
      this.Remove(e.Item);
    }

    public void Remove(ApplicationStateInfoItem item)
    {
      if (IsReadOnly) return;

      int num = this.IndexOf(item);
      if (num > -1)
      {
        this.RemoveAt(num);
      }
    }

    public void RemoveAt(int index)
    {
      if (IsReadOnly) return;

      ApplicationStateInfoItem item = this._internalList[index];
      if (item.Severity == MessageSeverity.Error)
      {
        if (this.firstWarningIndex > -1)
        {
          this.firstWarningIndex--;
        }
      }
      else if (this.NumberOfWarnings == 1)
      {
        this.firstWarningIndex = -1;
      }
      this._internalList.RemoveAt(index);
      if (this._parent != null)
      {
        this._parent.Remove(item);
      }
      this.OnRemoved(item);
    }

    bool System.Collections.Generic.ICollection<ApplicationStateInfoItem>.Remove(ApplicationStateInfoItem item)
    {
      if (IsReadOnly) return false;

      if (!this._internalList.Contains(item))
      {
        return false;
      }
      this.RemoveAt(this._internalList.IndexOf(item));
      return true;
    }

    void System.Collections.Generic.IList<ApplicationStateInfoItem>.Insert(int index, ApplicationStateInfoItem item)
    {
      if (IsReadOnly) return;

      this._internalList.Insert(index, item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _internalList.GetEnumerator();
    }

    public void Freeze()
    {
      _isReadOnly = true;
    }

    public event EventHandler<ApplicationStateInfoEventArgs> Appended;

    public event EventHandler<ApplicationStateInfoEventArgs> Removed;
  }
}
