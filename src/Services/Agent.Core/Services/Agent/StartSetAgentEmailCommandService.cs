using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.Properties;
using TGS.Services.Agent.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class StartSetAgentEmailCommandService : IGetService<StartSetAgentEmailCommand, string>
    {
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IEmailClient _emailClient;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartSetAgentEmailCommandService(
            IEmailClient emailClient,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator)
        {
            _emailClient = emailClient ?? throw new ArgumentNullException(nameof(emailClient));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator =
                validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask<string> GetAsync(StartSetAgentEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            var saveResult = await Policy
                .HandleResult<(bool Result, string ValidateCode, string SecurityKey)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new EmailValidateData(command.Email, command.AgentId);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            var mailSubject =
                Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailSubject), command.CultureInfo)!;
            var mailBody = string.Format(
                Resources.ResourceManager.GetString(nameof(Resources.ValidateCodeMailBody), command.CultureInfo)!,
                saveResult.ValidateCode);

            var mailMessage = new SendEmailRequest(
                new[]
                {
                    command.Email
                },
                mailSubject,
                mailBody);

            await _emailClient.SendAsync(mailMessage).ConfigureAwait(false);

            return saveResult.SecurityKey;
        }
    }
}