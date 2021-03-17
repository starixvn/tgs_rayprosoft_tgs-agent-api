using System;
using System.Threading.Tasks;
using Polly;
using TGS.Services.Operations;
using TGS.Services.Operator.Properties;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.PayPassowrd
{
    class StartChangePayPasswordCommandService : IGetService<StartChangePayPasswordCommand, string>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartChangePayPasswordCommandService(
            IPlayerOperations playerOperations,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }
        public async ValueTask<string> GetAsync(StartChangePayPasswordCommand command)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var saveResult = await Policy.HandleResult<(bool Result, string ValidateCode, string Token)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new ValidateCodeData(command.PlayerCode, token);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync<ValidateCodeData>(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            var shortMessage = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ForgotPasswordMessage), command.CultureInfo)!,
                saveResult.ValidateCode);

            await _validateCodeOperator
                .SendMessageAsync(player.Phone, shortMessage, player.DefaultWalletCurrencyCode, player.BrandId)
                .ConfigureAwait(false);

            return saveResult.Token;
        }
    }
}
