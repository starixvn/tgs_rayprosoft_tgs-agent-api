using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operations;
using TGS.Services.Operator.Properties;
using TGS.Services.Operator.ValidateCode;
using TGS.ServiceUtility;
using TGS.SmsService.Client;

namespace TGS.Services.Operator.Services.PayPassowrd
{
    internal class StartChangePayPasswordCommandService : IGetService<StartChangePayPasswordCommand, string>
    {
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IEmailClient _emailClient;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISmsServiceClient _smsServiceClient;
        private readonly ISysRepository _sysRepository;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartChangePayPasswordCommandService(
            IPlayerOperations playerOperations,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator,
            IEmailClient emailClient,
            ISmsServiceClient smsServiceClient,
            ISysRepository sysRepository)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _emailClient = emailClient ?? throw new ArgumentNullException(nameof(emailClient));
            _smsServiceClient = smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
        }

        public async ValueTask<string> GetAsync(
            StartChangePayPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(player.Email))
                throw new EmailNotVerifyException();

            var saveResult = await Policy.HandleResult<(bool Result, string ValidateCode, string Token)>(
                data => !data.Result)
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

            var brandInfo = await _sysRepository
                .GetBrandInfoAsync(player.BrandId)
                .ConfigureAwait(false);

            if (brandInfo!.SmsEnabled)
            {
                if (!player.PhoneAuthentication)
                    throw new PlayerPhoneNotSetException();

                await _smsServiceClient.SendShortMessageAsync(
                    $"{player.AreaCode}{player.Phone}",
                    string.Format(
                        Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                        saveResult.ValidateCode),
                    player.BrandId).ConfigureAwait(false);
            }
            else
            {
                var mailSubject = Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailSubject), command.CultureInfo)!;
                var mailBody = string.Format(
                    Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                    saveResult.ValidateCode);

                var mailMessage = new SendEmailRequest(
                    new[] { player.Email },
                    mailSubject,
                    mailBody);

                await _emailClient.SendAsync(mailMessage).ConfigureAwait(false);
            }

            return saveResult.Token;
        }
    }
}
