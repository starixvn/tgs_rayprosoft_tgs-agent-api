using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Tgs.ServiceUtility;
using TGS.Services.Operator.Properties;
using TGS.Services.Operator.ValidateCode;
using TGS.SmsService.Client;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class StartSetPlayerPhoneCommandService : IGetService<StartSetPlayerPhoneCommand, string>
    {
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISmsServiceClient _smsServiceClient;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartSetPlayerPhoneCommandService(
            IPlayerRepository playerRepository,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator,
            ISmsServiceClient smsServiceClient)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
            _validateCodeOperator =
                validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask<string> GetAsync(
            StartSetPlayerPhoneCommand command,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository
                .GetPlayerInfoAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (player is null)
                throw new PlayerNotFoundException(command.PlayerCode);

            var generateCode = () => _codeGenerator.GenerateCode();

            if (!command.SmsEnabled)
                generateCode = () => "123456";

            var saveResult = await Policy
                .HandleResult<(bool Result, string ValidateCode, string SecurityKey)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = generateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new PhoneValidateData(
                        command.AreaCode,
                        command.Phone,
                        command.PlayerCode);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            if (command.SmsEnabled)
            {
                var shortMessage = string.Format(
                    Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                    saveResult.ValidateCode);

                await _smsServiceClient
                    .SendShortMessageAsync($"{command.AreaCode}{command.Phone}", shortMessage, player.BrandId)
                    .ConfigureAwait(false);
            }

            return saveResult.SecurityKey;
        }
    }
}
