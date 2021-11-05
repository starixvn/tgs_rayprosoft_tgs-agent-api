using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TGS.Core.Infrastructures.Json;

namespace TGS.Apis.ReportApi
{
    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public string AccessToken { get; set; }
        public string ChatId { get; set; }

        public TelegramService(
            HttpClient client, IConfiguration configuration, IJsonSerializationProvider jsonSerializationProvider)
        {
            _httpClient = client;
            _jsonSerializationProvider = jsonSerializationProvider;
            configuration.Bind("Telegram", this);
        }

        public async Task SendMessage(string message)
        {
            var model = new
            {
                chat_id = ChatId,
                text = $"<b><i>{message}</i></b>",
                parse_mode = "HTML"
            };

            var todoItemJson = new StringContent(
                _jsonSerializationProvider.SerializeToString(model),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.PostAsync($"bot{AccessToken}/sendMessage", todoItemJson);

            response.EnsureSuccessStatusCode();
        }
    }
}