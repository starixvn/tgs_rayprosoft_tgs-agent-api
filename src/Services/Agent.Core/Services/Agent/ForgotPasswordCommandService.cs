using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Agent.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class ForgotPasswordCommandService : IGetService<ForgotPasswordCommand, string>
    {
        private readonly IValidateCodeGenerator _codeGenerator;
        private readonly ITgsRepository _tgsRepository;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ForgotPasswordCommandService(
            ITgsRepository tgsRepository,
            IValidateCodeGenerator codeGenerator,
            IValidateCodeOperator validateCodeOperator)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask<string> GetAsync(ForgotPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var agent = _tgsRepository.Agents.AsQueryable()
                .Where(agent => agent.Email == command.Email)
                .FirstOrDefault();

            if (agent == null)
                throw new AgentNotFoundException(command.Email);

            var saveResult = await Policy
                .HandleResult<(bool Result, string ValidateCode, string Token)>(data => !data.Result)
                .RetryAsync(5)
                .ExecuteAsync(async () =>
                {
                    var validateCode = _codeGenerator.GenerateCode();
                    var token = _codeGenerator.GenerateSecurityKey();

                    var data = new ValidateCodeData(agent.Id.ToString(), token);

                    var result = await _validateCodeOperator
                        .SaveValidateDataAsync(validateCode, token, data)
                        .ConfigureAwait(false);

                    return (result, validateCode, token);
                })
                .ConfigureAwait(false);

            if (!saveResult.Result)
                throw new GenerateValidateCodeFailException();

            await _validateCodeOperator.SendEmailAsync(command.CultureInfo, agent.Email, saveResult.ValidateCode);

            return saveResult.Token;
        }
    }
}
