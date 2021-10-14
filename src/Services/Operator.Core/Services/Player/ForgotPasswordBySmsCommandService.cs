using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Services.Operator.Properties;
using TGS.Services.Operator.ValidateCode;
using TGS.ServiceUtility;
using TGS.SmsService.Client;

namespace TGS.Services.Operator.Services.Player
{
    internal class ForgotPasswordBySmsCommandService : IGetService<ForgotPasswordBySmsCommand, string>
    {
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISmsServiceClient _smsServiceClient;
        private readonly ISysRepository _sysRepository;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ForgotPasswordBySmsCommandService(
            IPlayerRepository playerRepository,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator,
            ISysRepository sysRepository,
            ISmsServiceClient smsServiceClient)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
        }

        public async ValueTask<string> GetAsync(ForgotPasswordBySmsCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository
                .FindPlayerInfoByPhoneAsync(command.BrandId, command.AreaCode, command.Phone).ConfigureAwait(false);

            if (player == null)
                throw new PlayerNotFoundException($"The phone ({command.Phone}) can't find the player info.");

            var saveResult = await Policy.HandleResult<(bool Result, string ValidateCode, string Token)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new ValidateCodeData(player.PlayerCode, token);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            var brandInfo = await _sysRepository
                .GetBrandInfoAsync(command.BrandId)
                .ConfigureAwait(false);

            if (brandInfo?.SmsEnabled != true || !player.PhoneAuthentication)
                throw new PlayerPhoneNotSetException();

            await _smsServiceClient.SendShortMessageAsync(
                $"{player.AreaCode}{player.Phone}",
                string.Format(
                    Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                    saveResult.ValidateCode),
                player.BrandId).ConfigureAwait(false);

            return saveResult.Token;
        }
    }
}
