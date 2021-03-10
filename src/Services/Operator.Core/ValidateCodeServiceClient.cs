using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class ValidateCodeServiceClient : IValidateCodeServiceClient
    {
        private readonly HttpClient _httpClient;

        public ValidateCodeServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask SaveValidateCodeDataAsync(string validateCode, string content)
        {
            var body = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient
                .PostAsync($"/validate-code/{validateCode}", body)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}
