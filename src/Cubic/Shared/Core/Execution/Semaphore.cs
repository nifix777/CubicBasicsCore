using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Execution
{
  public class Semaphore : IDisposable
  {
    private ISemaphoreProvider _semaphoreProvider;

    private bool _disposed;

    private readonly string _name;

    private readonly string _atom;

    private readonly SemaphoreLockMode _mode;

    private Semaphore.SemaphoreState _currentState;

    public string Atom
    {
      get
      {
        return this._atom;
      }
    }

    public bool Failed
    {
      get
      {
        return this._currentState != Semaphore.SemaphoreState.Succeeded;
      }
    }

    public string LockDescription
    {
      get
      {
        if (this._currentState != Semaphore.SemaphoreState.Failed)
        {
          return string.Empty;
        }
        StringBuilder stringBuilder = new StringBuilder(this._name);
        if (!string.IsNullOrEmpty(this._atom))
        {
          stringBuilder.AppendFormat(" [{0}]", this._atom);
        }
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        //string semaphoreLockDescription = Resources.SemaphoreLockDescription;
        string semaphoreLockDescription = "Konkurrierender Zugriff auf '{0}' durch Benutzer '{1}' auf Host '{2}'";
        object[] lockUser = new object[] { stringBuilder, this.LockUser, this.LockStation };
        return string.Format(invariantCulture, semaphoreLockDescription, lockUser);
      }
    }

    public string LockStation
    {
      get
      {
        return this._semaphoreProvider.LockStation;
      }
    }

    public string LockUser
    {
      get
      {
        return this._semaphoreProvider.LockUser;
      }
    }

    public SemaphoreLockMode Mode
    {
      get
      {
        return this._mode;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public Semaphore(ISemaphoreProvider semaphoreProvider, string name, string atom, SemaphoreLockMode mode)
    {
      if (semaphoreProvider == null)
      {
        throw new ArgumentNullException("semaphoreProvider");
      }
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException("name");
      }
      this._semaphoreProvider = semaphoreProvider;
      this._name = name;
      this._atom = atom;
      this._mode = mode;
      this._currentState = Semaphore.SemaphoreState.Released;
    }

    public async Task<bool> AquireLock()
    {
      if ( !await this._semaphoreProvider.SetLock(this._name, this._atom, this._mode))
      {
        this._currentState = Semaphore.SemaphoreState.Failed;
      }
      else
      {
        this._currentState = Semaphore.SemaphoreState.Succeeded;
      }
      return this._currentState == Semaphore.SemaphoreState.Succeeded;
    }

    public static async Task<Semaphore> AquireSemaphore(ISemaphoreProvider semaphoreProvider, string name, string atom, SemaphoreLockMode mode)
    {
      Semaphore semaphore = new Semaphore(semaphoreProvider, name, atom, mode);
      await  semaphore.AquireLock();
      return semaphore;
    }

    public static Task<Semaphore> AquireSemaphore(ISemaphoreProvider semaphoreProvider, string name, SemaphoreLockMode mode)
    {
      return Semaphore.AquireSemaphore(semaphoreProvider, name, string.Empty, mode);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
      {
        return;
      }
      this.Release();
      this._disposed = true;
      this._semaphoreProvider = null;
    }

    ~Semaphore()
    {
      this.Dispose(false);
    }

    protected virtual void OnReleased()
    {
      this.Released?.Invoke(this, new EventData<Semaphore>(this));
    }

    public void Release()
    {
      bool flag = this._currentState == Semaphore.SemaphoreState.Succeeded;
      if (flag)
      {
        this._semaphoreProvider.ReleaseLock(this._name, this._atom, this._mode);
      }
      this._currentState = Semaphore.SemaphoreState.Released;
      if (flag)
      {
        this.OnReleased();
      }
    }

    public async Task<bool> Retry()
    {
      if (this._currentState == Semaphore.SemaphoreState.Succeeded)
      {
        return true;
      }
      return await this.AquireLock();
    }

    public async Task<bool> Retry(CancellationToken cancellation)
    {
      bool result = false;
      while (this._currentState != SemaphoreState.Succeeded && !cancellation.IsCancellationRequested)
      {
        result = await this.AquireLock();
      }

      return result;
    }

    public async Task<bool> Validate()
    {
      int num = 1;
      while (this.Failed)
      {
        if ( await this.AquireLock())
        {
          return true;
        }
        int num1 = num;
        num = num1 + 1;
        if (num1 <= 1000)
        {
          continue;
        }
        return false;
      }
      return true;
    }

    public event EventHandler<EventData<Semaphore>> Released;

    private enum SemaphoreState
    {
      Succeeded,
      Failed,
      Released
    }

    public enum SemaphoreLockMode
    {
      Read,
      Write,
      Exclusive
    }

    public interface ISemaphoreProvider : IDisposable
    {
      string LockStation
      {
        get;
      }

      string LockUser
      {
        get;
      }

      Task ReleaseLock(string name, string atom, SemaphoreLockMode mode);

      Task<bool> SetLock(string name, string atom, SemaphoreLockMode mode);
    }
  }
}