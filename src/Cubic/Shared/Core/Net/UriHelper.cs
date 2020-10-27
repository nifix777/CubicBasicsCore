using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Net
{
  public sealed class UriHelper
  {
    public bool TryCreateWebUri(string uriString, out Uri uri, out string message)
    {
      message = null;
      if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
      {
        return ValidateWebUri(uri, out message);
      }
      else
      {
        message = $"{uriString ?? "null"} is not a valid absolute uri.";
      }
      return false;
    }

    public static void ValidateWebUri(Uri uri)
    {
      if (uri is null)
      {
        throw new ArgumentNullException(nameof(uri));
      }

      if (!ValidateWebUri(uri, out string message))
      {
        throw new ArgumentException(message);
      }
    }

    public static bool ValidateWebUri(Uri uri, out string message)
    {
      message = null;
      if (!uri.IsAbsoluteUri)
      {
        message = "The uri cannot be a relative uri.";
        return false;
      }
      if (uri.IsFile)
      {
        message = "The uri cannot be a file.";
        return false;
      }
      if (uri.IsUnc)
      {
        message = "The uri cannot be a UNC path.";
        return false;
      }
      return true;
    }
  }
}
