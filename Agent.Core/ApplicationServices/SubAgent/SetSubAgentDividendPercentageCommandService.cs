using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

internal class SetSubAgentDividendPercentageCommandService : ICommandService<SetSubAgentDividendPercentageCommand>
{
    private readonly IAgentClient _agentClient;
    private readonly IBrandRepository _brandRepository;

    public SetSubAgentDividendPercentageCommandService(
        IBrandRepository brandRepository,
        IAgentClient agentClient)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
    }

    public async ValueTask ExecuteAsync(SetSubAgentDividendPercentageCommand command, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository
            .GetBrandInfoAsync(command.BrandId)
            .ConfigureAwait(false);

        if (brand == null || brand.AgentDividendType == AgentDividendType.MultipleLayer)
            return;

        var subAgent = await _agentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(command.SubAgentId), cancellationToken)
            .ConfigureAwait(false);

        if (subAgent is null || command.AgentId != subAgent.ParentAgentId)
            return;

        var childInfos = await _agentClient
            .GetChildAgentInfosAsync(new GetChildAgentInfosRequest(command.SubAgentId), cancellationToken)
            .ToArrayAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var childAgentMaxDividendPercentage = childInfos.Any() 
            ? childInfos.Max(x => x.DividendPercentage) 
            : 0M;

        var parentAgent = await _agentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(command.AgentId), cancellationToken)
            .ConfigureAwait(false);

        if (parentAgent is null)
            return;

        if (command.DividendPercentage < childAgentMaxDividendPercentage || command.DividendPercentage > parentAgent.DividendPercentage)
            return;

        await _agentClient
            .SetAgentDividendPercentageAsync(
                new SetAgentDividendPercentageRequest(
                    command.SubAgentId,
                    command.DividendPercentage), cancellationToken)
            .ConfigureAwait(false);

        await _agentClient
            .AddAgentAuditLogAsync(new AddAgentAuditLogRequest(
                    command.SubAgentId,
                    Tgs.Agent.Client.Models.Enums.AgentOperation.Info,
                    parentAgent.Username), cancellationToken)
            .ConfigureAwait(false);
    }
}
