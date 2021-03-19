using Cubic.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cubic.Shared.Core.Net.Http
{
  public sealed class HttpDataAccess : DataAccess
  {
    public HttpDataAccess()
    {
      base.AccessorType = AccessorType.HTTP;
    }
    public string Scheme
    {
      get { return (string)base[nameof(Scheme)]; }
      set { base[nameof(Scheme)] = value; }
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

    public string Auth
    {
      get { return (string)base[nameof(Auth)]; }
      set { base[nameof(Auth)] = value; }
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
  }
}
