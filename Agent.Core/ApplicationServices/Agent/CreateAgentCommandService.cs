using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.Agent.Exceptions;
using AgentType = Tgs.Agent.Client.Models.Enums.AgentType;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class CreateAgentCommandService : IGetService<CreateAgentCommand, CreateAgentInfo>
{
    private readonly ITgsRepository _tgsRepository;
    private readonly IAgentClient _agentClient;

    public CreateAgentCommandService(
        ITgsRepository tgsRepository,
        IAgentClient agentClient)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        _agentClient = agentClient;
    }

    public async ValueTask<CreateAgentInfo> GetAsync(
        CreateAgentCommand query,
        CancellationToken cancellationToken = default)
    {
        var parentAgent = await _tgsRepository.Agents.AsQueryable()
            .FirstAsync(x => x.Id == query.ParentAgentId, cancellationToken: cancellationToken);

        var brand = await _tgsRepository.Brands.AsQueryable()
            .Include(x => x.BrandAgentDividendConditions)
            .Include(x => x.Currencies)
            .FirstAsync(x => x.Id == query.BrandId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var isAgentNameExist = await _tgsRepository.Agents.AsQueryable()
            .AnyAsync(x => x.BrandId == query.BrandId
                && x.Username.ToLower().Equals(query.Username.ToLower()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (isAgentNameExist)
        {
            throw new UsernameException();
        }

        if (parentAgent.DividendPercentage < query.DividendPercentage &&
            brand.AgentDividendType == AgentDividendType.OneLayer)
        {
            var errorMessage = new { parentAgent.DividendPercentage };
            throw new DividendPercentageException(JsonSerializer.Serialize(errorMessage));
        }

        var agentId = await _agentClient.CreateAgentAsync(new CreateAgentRequest
        {
            BrandId = query.BrandId,
            ParentAgentId = parentAgent.ParentAgentId,
            Username = query.Username,
            RealName = query.RealName,
            CurrencyCode = brand.Currencies.First().CurrencyCode,
            Type = (AgentType)parentAgent.Type,
            LevelType = (Tgs.Agent.Client.Models.Enums.AgentLevelType)AgentLevelType.Agent,
            Password = query.Password,
            CardGameRebatePercentage = query.CardGameRebatePercentage,
            CommissionPercentage = query.CommissionPercentage,
            DividendPercentage = query.DividendPercentage,
            LiveDealerRebatePercentage = query.LiveDealerRebatePercentage,
            LotteryRebatePercentage = query.LotteryRebatePercentage,
            SlotRebatePercentage = query.SlotRebatePercentage,
            SportsRebatePercentage = query.SportsRebatePercentage,
            VirtualSportsRebatePercentage = query.VirtualSportsRebatePercentage,
            Remarks = query.Remarks ?? "",
            EditorUsername = parentAgent.Username
        },
            cancellationToken);

        return new CreateAgentInfo
        {
            IsNew = true,
            AgentId = agentId.ToString()
        };
    }
}
