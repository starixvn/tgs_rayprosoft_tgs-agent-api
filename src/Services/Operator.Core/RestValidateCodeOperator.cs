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
                .SaveValidateCodeDataAsync(validateCode, data.Token, JsonSerializer.Serialize(data));
        }

        public ValueTask SendMessageAsync(string phone, string shortMessage, string currency, Guid brandId)
        {
            return _smsServiceClient.SendShortMessageAsync(phone, shortMessage, currency, brandId);
        }
    }
}
