using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Agent.Properties;
using TGS.Services.ValidateCode.Clients;

namespace TGS.Services.Agent.ValidateCode
{
    internal class RestValidateCodeOperator : IValidateCodeOperator
    {
        private readonly IEmailClient _emailClient;
        private readonly IValidateCodeServiceClient _validateCodeServiceClient;

        public RestValidateCodeOperator(
            IValidateCodeServiceClient validateCodeServiceClient,
            IEmailClient emailClient)
        {
            _validateCodeServiceClient = validateCodeServiceClient ?? throw new ArgumentNullException(nameof(validateCodeServiceClient));
            _emailClient = emailClient ?? throw new ArgumentNullException(nameof(emailClient));
        }

        public async ValueTask<TData?> GetValidateCodeDataAsync<TData>(string validateCode, string token)
        {
            var result = await _validateCodeServiceClient
                .GetValidateCodeDataAsync(validateCode, token)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(result))
                return default;

            return DeserializeValidateCodeData<TData>(result);
        }

        public ValueTask<bool> SaveValidateDataAsync<TData>(string validateCode, string token, TData data)
        {
            return _validateCodeServiceClient
                .SaveValidateCodeDataAsync(validateCode, token, SerializeValidateCodeData(data));
        }

        public async ValueTask SendEmailAsync(CultureInfo cultureInfo, string email, string validateCode)
        {
            var mailSubject = Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailSubject), cultureInfo)!;
            var mailBody = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), cultureInfo)!,
                validateCode);

            var mailMessage = new SendEmailRequest(
                new[] { email },
                mailSubject,
                mailBody);

            await _emailClient.SendAsync(mailMessage).ConfigureAwait(false);
        }

        private TData? DeserializeValidateCodeData<TData>(string json) => JsonSerializer.Deserialize<TData>(json);

        private string SerializeValidateCodeData<TData>(TData data) => JsonSerializer.Serialize(data);
    }
}
