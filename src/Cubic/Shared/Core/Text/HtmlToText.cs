namespace Cubic.Core.Text
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Text;

  namespace Sagede.Core.Tools
  {
    internal class HtmlToText
    {
      protected static Dictionary<string, string> _tags;

      protected static HashSet<string> _ignoreTags;

      protected HtmlToText.TextBuilder _text;

      protected string _html;

      protected int _pos;

      protected bool EndOfText
      {
        get
        {
          return this._pos >= this._html.Length;
        }
      }

      static HtmlToText()
      {
        HtmlToText._tags = new Dictionary<string, string>()
            {
                { "address", "\n" },
                { "blockquote", "\n" },
                { "div", "\n" },
                { "dl", "\n" },
                { "fieldset", "\n" },
                { "form", "\n" },
                { "h1", "\n" },
                { "/h1", "\n" },
                { "h2", "\n" },
                { "/h2", "\n" },
                { "h3", "\n" },
                { "/h3", "\n" },
                { "h4", "\n" },
                { "/h4", "\n" },
                { "h5", "\n" },
                { "/h5", "\n" },
                { "h6", "\n" },
                { "/h6", "\n" },
                { "p", "\n" },
                { "/p", "\n" },
                { "table", "\n" },
                { "/table", "\n" },
                { "ul", "\n" },
                { "/ul", "\n" },
                { "ol", "\n" },
                { "/ol", "\n" },
                { "/li", "\n" },
                { "br", "\n" },
                { "/td", "\t" },
                { "/tr", "\n" },
                { "/pre", "\n" }
            };
        HtmlToText._ignoreTags = new HashSet<string>();
        HtmlToText._ignoreTags.Add("script");
        HtmlToText._ignoreTags.Add("noscript");
        HtmlToText._ignoreTags.Add("style");
        HtmlToText._ignoreTags.Add("object");
      }

      public HtmlToText()
      {
      }

      public string Convert(string html)
      {
        bool flag;
        string str;
        this._text = new HtmlToText.TextBuilder();
        this._html = html;
        this._pos = 0;
        while (!this.EndOfText)
        {
          if (this.Peek() == '<')
          {
            string str1 = this.ParseTag(out flag);
            if (str1 == "body")
            {
              this._text.Clear();
            }
            else if (str1 == "/body")
            {
              this._pos = this._html.Length;
            }
            else if (str1 == "pre")
            {
              this._text.Preformatted = true;
              this.EatWhitespaceToNextLine();
            }
            else if (str1 == "/pre")
            {
              this._text.Preformatted = false;
            }
            if (HtmlToText._tags.TryGetValue(str1, out str))
            {
              this._text.Write(str);
            }
            if (!HtmlToText._ignoreTags.Contains(str1))
            {
              continue;
            }
            this.EatInnerContent(str1);
          }
          else if (!char.IsWhiteSpace(this.Peek()))
          {
            this._text.Write(this.Peek());
            this.MoveAhead();
          }
          else
          {
            this._text.Write((this._text.Preformatted ? this.Peek() : ' '));
            this.MoveAhead();
          }
        }
        return WebUtility.HtmlDecode(this._text.ToString());
      }

      protected void EatInnerContent(string tag)
      {
        bool flag;
        string str = string.Concat("/", tag);
        while (!this.EndOfText)
        {
          if (this.Peek() != '<')
          {
            this.MoveAhead();
          }
          else
          {
            if (this.ParseTag(out flag) == str)
            {
              return;
            }
            if (flag || tag.StartsWith("/"))
            {
              continue;
            }
            this.EatInnerContent(tag);
          }
        }
      }

      protected void EatQuotedValue()
      {
        char chr = this.Peek();
        if (chr == '\"' || chr == '\'')
        {
          this.MoveAhead();
          string str = this._html;
          char[] chrArray = new char[] { chr, '\r', '\n' };
          this._pos = str.IndexOfAny(chrArray, this._pos);
          if (this._pos < 0)
          {
            this._pos = this._html.Length;
            return;
          }
          this.MoveAhead();
        }
      }

      protected void EatWhitespace()
      {
        while (char.IsWhiteSpace(this.Peek()))
        {
          this.MoveAhead();
        }
      }

      protected void EatWhitespaceToNextLine()
      {
        while (char.IsWhiteSpace(this.Peek()))
        {
          char chr = this.Peek();
          this.MoveAhead();
          if (chr != '\n')
          {
            continue;
          }
          return;
        }
      }

      protected void MoveAhead()
      {
        this._pos = Math.Min(this._pos + 1, this._html.Length);
      }

      protected string ParseTag(out bool selfClosing)
      {
        string empty = string.Empty;
        selfClosing = false;
        if (this.Peek() == '<')
        {
          this.MoveAhead();
          this.EatWhitespace();
          int num = this._pos;
          if (this.Peek() == '/')
          {
            this.MoveAhead();
          }
          while (!this.EndOfText && !char.IsWhiteSpace(this.Peek()) && this.Peek() != '/' && this.Peek() != '>')
          {
            this.MoveAhead();
          }
          empty = this._html.Substring(num, this._pos - num).ToLower();
          while (!this.EndOfText && this.Peek() != '>')
          {
            if (this.Peek() == '\"' || this.Peek() == '\'')
            {
              this.EatQuotedValue();
            }
            else
            {
              if (this.Peek() == '/')
              {
                selfClosing = true;
              }
              this.MoveAhead();
            }
          }
          this.MoveAhead();
        }
        return empty;
      }

      protected char Peek()
      {
        if (this._pos >= this._html.Length)
        {
          return '\0';
        }
        return this._html[this._pos];
      }

      protected class TextBuilder
      {
        private StringBuilder _text;

        private StringBuilder _currLine;

        private int _emptyLines;

        private bool _preformatted;

        public bool Preformatted
        {
          get
          {
            return this._preformatted;
          }
          set
          {
            if (value)
            {
              if (this._currLine.Length > 0)
              {
                this.FlushCurrLine();
              }
              this._emptyLines = 0;
            }
            this._preformatted = value;
          }
        }

        public TextBuilder()
        {
          this._text = new StringBuilder();
          this._currLine = new StringBuilder();
          this._emptyLines = 0;
          this._preformatted = false;
        }

        public void Clear()
        {
          this._text.Length = 0;
          this._currLine.Length = 0;
          this._emptyLines = 0;
        }

        protected void FlushCurrLine()
        {
          string str = this._currLine.ToString().Trim();
          if (str.Replace("&nbsp;", string.Empty).Length != 0)
          {
            this._emptyLines = 0;
            this._text.AppendLine(str);
          }
          else
          {
            this._emptyLines++;
            if (this._emptyLines < 2 && this._text.Length > 0)
            {
              this._text.AppendLine(str);
            }
          }
          this._currLine.Length = 0;
        }

        public override string ToString()
        {
          if (this._currLine.Length > 0)
          {
            this.FlushCurrLine();
          }
          return this._text.ToString();
        }

        public void Write(string s)
        {
          string str = s;
          for (int i = 0; i < str.Length; i++)
          {
            this.Write(str[i]);
          }
        }

        public void Write(char c)
        {
          if (this._preformatted)
          {
            this._text.Append(c);
            return;
          }
          if (c == '\r')
          {
            return;
          }
          if (c == '\n')
          {
            this.FlushCurrLine();
            return;
          }
          if (!char.IsWhiteSpace(c))
          {
            this._currLine.Append(c);
          }
          else
          {
            int length = this._currLine.Length;
            if (length == 0 || !char.IsWhiteSpace(this._currLine[length - 1]))
            {
              this._currLine.Append(' ');
              return;
            }
          }
        }
      }
    }
  }
}