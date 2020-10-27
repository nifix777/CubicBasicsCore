using System;
using System.IO;
using Cubic.Core.Execution.Transient;
using Xunit;

namespace Cubic.Basics.Testing.Execution
{
  
  public class TransientTests
  {
    [Fact]
    public void Transient_Directory_Test()
    {
      var dirname = "MyTestDirectory";

      Assert.True(!Directory.Exists(dirname));

      using (var test = new TestEnviroment())
      {
        var dir = test.CreateDirectory(dirname);

        Assert.True(Directory.Exists(dirname));
      }

      Assert.True(!Directory.Exists(dirname));
    }

    [Fact]
    public void Transient_DirectoryAndFile_Test()
    {
      var dirname = "MyTestDirectory";
      var filename = "MyTestFile.txt";

      Assert.False(Directory.Exists(dirname));

      using (var test = new TestEnviroment())
      {
        var dir = test.CreateDirectory(dirname);

        Assert.True(Directory.Exists(dirname));

        var file = dir.CreateFile(filename, writer => writer.WriteLine("Hello World"));

        Assert.True(File.Exists(file.FilePath));

      }


      Assert.False(File.Exists(filename));
      Assert.False(Directory.Exists(dirname));
    }
  }
}
