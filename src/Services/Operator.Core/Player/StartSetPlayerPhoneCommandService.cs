using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Services.Operations;
using TGS.Services.Operator.Properties;
using TGS.ServiceUtility;
using TGS.SmsService.Client;

namespace TGS.Services.Operator.Player
{
    internal class StartSetPlayerPhoneCommandService : IGetService<StartSetPlayerPhoneCommand, string>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly ISmsServiceClient _smsServiceClient;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartSetPlayerPhoneCommandService(
            IPlayerOperations playerOperations,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator,
            ISmsServiceClient smsServiceClient)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask<string> GetAsync(StartSetPlayerPhoneCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var saveResult = await Policy.HandleResult<(bool Result, string ValidateCode, string SecurityKey)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new PhoneValidateData(command.Phone, command.PlayerCode);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            var shortMessage = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                saveResult.ValidateCode);

            await _smsServiceClient
                .SendShortMessageAsync(command.Phone, shortMessage, player.DefaultWalletCurrencyCode, player.BrandId)
                .ConfigureAwait(false);

            return saveResult.SecurityKey;
        }
    }
}
