using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.ValidateCode;
using Tgs.ServiceUtility;
using TGS.SmsService.Client;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class ForgotPasswordByPhoneCommandService(
    IAgentRepository agentRepository,
    IValidateCodeGenerator codeGenerator,
    IValidateCodeOperator validateCodeOperator,
    ISmsServiceClient smsServiceClient)
    : IGetService<ForgotPasswordByPhoneCommand, string>
{
    private readonly IAgentRepository _agentRepository =
        agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    private readonly IValidateCodeGenerator _codeGenerator =
        codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));

    private readonly ISmsServiceClient _smsServiceClient =
        smsServiceClient ?? throw new ArgumentNullException(nameof(smsServiceClient));

    private readonly IValidateCodeOperator _validateCodeOperator =
        validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));

    public async ValueTask<string> GetAsync(ForgotPasswordByPhoneCommand command,
        CancellationToken cancellationToken = default)
    {
        var foundAgents = await _agentRepository
            .GetAgentsByPhoneAsync(command.BrandId, command.Phone)
            .ToArrayAsync()
            .ConfigureAwait(false);

        if (foundAgents.Length != 1)
            throw new AgentNotFoundException(command.Phone);

        var agent = foundAgents[0];

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

        await _smsServiceClient
            .SendShortMessageAsync(command.Phone, saveResult.ValidateCode, command.BrandId)
            .ConfigureAwait(false);

        return saveResult.Token;
    }
}