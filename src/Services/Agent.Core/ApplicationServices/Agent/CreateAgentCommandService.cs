using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.Agent.Exceptions;
using CoreModels = TGS.Core.DataServices.Tgs.Models;
using CoreUtility = TGS.Core.Infrastructures.Utility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class CreateAgentCommandService : IGetService<CreateAgentCommand, CreateAgentInfo>
{
    private readonly IPasswordHandler _passwordHandler;
    private readonly ITgsRepository _tgsRepository;

    public CreateAgentCommandService(
        ITgsRepository tgsRepository,
        IPasswordHandler passwordHandler)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
    }

    public async ValueTask<CreateAgentInfo> GetAsync(
        CreateAgentCommand query,
        CancellationToken cancellationToken = default)
    {
        var parentAgent = await _tgsRepository.Agents.AsQueryable()
            .FirstAsync(x => x.Id == query.ParentAgentId);

        var brand = await _tgsRepository.Brands.AsQueryable()
            .Include(x => x.BrandAgentDividendConditions)
            .FirstAsync(x => x.Id == query.BrandId)
            .ConfigureAwait(false);

        var isAgentNameExist = await _tgsRepository.Agents.AsQueryable()
            .AnyAsync(x => x.BrandId == query.BrandId
                && EF.Functions.ILike(x.Username, $"%{query.Username}%"))
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

        var invitationCode = CoreUtility.InvitationCodeGenerator.NewId();

        while (await _tgsRepository.Agents.AsQueryable()
            .AnyAsync(agent => agent.InvitationCode == invitationCode, cancellationToken)
            .ConfigureAwait(false))
        {
            invitationCode = CoreUtility.InvitationCodeGenerator.NewId();
        }

        var defaultWalletId = GuidExtensions.NewSequentialGuid();

        var agentId = Guid.NewGuid();

        var agent = new CoreModels.Agent
        {
            Id = agentId,
            Username = query.Username,
            LevelType = AgentLevelType.Agent,
            BrandId = parentAgent.BrandId,
            Type = parentAgent.Type,
            Status = AgentStatus.Enabled,
            ParentAgentId = parentAgent?.Id,
            DefaultWalletId = defaultWalletId,
            AgentStatus =
                brand.AgentDividendType == AgentDividendType.MultipleLayer &&
                brand.BrandAgentDividendConditions?.Count > 0 ||
                brand.AgentDividendType == AgentDividendType.OneLayer && query.DividendPercentage > 0
                    ? AgentConfigStatus.DoneForConfigurePercentage
                    : AgentConfigStatus.WaitForConfigurePercentage,
            DividendPercentage = query.DividendPercentage,
            LiveDealerRebatePercentage = query.LiveDealerRebatePercentage,
            SlotRebatePercentage = query.SlotRebatePercentage,
            LotteryRebatePercentage = query.LotteryRebatePercentage,
            CardGameRebatePercentage = query.CardGameRebatePercentage,
            SportsRebatePercentage = query.SportsRebatePercentage,
            VirtualSportsRebatePercentage = query.VirtualSportsRebatePercentage,
            InvitationCode = invitationCode,
            Remarks = query.Remarks,
            Email = string.IsNullOrWhiteSpace(query.Email) ? null : query.Email,
            Phone = query.Phone,
            RealName = query.RealName,
            CreatedByUsername = parentAgent!.Username,
            UpdatedByUsername = parentAgent!.Username,
            CreatedOn = DateTimeOffset.Now,
            UpdatedOn = DateTimeOffset.Now,
            RegisteredOn = DateTimeOffset.Now,
            Path = new LTree($"{parentAgent.Path}.{agentId:N}")
        };

        if (brand.AgentDividendType == AgentDividendType.MultipleLayer)
        {
            agent.AgentDividendConditions = brand.BrandAgentDividendConditions!.Select(
                x => new CoreModels.AgentDividendCondition
                {
                    Rank = x.Rank,
                    NetProfit = x.NetProfit,
                    ActiveNumber = x.ActiveNumber,
                    DividendPercentage = x.DividendPercentage
                }).ToHashSet();
        }

        var (salt, pwd) = _passwordHandler.GenerateCredential(query.Password);

        agent.Password = pwd;
        agent.Salt = salt;
        agent.WithdrawPassword = Array.Empty<byte>();
        agent.WithdrawSalt = Array.Empty<byte>();

        agent.Wallets = new[]
            {
                new CoreModels.AgentWallet
                {
                    Id = defaultWalletId,
                    Agent = agent,
                    Balance = 0,
                    CurrencyCode = brand.Currencies.First().CurrencyCode,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                }
            }
            .ToHashSet();

        await _tgsRepository.Agents.AddAsync(agent, cancellationToken).ConfigureAwait(false);
        await _tgsRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new CreateAgentInfo
        {
            IsNew = true,
            AgentId = agent.Id.ToString()
        };
    }
}
