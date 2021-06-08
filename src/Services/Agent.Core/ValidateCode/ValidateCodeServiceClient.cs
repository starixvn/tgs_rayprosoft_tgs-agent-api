using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TGS.Services.Agent.ValidateCode;
using TGS.Services.Agent;

namespace TGS.Services.Agent.ValidateCode
{
    internal class ValidateCodeServiceClient : IValidateCodeServiceClient
    {
        private readonly HttpClient _httpClient;

        public ValidateCodeServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask<string?> GetValidateCodeDataAsync(string validateCode, string token)
        {
            var response = await _httpClient
                .GetAsync($"/validate-code/{validateCode}/{token}")
                .ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async ValueTask<bool> SaveValidateCodeDataAsync(string validateCode, string token, string content)
        {
            var body = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient
                .PostAsync($"/validate-code/{validateCode}/{token}", body)
                .ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.Created;
        }
    }
}
