using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operations;
using TGS.Services.Operator.Properties;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.PayPassowrd
{
    internal class StartChangePayPasswordCommandService : IGetService<StartChangePayPasswordCommand, string>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IValidateCodeOperator _validateCodeOperator;
        private readonly IEmailManager _emailManager;

        public StartChangePayPasswordCommandService(
            IPlayerOperations playerOperations,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator,
            IEmailManager emailManager)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _emailManager = emailManager ?? throw new ArgumentNullException(nameof(emailManager));
        }

        public async ValueTask<string> GetAsync(StartChangePayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(player.Email))
                throw new EmailNotVerifyException();

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

            var mailSubject = Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailSubject), command.CultureInfo)!;
            var mailBody = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                saveResult.ValidateCode);

            var mailMessage = new EmailPackage(
                new[] { player.Email },
                mailSubject,
                mailBody);

            await _emailManager.SendEmailAsync(mailMessage).ConfigureAwait(false);

            return saveResult.Token;
        }
    }
}