using System;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Collections;

namespace Cubic.Core.Console
{
  public static class ConsoleExtensions
  {

    private static object _sync = new object();

    internal static bool IsTyping(IConsole console)
    {
      return console.CursorLeft != 0;
    }

    public static void PrependLine(IConsole console, string message)
    {
      lock (_sync)
      {
        if (IsTyping(console))
        {
          var currentCursor = new CursorPsoition(console.CursorLeft, console.CursorTop);

          console.SetCursorPosition(0, currentCursor.Top - 1);

          console.WriteLine(message);

          console.SetCursorPosition(currentCursor.Left, currentCursor.Top);

        }
      }
    }

    internal struct CursorPsoition
    {

      public CursorPsoition(int left, int top)
      {
        Left = left;
        Top = top;
      }

      internal int Left { get; set; }
      internal int Top { get; set; }
    }

    //public static string Input(string output = null)
    //{
    //  if (!output.IsNullOrEmpty())
    //  {
    //    System.Console.Write(output);
    //  }
    //  return System.Console.ReadLine();
    //}

    public static string Input(this IConsole console, string output = null)
    {
      if (!output.IsNullOrEmpty())
      {
        console.Write(output);
      }
      return console.ReadLine();
    }

    public static bool IsNewLine(this IConsole console, out char input)
    {
      input = console.Read();
      return input != '\r';
    }

    public static bool IsEndOfLine(this IConsole console)
    {
      return console.CursorLeft == console.BufferWidth;
    }

    public static bool IsBeginOfLine(this IConsole console)
    {
      return 0 == console.CursorLeft;
    }

    public static void DeleteLine(this IConsole console)
    {
      while(console.IsBeginOfLine())
      {
        console.Delete();
      }
    }

    public static void Delete(this IConsole console)
    {
      if (!console.IsBeginOfLine())
      {
        var newLIne = console.CurrentLine.Substring(0, console.CurrentLine.Length - 1);
        console.ReplaceLine(newLIne);
      }
    }

    public static void ClearLine(this IConsole console)
    {
      console.ReplaceLine();
    }

    public static void Write(this IConsole console, Exception error)
    {
      var color = console.ForegroundColor;
      console.ForegroundColor = ConsoleColor.Red;
      console.Write(error.ToString());
      console.ForegroundColor = color;
    }

    public static void ReplaceLine(this IConsole console, string newLIne = null)
    {
      var currentCursor = console.CursorTop;
      console.SetCursorPosition(0, console.CursorTop);

      if (newLIne.IsNullOrEmpty())
      {
        console.Write(new string(' ', console.BufferWidth));
        console.SetCursorPosition(0, currentCursor);
      }
      else
      {
        console.Write(newLIne);
        console.SetCursorPosition(newLIne.Length, currentCursor);
      }

    }

    public static CancellationToken GetCtrlCToken(this IConsole console)
    {
      var cts = new CancellationTokenSource();
      console.CancelKeyPress += (sender, args) =>
      {
        if (cts.IsCancellationRequested)
        {
          // Terminate forcibly, the user pressed Ctrl-C a second time
          args.Cancel = false;
        }
        else
        {
          // Don't terminate, just trip the token
          args.Cancel = true;
          cts.Cancel();
        }
      };
      return cts.Token;
    }

    public static Task WaitForCtrlCAsync(this IConsole console)
    {
      var tcs = new TaskCompletionSource<object>();
      console.CancelKeyPress += (sender, args) =>
      {
        // Don't terminate, just trip the task
        args.Cancel = true;
        tcs.TrySetResult(null);
      };
      return tcs.Task;
    }
  }
}