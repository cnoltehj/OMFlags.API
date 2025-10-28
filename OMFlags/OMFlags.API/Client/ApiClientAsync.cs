using OMFlags.Domain.Contracts;
using System.Text.Json;

namespace OMFlags.API.Client
{
    public class ApiClientAsync : IApiClientAsync
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json =
            new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

        public ApiClientAsync(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _http.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<T?> GetAsync<T>(Uri uri)
        {
            using var resp = await _http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead)
                                        .ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            await using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<T>(stream, _json).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            _http.Dispose();
            return ValueTask.CompletedTask;
        }

        public Task<HttpResponseMessage> PostAsync<T>(string path, T body, CancellationToken ct = default) => _http.PostAsJsonAsync(path, body, ct);
    }

}
