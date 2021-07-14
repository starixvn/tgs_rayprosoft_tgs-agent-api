using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Agent.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.PayPassowrd
{
    internal class StartChangePayPasswordCommandService : IGetService<StartChangePayPasswordCommand, string>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public StartChangePayPasswordCommandService(
            IAgentRepository agentRepository,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask<string> GetAsync(StartChangePayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository
                .GetAgentByIdAsync(command.AgentId)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(agent.Email))
                throw new EmailNotVerifyException();

            var saveResult = await Policy
                .HandleResult<(bool Result, string ValidateCode, string Token)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new ValidateCodeData(command.AgentId.ToString(), token);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync<ValidateCodeData>(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            var cultureInfo = command.CultureInfo ?? new CultureInfo("en-us");

            await _validateCodeOperator
                .SendEmailAsync(
                    cultureInfo,
                    agent.Email,
                    saveResult.ValidateCode)
                .ConfigureAwait(false);

            return saveResult.Token;
        }
    }
}
