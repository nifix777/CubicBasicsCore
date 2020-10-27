using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cubic.Core.Collections;


namespace Cubic.Core.IO
{
  public static class FileExtensions
  {
    public const int DefaultBufferSize = 4096;
    private const char NewChar = '\0';

    public static IDisposable LockFile(string filePath)
    {
      var @lock = new FileLock(filePath);
      @lock.Open();
      return @lock;
    }

    public static StreamReader AsyncStream(string path, Encoding encoding)
    {
      FileStream stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);

      return new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
    }

    public static Task<string> ReadAllTextAsync(string path, Encoding encoding)
    {
      using (var reader = AsyncStream(path, encoding))
      {
        return reader.ReadToEndAsync();
      }
    }

    /// <summary>
    /// Returns a relative path string from a full path based on a base path
    /// provided.
    /// </summary>
    /// <param name="fullPath">The path to convert. Can be either a file or a directory</param>
    /// <param name="basePath">The base path on which relative processing is based. Should be a directory.</param>
    /// <returns>
    /// String of the relative path.
    /// 
    /// Examples of returned values:
    ///  test.txt, ..\test.txt, ..\..\..\test.txt, ., .., subdir\test.txt
    /// </returns>
    public static string GetRelativePath(string fullPath, string basePath)
    {
      // Require trailing backslash for path
      if (!basePath.EndsWith("\\"))
        basePath += "\\";

      Uri baseUri = new Uri(basePath);
      Uri fullUri = new Uri(fullPath);

      Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

      // Uri's use forward slashes so convert back to backward slashes
      return relativeUri.ToString().Replace("/", "\\");

    }

    public static IEnumerable<byte[]> GetChunks(this FileStream fileStream, double kilobytesPerChunk)
    {
      int chunkSize = (int) kilobytesPerChunk*1024;

      int position = 0;

      while (fileStream.Position <= fileStream.Length -1)
      {
        var chunk = BufferPool.Shared.Rent(chunkSize);
        var filledChunkSize = fileStream.Read(chunk, position, chunkSize);
        position = position + filledChunkSize;
        yield return chunk;
      }
    }

    public static bool IsFileSystemException(Exception exception)
    {
      return (exception is IOException | exception is UnauthorizedAccessException | exception is PathTooLongException |
              exception is DirectoryNotFoundException | exception is SecurityException);
    }

    public static bool IsUncPath(string uncpath)
    {
      return UncPattern.IsMatch(uncpath);
    }

    // regular expression used to match file-specs beginning with "<drive letter>:" 
    internal static readonly Regex DrivePattern = new Regex(@"^[A-Za-z]:", RegexOptions.Compiled);

    // regular expression used to match UNC paths beginning with "\\<server>\<share>"
    internal static readonly Regex UncPattern =
      new Regex(
        string.Format(CultureInfo.InvariantCulture, @"^[\{0}\{1}][\{0}\{1}][^\{0}\{1}]+[\{0}\{1}][^\{0}\{1}]+",
          Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), RegexOptions.Compiled);



    public static void CopyDirectory(string sourceDirectory, string destinationDirectory)
    {
      if (!Directory.Exists(sourceDirectory))
      {
        throw new ArgumentException("Source directory doesn't exist:" + sourceDirectory);
      }

      sourceDirectory = sourceDirectory.TrimSlash();

      if (string.IsNullOrEmpty(destinationDirectory))
      {
        throw new ArgumentNullException(nameof(destinationDirectory));
      }

      destinationDirectory = destinationDirectory.TrimSlash();

      var files = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
      foreach (var file in files)
      {
        var relative = file.Substring(sourceDirectory.Length + 1);
        var destination = Path.Combine(destinationDirectory, relative);
        CopyFile(file, destination);
      }
    }

    public static void CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = false)
    {
      if (!File.Exists(sourceFilePath))
      {
        return;
      }

      if (!overwrite && File.Exists(destinationFilePath))
      {
        return;
      }

      var directory = Path.GetDirectoryName(destinationFilePath);
      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      File.Copy(sourceFilePath, destinationFilePath, overwrite);
      File.SetAttributes(destinationFilePath, File.GetAttributes(destinationFilePath) & ~FileAttributes.ReadOnly);
    }

    public static string RemoveInvalidFileNameChars(this string filename)
    {
      foreach(var character in Path.GetInvalidFileNameChars())
      {
        filename.Replace(character, NewChar);
      }

      return filename;
    }

    private static char[] InvalidFileSystemCharacters { get; } = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToArray();

    public static string RemoveInvalidPathChars(this string path)
    {
      for (int i = 0; i < InvalidFileSystemCharacters.Length; i++)
      {
        char character = InvalidFileSystemCharacters[i];
        path.Replace(character, NewChar);
      }

      return path;
    }
  }
}
