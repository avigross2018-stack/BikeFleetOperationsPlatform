using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IngestionService.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;
        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetRequest<T>(
            string endpoint,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }
    }
}