using System;
using System.IO;
using Cubic.Core;
using Cubic.Core.Tools.Options;
using Xunit;

namespace Cubic.Basics.Testing.Tools
{
  
  public class OptionTests
  {
    [Fact]
    public void Parse_Option_string_Test()
    {
      var commandline = "-v -name=Warning";

      var switchVal = false;
      var serveritiy = MessageSeverity.Verbose;

      var options = new OptionSet()
        .Add("v", s => switchVal = true)
        .Add("name=", s => serveritiy = s.ToEnum<MessageSeverity>());

      options.Parse(commandline.Split(Constants.Space));

      Assert.True(switchVal);
      Assert.Equal(serveritiy, MessageSeverity.Warning);
    }

    [Fact]
    public void Parse_Option_Int_Test()
    {
      var commandline = "-level=3";

      var level = 0;

      var options = new OptionSet()
        .Add<int>("level=", lvl => level = lvl);

      options.Parse(commandline.Split(Constants.Space));

      Assert.Equal(3, level);
    }

    [Fact]
    public void Parse_ShowHelp_Test()
    {
      var commandline = "-h";

      var showHelp = false;
      var stream = new MemoryStream();

      using (var writer = new StreamWriter(stream))
      {
        var options = new OptionSet()
          .Add("h|help|?", "Shows a Help-Text", s => showHelp = true);

        options.Parse(commandline.Split(Constants.Space));

        Assert.True(showHelp);

        if (showHelp)
        {
          options.WriteOptionDescriptions(writer);
          
        }

        writer.Flush();

        Assert.True(stream.Length > 0);
      }


    }
  }
}
