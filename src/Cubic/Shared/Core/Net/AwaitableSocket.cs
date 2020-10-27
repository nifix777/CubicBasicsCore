using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
/// <example>
///     // Reusable SocketAsyncEventArgs and awaitable wrapper
////var args = new SocketAsyncEventArgs();
////args.SetBuffer(new byte[0x1000], 0, 0x1000);
////    var awaitable = new ReceiveSocketAwaitable(args);

////// Do processing, continually receiving from the socket
////int bytesRead;
////    while ((bytesRead = await s.ReceiveAsync(awaitable)) > 0)
////    {
////        Console.WriteLine(bytesRead);
////    } 
/// </example>
namespace Cubic.Core.Net
{

  public sealed class AwaitableSocket : INotifyCompletion, IDisposable
  {
    private static readonly Action _sentinel = () => { };

    private readonly SocketAsyncEventArgs _socketAsyncEventArgs;
    private readonly Socket _socket;

    private Action _continuation;

    public AwaitableSocket(SocketAsyncEventArgs socketAsyncEventArgs, Socket socket)
    {
      _socketAsyncEventArgs = socketAsyncEventArgs;
      _socket = socket;

      socketAsyncEventArgs.Completed
          += (_, __) =>
          {
            var continuation
                      = _continuation
                        ?? Interlocked.CompareExchange(ref _continuation, _sentinel, null);

            continuation?.Invoke();
          };
    }

    public bool IsConnected => _socket.Connected;
    public int BytesTransferred => _socketAsyncEventArgs.BytesTransferred;

    public void SetBuffer(byte[] buffer, int offset, int count)
    {
      _socketAsyncEventArgs.SetBuffer(buffer, offset, count);
    }

    public AwaitableSocket ConnectAsync(CancellationToken cancellationToken)
    {
      Reset();

      if (!_socket.ConnectAsync(_socketAsyncEventArgs))
      {
        IsCompleted = true;
      }

      cancellationToken.Register(Cancel);



      return this;
    }

    void Cancel()
    {
      if (!_socket.Connected)
      {
        _socket.Dispose();
      }
    }

    public AwaitableSocket ReceiveAsync()
    {
      Reset();

      if (!_socket.ReceiveAsync(_socketAsyncEventArgs))
      {
        IsCompleted = true;
      }

      return this;
    }

    public AwaitableSocket SendAsync()
    {
      Reset();

      if (!_socket.SendAsync(_socketAsyncEventArgs))
      {
        IsCompleted = true;
      }

      return this;
    }

    private void Reset()
    {
      IsCompleted = false;
      _continuation = null;
    }

    public AwaitableSocket GetAwaiter()
    {
      return this;
    }

    public bool IsCompleted { get; private set; }

    public void OnCompleted(Action continuation)
    {
      if (_continuation == _sentinel
          || Interlocked.CompareExchange(
              ref _continuation, continuation, null) == _sentinel)
      {
        Task.Run(continuation);
      }
    }

    public void GetResult()
    {
      if (_socketAsyncEventArgs.SocketError != SocketError.Success)
      {
        throw new SocketException((int)_socketAsyncEventArgs.SocketError);
      }
    }

    public void Dispose()
    {
      if (_socket != null)
      {
        if (_socket.Connected)
        {
          _socket.Shutdown(SocketShutdown.Both);
          _socket.Close();
        }

        _socket.Dispose();
      }

      _socketAsyncEventArgs?.Dispose();
    }
  }

}