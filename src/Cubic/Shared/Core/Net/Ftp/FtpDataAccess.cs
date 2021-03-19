using Cubic.Core.Data;
using System;

namespace Cubic.Shared.Core.Net.Ftp
{
  public sealed class FtpDataAccess : DataAccess
  {
    public FtpDataAccess()
    {
      base.AccessorType = AccessorType.FTP;

      if (!Contains(nameof(Port))) Port = 21;
    }
    public bool KeepAlive
    {
      get { return (bool)base[nameof(KeepAlive)]; }
      set { base[nameof(KeepAlive)] = value; }
    }

    public int Port
    {
      get { return (int)base[nameof(Port)]; }
      set { base[nameof(Port)] = value; }
    }
    public string Host
    {
      get { return (string)base[nameof(Host)]; }
      set { base[nameof(Host)] = value; }
    }

    public bool UsePassive
    {
      get { return (bool)base[nameof(UsePassive)]; }
      set { base[nameof(UsePassive)] = value; }
    }

    public bool UseBinary
    {
      get { return (bool)base[nameof(UseBinary)]; }
      set { base[nameof(UseBinary)] = value; }
    }

    public bool UseSsl
    {
      get { return (bool)base[nameof(UseSsl)]; }
      set { base[nameof(UseSsl)] = value; }
    }

    public string User
    {
      get { return (string)base[nameof(User)]; }
      set { base[nameof(User)] = value; }
    }

    public string Secret
    {
      get { return (string)base[nameof(Secret)]; }
      set { base[nameof(Secret)] = value; }
    }

    public TimeSpan Timeout
    {
      get { return (TimeSpan)base[nameof(Timeout)]; }
      set { base[nameof(Timeout)] = value; }
    }

    public int BufferSize
    {
      get { return (int)base[nameof(BufferSize)]; }
      set { base[nameof(BufferSize)] = value; }
    }


  }
}
