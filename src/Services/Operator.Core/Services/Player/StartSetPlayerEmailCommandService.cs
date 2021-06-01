using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operator.Properties;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Player;
using TGS.Services.Operator.ValidateCode;

namespace TGS.Services.Operator.Services.Player
{
    internal class StartSetPlayerEmailCommandService : IGetService<StartSetPlayerEmailCommand, string>
    {
        private readonly IEmailManager _emailManager;
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartSetPlayerEmailCommandService(
            IEmailManager emailManager,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator)
        {
            _emailManager = emailManager ?? throw new ArgumentNullException(nameof(emailManager));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask<string> GetAsync(StartSetPlayerEmailCommand command, CancellationToken cancellationToken = default)
        {
            var saveResult = await Policy.HandleResult<(bool Result, string ValidateCode, string SecurityKey)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new EmailValidateData(command.Email, command.PlayerCode);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
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
                new[] { command.Email },
                mailSubject,
                mailBody);

            await _emailManager.SendEmailAsync(mailMessage).ConfigureAwait(false);

            return saveResult.SecurityKey;
        }
    }
}
