using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;

namespace TGS.Core.Infrastructures.ApiClient
{
    public abstract class BaseApiClient
    {
        private const char Slash = '/';
        private static readonly char[] UrlTrimChars = {Slash, ' '};
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        private readonly HttpClient _client;

        protected IJsonSerializationProvider Json;
        protected IApiClientLogger Logger;

        protected BaseApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
        {
            Logger = logger;
            Json = json;
            _client = client;
        }

        protected Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            string logPrefix, TimeSpan? timeout = null) =>
            Logger == null ? SendWithTimeoutAsync(request, timeout) : SendAndLogAsync(request, logPrefix, timeout);

        protected async Task<TResponse> ExecuteJsonRequestAsync<TRequest, TResponse>(HttpMethod method, string path,
            TRequest request, IReadOnlyDictionary<string, string> headers = null,
            IReadOnlyDictionary<string, string> contentHeaders = null,
            string logPrefix = null, TimeSpan? timeout = null, JsonSerializerOptions options = null)
        {
            var requestMessage = CreateHttpRequestMessage(path, method);
            var httpRequestMessage = CreateRequest(requestMessage, request, headers, contentHeaders);
            var response = await SendAsync(httpRequestMessage, logPrefix, timeout);
            return await ReadJsonContentAsync<TResponse>(response.Content, options);
        }

        protected async Task<TResponse> ExecuteJsonRequestAsync<TRequest, TResponse>(HttpMethod method, Uri apiUrl,
            string path, TRequest request, IReadOnlyDictionary<string, string> headers = null,
            IReadOnlyDictionary<string, string> contentHeaders = null,
            string logPrefix = null, TimeSpan? timeout = null, JsonSerializerOptions options = null)
        {
            var uriString = apiUrl.AbsoluteUri.Trim(UrlTrimChars);
            if (!string.IsNullOrWhiteSpace(path))
            {
                uriString += $"{Slash}{path.TrimStart(UrlTrimChars)}";
            }

            var requestMessage = CreateHttpRequestMessage(uriString, method);
            var httpRequestMessage = CreateRequest(requestMessage, request, headers, contentHeaders);
            var response = await SendAsync(httpRequestMessage, logPrefix, timeout);
            return await ReadJsonContentAsync<TResponse>(response.Content, options);
        }

        protected HttpRequestMessage CreateRequest(
            HttpRequestMessage httpRequestMessage, object request,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> contentHeaders)
        {
            PrepareJsonRequest(httpRequestMessage, request);
            AddHeaders(httpRequestMessage, headers, contentHeaders);
            return httpRequestMessage;
        }

        #region Private Methods

        private async Task<HttpResponseMessage> SendAndLogAsync(HttpRequestMessage request, string logPrefix,
            TimeSpan? timeout)
        {
            logPrefix ??= GetType().Name;
            // Have to serialize request content first.
            // https://github.com/dotnet/corefx/pull/19082
            var requestContent = request.Content != null ? await request.Content.ReadAsStringAsync() : string.Empty;
            var sw = Stopwatch.StartNew();

            try
            {
                var response = await SendWithTimeoutAsync(request, timeout);
                sw.Stop();
                await Logger.LogHttpResponseAsync(response, (int)sw.ElapsedMilliseconds, requestContent, logPrefix);

                return response;
            }
            catch (TaskCanceledException ex)
            {
                sw.Stop();

                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(
                        $"Connect to remote host timeout {Environment.NewLine}{Environment.NewLine}{ex}"),
                    RequestMessage = request
                };

                await Logger.LogHttpResponseAsync(response, (int)sw.ElapsedMilliseconds, requestContent, logPrefix);
                throw;
            }
            catch (Exception ex)
            {
                sw.Stop();
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content =
                        new StringContent($"Internal TGS exception{Environment.NewLine}{Environment.NewLine}{ex}"),
                    RequestMessage = request
                };

                await Logger.LogHttpResponseAsync(response, (int)sw.ElapsedMilliseconds, requestContent, logPrefix);

                throw;
            }
        }

        private Task<HttpResponseMessage> SendWithTimeoutAsync(HttpRequestMessage request, TimeSpan? timeout)
        {
            timeout ??= DefaultTimeout;
            using var cts = new CancellationTokenSource(timeout.Value);
            return _client.SendAsync(request, cts.Token);
        }

        private void PrepareJsonRequest(HttpRequestMessage request, object content)
        {
            request.Headers.Remove("Accept");
            request.Headers.Add("Accept", "application/json");

            var method = request.Method;
            if (HttpMethod.Post == method || HttpMethod.Put == method || HttpMethod.Delete == method)
            {
                if (content == null)
                {
                    return;
                }

                request.Content = new StringContent(Json.SerializeToString(content));
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.Add("Content-Type", "application/json");
            }
            else if (content != null)
            {
                throw new InvalidOperationException($"Unexpected to have content for method {method}");
            }
        }

        private static void AddHeaders(HttpRequestMessage request, IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> contentHeaders)
        {
            if (headers != null)
            {
                foreach (var (key, value) in headers)
                {
                    if (request.Headers.Contains(key))
                    {
                        request.Headers.Remove(key);
                    }

                    request.Headers.Add(key, value);
                }
            }

            if (contentHeaders == null || request.Content == null)
            {
                return;
            }

            foreach (var (key, value) in contentHeaders)
            {
                if (request.Content.Headers.Contains(key))
                {
                    request.Content.Headers.Remove(key);
                }

                request.Content.Headers.Add(key, value);
            }
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string path, HttpMethod method) =>
            new(method, path);

        private async Task<T> ReadJsonContentAsync<T>(HttpContent content, JsonSerializerOptions options)
        {
            var json = await content.ReadAsStringAsync();
            return Json.DeserializeFromString<T>(json, options);
        }

        #endregion Private Methods
    }
}