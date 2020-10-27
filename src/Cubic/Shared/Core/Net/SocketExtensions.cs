using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cubic.Core.Net
{
  public static class SocketExtensions
  {
    public static Task ConnectTaskAsync(this Socket socket, string host, int port)
    {
      return Task.Factory.FromAsync(
                   socket.BeginConnect(host, port, null, null),
                   socket.EndConnect);
    }

    public static Task<int> ReceiveTaskAsync(this Socket socket,
                                            byte[] buffer,
                                            int offset,
                                            int count)
    {
      return Task.Factory.FromAsync<int>(
         socket.BeginReceive(buffer, offset, count, SocketFlags.None, null, socket),
         socket.EndReceive);
    }


    public static Task SendTaskAsync(this Socket socket, byte[] buffer)
    {
      return Task.Factory.FromAsync<int>(
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
            socket.EndSend);
    }

    public static Task SendFileTaskAsync(this Socket socket, string file)
    {
      return Task.Factory.FromAsync(
            socket.BeginSendFile(file, null, socket),
            socket.EndSendFile);
    }
  }
}