using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Cubic.Core.Collections;

namespace Cubic.Core.Net.Http
{
  public static class HttpExtensions
  {
    public async static Task<XmlDocument> GetXmlAsync(this HttpClient client, Uri uri)
    {
      using (var xmlStream = await client.GetStreamAsync(uri))
      {
        var doc = new XmlDocument();
        doc.Load(xmlStream);
        return doc;
      }
    }

    public async static Task<string> GetFileAsync(this HttpClient client, Uri uri, string newFileName = null)
    {
      var fileName = newFileName ?? Path.GetTempFileName();

      using (var downloadStream = await client.GetStreamAsync(uri))
      {

        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, Constants.LargeBufferSize, true))
        {
          await downloadStream.CopyToAsync(fileStream);
        }
      }

      return fileName;
    }

    public static IAsyncEnumerator2<string> GetFilesAsync(this HttpClient client, Uri[] uris)
    {
      var index = 0;
      return new AsyncEnumrator2<string, int>(i =>
       {
         if (i < uris.Length)
         {
           return client.GetFileAsync(uris[i]);
         }

         return null;
       }, index
        );
    }
  }
}