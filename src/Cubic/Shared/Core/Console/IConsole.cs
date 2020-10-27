using Cubic.Core.Execution;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Cubic.Core.Console
{
  public interface IConsole
  {
    bool PasswordMode { get; set; }

    bool UseHistory { get; set; }

    string[] History { get; }

    string CurrentLine { get; }

    bool CursorVisible { get; set; }

    event ConsoleCancelEventHandler CancelKeyPress;

    /// <summary>
    /// stdout
    /// </summary>
    TextWriter Out { get; }

    /// <summary>
    /// stderr
    /// </summary>
    TextWriter Error { get; }

    /// <summary>
    /// stdin
    /// </summary>
    TextReader In { get; }

    /// <summary>
    /// Is stdin piped from somewhere?
    /// </summary>
    bool IsInputRedirected { get; }

    /// <summary>
    /// Is stdout being piped to somewhere?
    /// </summary>
    bool IsOutputRedirected { get; }

    /// <summary>
    /// Is stderr being piped to somewhere?
    /// </summary>
    bool IsErrorRedirected { get; }

    /// <summary>
    /// The foreground color of output.
    /// </summary>
    ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// The background color of output.
    /// </summary>
    ConsoleColor BackgroundColor { get; set; }

    /// <summary>
    /// Resets <see cref="ForegroundColor"/> and <see cref="BackgroundColor"/>.
    /// </summary>
    void ResetColor();

    int CursorLeft { get; }
    int CursorTop { get; }
    int BufferWidth { get; }
    int BufferHeight { get; }

    void SetCursorPosition(int left, int top);
    void SetBufferSize(int width, int height);

    void Write(string value);
    void Write(char value);
    void WriteLine(string value);

    string ReadLine();
    string ReadLine(IAutoCompletionHandler handler);
    char Read();

    ConsoleKeyInfo ReadKey(bool intercept = false);
    void Read(char[] buffer, int length, int offset= 0);

    bool Ask(string question, string yes = "y", string no = "n");

    Optional<int> ReadInputNumber(int min, int max);
  }
}