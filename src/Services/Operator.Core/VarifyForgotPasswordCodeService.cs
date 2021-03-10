using System;
using System.Threading.Tasks;
using Valhalla.Services;

namespace TGS.Services.Operator
{
    internal class VarifyForgotPasswordCodeService : IGetService<VarifyForgotPasswordCodeCommand, bool>
    {
        private readonly IValidateCodeServiceClient _validateCodeServiceClient;

        public VarifyForgotPasswordCodeService(IValidateCodeServiceClient validateCodeServiceClient)
        {
            _validateCodeServiceClient = validateCodeServiceClient ?? throw new ArgumentNullException(nameof(validateCodeServiceClient));
        }

        public async ValueTask<bool> GetAsync(VarifyForgotPasswordCodeCommand query)
        {
            var result = await _validateCodeServiceClient
                .GetValidateCodeDataAsync(query.ValidateCode, query.Token)
                .ConfigureAwait(false);

            return !string.IsNullOrEmpty(result);
        }
    }
}
