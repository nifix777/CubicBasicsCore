using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cubic.Core.Diagnostics;
using Cubic.Core.Execution;
using Cubic.Core.Text;

namespace Cubic.Core.Console
{
  public class SystemConsole : IConsole
  {
    /// <summary>
    /// A shared instance of <see cref="PhysicalConsole"/>.
    /// </summary>
    public static IConsole Singleton { get; } = new SystemConsole();

    private Stack<string> _historyStack;

    private string _currentLine;

    private char _passwordChar = '#';

    public event ConsoleCancelEventHandler CancelKeyPress
    {
      add
      {
        System.Console.CancelKeyPress += value;
      }
      remove
      {
        System.Console.CancelKeyPress -= value;
      }
    }

    public SystemConsole()
    {
      _historyStack = new Stack<string>();

      
    } 
    public bool PasswordMode { get; set; }


    public bool UseHistory { get; set; }
    public string[] History => _historyStack.ToArray();
    public string CurrentLine => _currentLine;

    public bool CursorVisible
    {
      get { return System.Console.CursorVisible; }
      set { System.Console.CursorVisible = value; }
    }

    public int CursorLeft => System.Console.CursorLeft;
    public int CursorTop => System.Console.CursorTop;
    public int BufferWidth => System.Console.BufferWidth;
    public int BufferHeight => System.Console.BufferHeight;

    public TextWriter Out => System.Console.Out;

    public TextWriter Error => System.Console.Error;

    public TextReader In => System.Console.In;

    public bool IsInputRedirected => System.Console.IsInputRedirected;

    public bool IsOutputRedirected => System.Console.IsOutputRedirected;

    public bool IsErrorRedirected => System.Console.IsErrorRedirected;

    public ConsoleColor ForegroundColor { get => System.Console.ForegroundColor; set => System.Console.ForegroundColor = value; }
    public ConsoleColor BackgroundColor { get => System.Console.BackgroundColor; set => System.Console.BackgroundColor = value; }

    public void SetCursorPosition(int left, int top)
    {
      System.Console.SetCursorPosition(left, top);
    }

    public void SetBufferSize(int width, int height)
    {
      System.Console.SetBufferSize(width, height);
    }

    public void Write(string value)
    {
      if (PasswordMode)
      {
        _currentLine = _currentLine + value;
        System.Console.Write(new string(_passwordChar, value.Length));
      }
      else
      {
        System.Console.Write(value);
      }
    }

    public void WriteLine(string value)
    {
      if (PasswordMode)
      {
        _currentLine = _currentLine + value;
        System.Console.WriteLine(new string(_passwordChar, value.Length));
      }
      else
      {
        System.Console.WriteLine(value);
      }
    }

    public void Write(char value)
    {
      System.Console.Write(value);
    }

    public string ReadLine()
    {
      var input = System.Console.ReadLine();

      _currentLine = input;

      if (input.IsNullOrEmpty())
      {
        return input;
      }

      if (UseHistory)
      {
        _historyStack.Push(input);
      }

      if (PasswordMode)
      {

        this.DeleteLine();
        this.WriteLine(new string(_passwordChar, input.Length));
      }

      return input;
    }

    public string ReadLine(IAutoCompletionHandler handler)
    {
      Guard.AgainstNull(handler, nameof(handler));

      var builder = new StringBuilder();
      var input = this.ReadKey(intercept: true);

      while (input.Key != ConsoleKey.Enter)
      {
        var currentInput = builder.ToString();
        if (input.Key == ConsoleKey.Tab)
        {
          var match = handler.GetSuggestion(builder, currentInput);
          if (match.IsNullOrEmpty())
          {
            input = this.ReadKey(intercept: true);
            continue;
          }

          this.ClearLine();
          builder.Clear();

          this.Write(match);
          builder.Append(match);
        }
        else
        {
          if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
          {
            builder.Remove(builder.Length - 1, 1);
            this.ClearLine();

            currentInput = currentInput.Remove(currentInput.Length - 1);
            this.Write(currentInput);
          }
          else
          {
            var key = input.KeyChar;
            builder.Append(key);
            this.Write(key);
          }
        }

        input = this.ReadKey(intercept: true);
      }


      return builder.ToString();
    }



    public char Read()
    {
      var input = ReadChar();

      if (PasswordMode)
      {
        _currentLine = string.Concat(_currentLine, input);
        this.Delete();
        this.Write(_passwordChar.ToString());
      }
      else
      {
        _currentLine = string.Concat(_currentLine, input);
      }

      return input;
    }

    private char ReadChar()
    {
      try
      {
        return Convert.ToChar(System.Console.Read());
      }
      catch
      {
        return Char.MinValue;
      }
    }

    public ConsoleKeyInfo ReadKey(bool intercept = false)
    {
      return System.Console.ReadKey(intercept);
    }


    private string ReadUntil(char until)
    {
      StringBuilder buffer = new StringBuilder();
      var input = this.ReadChar();

      while (input != until)
      {
        buffer.Append(input);
        input = this.ReadChar();
      }

      return buffer.ToString();
    }

    private string ReadUntil(params ConsoleKey[] keys)
    {
      StringBuilder buffer = new StringBuilder();
      var input = this.ReadKey(true);

      while (!keys.Contains(input.Key))
      {
        buffer.Append(input.KeyChar);
        input = this.ReadKey(true);
      }

      return buffer.ToString();
    } 

    public void Read(char[] buffer, int length, int offset = 0)
    {
      var counter = 0;
      var index = offset + counter;

      while (counter < length)
      {
        var input = this.Read();
        buffer[index] = input;
        counter++;
        index = offset + counter;
      }
    }

    public void ResetColor()
    {
      System.Console.ResetColor();
    }

    public bool? Ask(string question, string yes = "y", string no = "n")
    {
      this.WriteLine(question);
      while(true)
      {
        var line = this.ReadLine().ToLowerInvariant()?.Trim();

        if (yes == line) return true;

        if (no == line) return false;

        return null;
      }
    }

    bool IConsole.Ask(string question, string yes, string no)
    {
      throw new NotImplementedException();
    }

    public Optional<int> ReadInputNumber(int min, int max)
    {
      var defaultColor = this.ForegroundColor;
      while (true)
      {
        this.ForegroundColor = ConsoleColor.Yellow;
        this.Write(">");
        this.ForegroundColor = defaultColor;
        var line = this.ReadLine();
        if (int.TryParse(line, out var result) && result >= min && result <= max)
        {
          return result;
        }

        if (line?.Trim() == "exit" || line?.Trim() == "quit")
        {
          break;
        }
      }

      return Optional<int>.None;
    }
  }
}