using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Client.UI.Services
{
    public class TourPlannerApiService : IApiService
    {
        private readonly string _url = "https://localhost:7222/api/";
        private readonly HttpClient _httpClient;

        public TourPlannerApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpStatusCode> DeleteAsync(string path)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, _url + path);
            var response = await _httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

            return response.StatusCode;
        }

        public async Task<(byte[], HttpStatusCode)> GetBytesAsync(string path)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _url + path);
            var response = await _httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsByteArrayAsync(), response.StatusCode);
            }
            catch
            {
                return (Array.Empty<byte>(), response.StatusCode);
            }
        }

        public async Task<(string, HttpStatusCode)> GetStringAsync(string path)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _url + path);
            var response = await _httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
            catch
            {
                return (string.Empty, response.StatusCode);
            }
        }

        /// <summary>
        /// Sends a post request to the TourPlanner REST API
        /// </summary>
        /// <param name="path">Everything that comes after localhost/api/</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<(string, HttpStatusCode)> PostAsync(string path, object content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _url + path);
            using var httpContent = CreateHttpContent(content);
            request.Content = httpContent;

            var response = await _httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
            catch
            {
                return (string.Empty, response.StatusCode);
            }
        }

        public async Task<(string, HttpStatusCode)> PutAsync(string path, object content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, _url + path);
            using var httpContent = CreateHttpContent(content);
            request.Content = httpContent;

            var response = await _httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
            catch
            {
                return (string.Empty, response.StatusCode);
            }
        }

        private static void SerializeJsonIntoStream(object? value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        private static HttpContent? CreateHttpContent(object? content)
        {
            HttpContent? httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

    }
}