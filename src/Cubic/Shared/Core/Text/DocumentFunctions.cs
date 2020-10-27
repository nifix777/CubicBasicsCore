using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cubic.Core.Text
{
  public static class DocumentFunctions
  {
    public static async Task<IEnumerable<TextChange>> GetTextChanges(string oldText, string newText)
    {
      await Task.Delay(0);

      var changes = new List<TextChange>();

      if (oldText.IsNullOrEmpty() || newText.IsNullOrEmpty()) return Enumerable.Empty<TextChange>().ToList();

      var oldSegments = Enumerable.ToArray(oldText.SplitInSegments());

      var newSegments = Enumerable.ToArray(newText.SplitInSegments());


      for (int i = 0; i < oldSegments.Length; i++)
      {
        if (i < newSegments.Length)
        {
          if (!oldSegments[i].Equals( newSegments[i]))
            changes.Add(new TextChange( oldSegments[i], newSegments[i]));
        }
        else
        {
          changes.Add(new TextChange(oldSegments[i], null));
        }
      }

      return changes;
    }

    public static async Task<IEnumerable<TextChange>> GetFileTextChanges(string oldFile, string newFile)
    {
      using (var oldreader = File.OpenText(oldFile))
      using (var newreader = File.OpenText(oldFile))
      {
        var oldText = await oldreader.ReadToEndAsync();
        var newText = await newreader.ReadToEndAsync();

        return await GetTextChanges(oldText, newText);
      }
    }
  }
}