using Cubic.Core.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cubic.Core.Net
{
  public static class NetworkFunctions
  {
    private static TimeSpan defaultTimeout = TimeSpan.FromMilliseconds(1500);
    private static Uri onlineTestDefaultUri = new Uri("http://g.cn/generate_204");

    /// <summary>Checks the internet connection asynchronous.</summary>
    /// <param name="testUri">The test URI.
    /// Default is google.com</param>
    /// <param name="timeout">The timeout.
    /// Default is 1500ms.</param>
    /// <returns></returns>
    public static async Task<bool> CheckInternetConnectionAsync(Uri testUri = null, TimeSpan timeout = default)
    {
      if(NetworkInterface.GetIsNetworkAvailable())
      {
        var ti = timeout == default ? defaultTimeout : timeout;
        var uri = testUri ?? onlineTestDefaultUri;

        try
        {
          using (var client = new HttpClient())
          {
            client.Timeout = ti;
            using (var response = await client.GetAsync(uri))
            {
              return response.IsSuccessStatusCode;
            }
          }
        }
        catch (Exception)
        {
          return false;
        }
      }

      return false;
    }

    public static async Task<long> Ping(string host, int timeout = 0)
    {

      // Ensure that the timeout is not set to 0 or a negative number.
      if (timeout < 1)
        throw new ArgumentException("Timeout value must be higher than 0.");

      try
      {
        Ping ping = new Ping();
        var reply = await ping.SendPingAsync(host, timeout);

        //if (reply.Status != IPStatus.Success)
        //{
        //  return  reply.RoundtripTime;
        //}
        return reply.RoundtripTime;
      }
      catch (Exception)
      {
        return long.MinValue;
      }
    }

    /// <summary>
    /// Traces the route which data have to travel through in order to reach an IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address of the destination.</param>
    /// <param name="maxHops">Max hops to be returned.</param>
    public static IEnumerable<TracertEntry> Tracert(string ipAddress, int maxHops, int timeout)
    {
      IPAddress address;

      // Ensure that the argument address is valid.
      if (!IPAddress.TryParse(ipAddress, out address))
        throw new ArgumentException(string.Format("{0} is not a valid IP address.", ipAddress));

      // Max hops should be at least one or else there won't be any data to return.
      if (maxHops < 1)
        throw new ArgumentException("Max hops can't be lower than 1.");

      // Ensure that the timeout is not set to 0 or a negative number.
      if (timeout < 1)
        throw new ArgumentException("Timeout value must be higher than 0.");


      Ping ping = new Ping();
      PingOptions pingOptions = new PingOptions(1, true);
      Stopwatch pingReplyTime = new Stopwatch();
      PingReply reply;

      do
      {
        pingReplyTime.Start();
        reply = ping.Send(address, timeout, new byte[] {0}, pingOptions);
        pingReplyTime.Stop();

        string hostname = string.Empty;
        if (reply.Address != null)
        {
          try
          {
            hostname = Dns.GetHostEntry(reply.Address).HostName; // Retrieve the hostname for the replied address.
          }
          catch (SocketException)
          {
            /* No host available for that address. */
          }
        }

        // Return out TracertEntry object with all the information about the hop.
        yield return new TracertEntry()
        {
          HopID = pingOptions.Ttl,
          Address = reply.Address == null ? "N/A" : reply.Address.ToString(),
          Hostname = hostname,
          ReplyTime = pingReplyTime.ElapsedMilliseconds,
          ReplyStatus = reply.Status
        };

        pingOptions.Ttl++;
        pingReplyTime.Reset();
      } while (reply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops);
    }

    public class TracertEntry
    {
      /// <summary>
      /// The hop id. Represents the number of the hop.
      /// </summary>
      public int HopID { get; set; }

      /// <summary>
      /// The IP address.
      /// </summary>
      public string Address { get; set; }

      /// <summary>
      /// The hostname
      /// </summary>
      public string Hostname { get; set; }

      /// <summary>
      /// The reply time it took for the host to receive and reply to the request in milliseconds.
      /// </summary>
      public long ReplyTime { get; set; }

      /// <summary>
      /// The reply status of the request.
      /// </summary>
      public IPStatus ReplyStatus { get; set; }

      public override string ToString()
      {
        return
          $"{HopID} | {(string.IsNullOrEmpty(Hostname) ? Address : $"{Hostname} [{Address}]")} | {(ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : $"{ReplyTime} ms")}";
      }
    }

    public static IPAddress GetHostIpAddressV4()
    {
      var host = Dns.GetHostName();
      IPHostEntry ipEntry = Dns.GetHostEntry(host); // Using this method we can get Host Name,IP address is obtained.  
      return ipEntry.AddressList.FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);
    }

    public static IPAddress GetHostIpAddressV6()
    {
      var host = Dns.GetHostName();
      IPHostEntry ipEntry = Dns.GetHostEntry(host); // Using this method we can get Host Name,IP address is obtained.  
      return ipEntry.AddressList.FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetworkV6);
    }

    public static async Task DownloadFile(Uri sourceFile, string targetFile)
    {
      using (var webClient = new WebClient())
      {
        await webClient.DownloadFileTaskAsync(sourceFile, targetFile);
      }
    }

    public static async Task DownloadFileAsync(Uri sourceFile, string targetFile)
    {
      using (var webClient = new HttpClient())
      {
        using(var target = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
        {
          using (var downloadStraem = await webClient.GetStreamAsync(sourceFile.AbsoluteUri).ConfigureAwait(false))
          {
            await target.CopyToAsync(target).AnyContext();
          }
        }
      }
    }


    /// <summary>
    /// Gets the FQDN for the Local Machine.
    /// </summary>
    /// <returns></returns>
    public static string GetLocalFQDN()
    {
      string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
      string hostName = Dns.GetHostName();

      domainName = "." + domainName;
      if (!hostName.EndsWith(domainName))  // if hostname does not already include domain name
      {
        hostName += domainName;   // add the domain name part
      }

      return hostName;                    // return the fully qualified name
    }

    public static string ResolveHostName(string ipOrHostName)
    {
      return Dns.GetHostEntry(ipOrHostName).HostName;
    }
  }
}