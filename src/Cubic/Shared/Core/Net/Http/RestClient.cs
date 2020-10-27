using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Net.Http
{
  /// <summary>
  /// An abstraction over <see cref="HttpClient" /> to address the following issues:
  /// <para>
  /// <see href="http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/" /><br/>
  /// </para>
  /// <para>
  /// <see href="http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html" /><br/>
  /// </para>
  /// <para>
  /// <see href="http://naeem.khedarun.co.uk/blog/2016/11/30/httpclient-dns-settings-for-azure-cloud-services-and-traffic-manager-1480285049222/" />
  /// </para>
  /// </summary>
  /// <seealso cref="IRestClient" />
  public sealed class RestClient : IRestClient
  {

    private readonly HttpClient _client;


    private readonly HashSet<Uri> _endpoints;


    private readonly TimeSpan _connectionCloseTimeoutPeriod;


    /// <summary>
    /// Creates an instance of the <see cref="RestClient" />.
    /// </summary>
    /// <param name="defaultRequestHeaders">The default request headers. If you leave this <c>null</c>, a default "<c>Accept</c>" header with "<c>application/json</c>" setting will be added automatically.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="disposeHandler">If you set this to <c>true</c> any handler will be disposed.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="maxResponseContentBufferSize">Maximum size of the response content buffer. <b>ATTENTION:</b> Make sure you set a sufficient value here to cover the size of your payload!</param>
    /// <param name="userNameForBasicAuthentication">Optional. The user name for basic authentication. Leave <c>null</c> or <see cref="string.Empty" /> if basic authentication is not relevant.</param>
    /// <param name="userPasswordForBasicAuthentication">Optional. The user password for basic authentication. <b>ATTENTION:</b> The password may <b>not</b> contain any colon character (<c>:</c>) !</param>
    /// <exception cref="ArgumentException">2018020150: The provided password cannot be used with this class! It contains a colon character (:) ! - userPasswordForBasicAuthentication</exception>
    public RestClient(IDictionary<string, string> defaultRequestHeaders = null,
                       HttpMessageHandler handler = null,
                       bool disposeHandler = true,
                       TimeSpan? timeout = null,
                       ulong? maxResponseContentBufferSize = null,
                       string userNameForBasicAuthentication = null,
                       string userPasswordForBasicAuthentication = null)
    {
      this._client = handler == null ? new HttpClient() : new HttpClient(handler, disposeHandler);

      if (defaultRequestHeaders == null)
      {
        defaultRequestHeaders = new Dictionary<string, string>
                                {
                                  {"Accept" , "application/json"}
                                };
      }

      this.AddDefaultHeaders(defaultRequestHeaders);

      if (!string.IsNullOrWhiteSpace(userNameForBasicAuthentication))
      {
        if (userPasswordForBasicAuthentication == null) userPasswordForBasicAuthentication = string.Empty;

        if (userPasswordForBasicAuthentication.Contains(":"))
        {
          throw new ArgumentException("2018020150: The provided password cannot be used with this class! It contains a colon character (:) !", "userPasswordForBasicAuthentication");
        }

        this._client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
          "Basic",
          Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userNameForBasicAuthentication, userPasswordForBasicAuthentication))));
      }

      this.AddRequestTimeout(timeout);

      this.AddMaxResponseBufferSize(maxResponseContentBufferSize);

      this._endpoints = new HashSet<Uri>();

      this._connectionCloseTimeoutPeriod = TimeSpan.FromMinutes(1);

      // Default is 2 minutes: https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
      ServicePointManager.DnsRefreshTimeout = TimeSpan.FromMinutes(1).TotalMilliseconds.ToInt32();

      // Increases the concurrent outbound connections
      ServicePointManager.DefaultConnectionLimit = 1024;
    }

    #region "Methods"

    /// <summary>
    /// System AG: Gets the headers which should be sent with each request.
    /// </summary>
    /// <value>The default request headers.</value>
    public IDictionary<string, string> DefaultRequestHeaders => _client.DefaultRequestHeaders.ToDictionary(x => x.Key, x => x.Value.First());

    public Uri BaseAddress
    {
      get { return _client.BaseAddress; }
      set { _client.BaseAddress = value; }
    }

    /// <summary>
    /// System AG: Gets the time to wait before the request times out.
    /// </summary>
    /// <value>The timeout.</value>
    public TimeSpan Timeout => _client.Timeout;

    /// <summary>
    /// System AG: Gets the maximum number of bytes to buffer when reading the response content.
    /// </summary>
    /// <value>The maximum size of the response content buffer.</value>
    public uint MaxResponseContentBufferSize => (uint)_client.MaxResponseContentBufferSize;

    /// <summary>
    /// System AG: Gets all of the endpoints which this instance has sent a request to.
    /// </summary>
    /// <value>The endpoints.</value>
    public Uri[] Endpoints
    {
      get
      {
        lock (_endpoints)
        {
          return _endpoints.ToArray();
        }
      }
    }

    /// <summary>
    /// System AG: Sends the given <paramref name="request" />.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      => SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

    /// <summary>
    /// System AG: Sends the given <paramref name="request" /> with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken)
      => SendAsync(request, HttpCompletionOption.ResponseContentRead, cToken);

    /// <summary>
    /// System AG: Sends the given <paramref name="request" /> with the given <paramref name="option" />.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="option">The option.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option)
      => SendAsync(request, option, CancellationToken.None);

    /// <summary>
    /// System AG: Sends the given <paramref name="request" /> with the given <paramref name="option" /> and <paramref name="cToken" />.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="option">The option.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option, CancellationToken cToken)
    {
      Guard.AgainstNull(request, nameof(request));

      AddConnectionLeaseTimeout(request.RequestUri);

      return _client.SendAsync(request, option, cToken);
    }

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Task<HttpResponseMessage> GetAsync(string uri) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> GetAsync(Uri uri) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" /> with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" /> with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" /> with the given <paramref name="option" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="option">The option.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option)
      => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" /> with the given <paramref name="option" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="option">The option.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option)
      => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" /> with the given <paramref name="option" /> and <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="option">The option.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" /> with the given <paramref name="option" /> and <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="option">The option.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

    /// <summary>
    /// System AG: Sends a <c>PUT</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
      => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

    /// <summary>
    /// System AG: Sends a <c>PUT</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
      => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

    /// <summary>
    /// System AG: Sends a <c>PUT</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />  with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

    /// <summary>
    /// System AG: Sends a <c>PUT</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" /> with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

    /// <summary>
    /// System AG: Sends a <c>POST</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
      => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

    /// <summary>
    /// System AG: Sends a <c>POST</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
      => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

    /// <summary>
    /// System AG: Sends a <c>POST</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />
    /// with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

    /// <summary>
    /// System AG: Sends a <c>POST</c> request with the given <paramref name="content" /> to the specified <paramref name="uri" />
    /// with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="content">The content.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

    /// <summary>
    /// System AG: Sends a <c>DELETE</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<HttpResponseMessage> DeleteAsync(string uri)
      => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

    /// <summary>
    /// System AG: Sends a <c>DELETE</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<HttpResponseMessage> DeleteAsync(Uri uri)
      => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

    /// <summary>
    /// System AG: Sends a <c>DELETE</c> request to the specified <paramref name="uri" /> with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

    /// <summary>
    /// System AG: Sends a <c>DELETE</c> request to the specified <paramref name="uri" /> with the given <paramref name="cToken" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="cToken">The c token.</param>
    /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken)
      => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public Task<string> GetStringAsync(string uri)
    {
      Guard.AgainstNullOrEmpty(uri, nameof(uri));

      return GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<string> GetStringAsync(Uri uri)
    {
      Guard.AgainstNull(uri, nameof(uri));

      AddConnectionLeaseTimeout(uri);

      return _client.GetStringAsync(uri);
    }

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;Stream&gt;.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public Task<Stream> GetStreamAsync(string uri)
    {
      Guard.AgainstNullOrEmpty(uri, nameof(uri));

      return GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;Stream&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<Stream> GetStreamAsync(Uri uri)
    {
      Guard.AgainstNull(uri, nameof(uri));

      AddConnectionLeaseTimeout(uri);

      return _client.GetStreamAsync(uri);
    }

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;System.Byte[]&gt;.</returns>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Task<byte[]> GetByteArrayAsync(string uri)
    {
      Guard.AgainstNullOrEmpty(uri, nameof(uri));

      return GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// System AG: Sends a <c>GET</c> request to the specified <paramref name="uri" />.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>Task&lt;System.Byte[]&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<byte[]> GetByteArrayAsync(Uri uri)
    {
      Guard.AgainstNull(uri, nameof(uri));

      AddConnectionLeaseTimeout(uri);

      return _client.GetByteArrayAsync(uri);
    }

    /// <summary>
    /// System AG: Clears all of the endpoints which this instance has sent a request to.
    /// </summary>
    public void ClearEndpoints()
    {
      lock (_endpoints)
      {
        _endpoints.Clear();
      }
    }

    /// <summary>
    /// System AG: Cancels all pending requests on this instance.
    /// </summary>
    public void CancelPendingRequests() => _client.CancelPendingRequests();

    /// <summary>
    /// System AG: Releases the unmanaged resources and disposes of the managed resources used by the <see cref="HttpClient" />.
    /// </summary>
    public void Dispose()
    {
      _client.Dispose();

      lock (_endpoints)
      {
        _endpoints.Clear();
      }
    }

    /// <summary>
    /// System AG: Adds the default headers.
    /// </summary>
    /// <param name="headers">The headers.</param>
    private void AddDefaultHeaders(IDictionary<string, string> headers)
    {
      if (headers == null)
      {
        return;
      }

      foreach (var item in headers)
      {
        _client.DefaultRequestHeaders.Add(item.Key, item.Value);
      }
    }

    /// <summary>
    /// System AG: Adds the request timeout.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    private void AddRequestTimeout(TimeSpan? timeout)
    {
      if (!timeout.HasValue)
      {
        return;
      }

      _client.Timeout = timeout.Value;
    }

    /// <summary>
    /// System AG: Adds the maximum size of the response buffer.
    /// </summary>
    /// <param name="size">The size.</param>
    private void AddMaxResponseBufferSize(ulong? size)
    {
      if (!size.HasValue)
      {
        return;
      }

      _client.MaxResponseContentBufferSize = (long)size.Value;
    }

    /// <summary>
    /// System AG: Adds the connection lease timeout.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    private void AddConnectionLeaseTimeout(Uri endpoint)
    {
      lock (_endpoints)
      {
        if (_endpoints.Contains(endpoint))
        {
          return;
        }

        ServicePointManager.FindServicePoint(endpoint).ConnectionLeaseTimeout = (int)_connectionCloseTimeoutPeriod.TotalMilliseconds;

        _endpoints.Add(endpoint);
      }
    }

    #endregion
  }

}