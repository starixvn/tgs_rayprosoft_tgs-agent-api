using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class SmsServiceClient : ISmsServiceClient
    {
        private readonly static JsonSerializerOptions _JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly HttpClient _httpClient;

        public SmsServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask SendShortMessageAsync(string phone, string message, string currency, Guid brandId)
        {
            var postData = new
            {
                Destination = phone,
                Message = message,
                Currency = currency,
                BrandId = brandId
            };

            var body = new StringContent(
                JsonSerializer.Serialize(postData, _JsonSerializerOptions),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
            var response = await _httpClient
                .PostAsync("/api/sms", body)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}
