using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operator.Properties;
using TGS.SmsService.Client;

namespace TGS.Services.Operator
{
    internal class RestValidateCodeOperator : IValidateCodeOperator
    {
        private readonly IValidateCodeServiceClient _validateCodeServiceClient;
        private readonly ISmsServiceClient _smsServiceClient;
        private readonly IEmailManager _emailManager;

        public RestValidateCodeOperator(
            IValidateCodeServiceClient validateCodeServiceClient,
            ISmsServiceClient smsServiceClient,
            IEmailManager emailManager)
        {
            _validateCodeServiceClient = validateCodeServiceClient ?? throw new ArgumentNullException(nameof(validateCodeServiceClient));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
            _emailManager = emailManager ?? throw new ArgumentNullException(nameof(emailManager));
        }

        public ValueTask<bool> SaveValidateDataAsync<TData>(string validateCode, string token, TData data)
        {
            return _validateCodeServiceClient
                .SaveValidateCodeDataAsync(validateCode, token, SerializeValidateCodeData(data));
        }

        public ValueTask SendMessageAsync(string phone, string shortMessage, string currency, Guid brandId)
        {
            return _smsServiceClient.SendShortMessageAsync(phone, shortMessage, currency, brandId);
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

        private TData? DeserializeValidateCodeData<TData>(string json) => JsonSerializer.Deserialize<TData>(json);

        private string SerializeValidateCodeData<TData>(TData data) => JsonSerializer.Serialize(data);

        public async ValueTask SendEmailAsync(CultureInfo cultureInfo, string email, string validateCode)
        {
            var mailSubject = Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailSubject), cultureInfo)!;
            var mailBody = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), cultureInfo)!,
                validateCode);

            var mailMessage = new EmailPackage(
                new[] { email },
                mailSubject,
                mailBody);

            await _emailManager.SendEmailAsync(mailMessage).ConfigureAwait(false);
        }
    }
}