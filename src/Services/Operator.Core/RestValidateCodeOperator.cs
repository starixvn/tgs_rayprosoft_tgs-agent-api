using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class RestValidateCodeOperator : IValidateCodeOperator
    {
        private readonly IValidateCodeServiceClient _validateCodeServiceClient;
        private readonly ISmsServiceClient _smsServiceClient;

        public RestValidateCodeOperator(IValidateCodeServiceClient validateCodeServiceClient, ISmsServiceClient smsServiceClient)
        {
            _validateCodeServiceClient = validateCodeServiceClient ?? throw new ArgumentNullException(nameof(validateCodeServiceClient));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
        }

        public ValueTask<bool> SaveValidateDataAsync(string validateCode, ValidateCodeData data)
        {
            return _validateCodeServiceClient
                .SaveValidateCodeDataAsync(validateCode, data.Token, SerializeValidateCodeData(data));
        }

        public ValueTask SendMessageAsync(string phone, string shortMessage, string currency, Guid brandId)
        {
            return _smsServiceClient.SendShortMessageAsync(phone, shortMessage, currency, brandId);
        }

        public async ValueTask<ValidateCodeData?> GetValidateCodeDataAsync(string validateCode, string token)
        {
            var result = await _validateCodeServiceClient
                .GetValidateCodeDataAsync(validateCode, token)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(result))
                return null;

            return DeserializeValidateCodeData(result);
        }

        private ValidateCodeData? DeserializeValidateCodeData(string json) => JsonSerializer.Deserialize<ValidateCodeData>(json);

        private string SerializeValidateCodeData(ValidateCodeData data) => JsonSerializer.Serialize(data);
    }
}
