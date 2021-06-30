using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;
using TGS.ServiceUtility;

using CoreModels = TGS.Core.DataServices.Tgs.Models;

using CoreUtility = TGS.Core.Infrastructures.Utility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class GetCreateAgentQueryService : IGetService<GetCreateAgentQuery, CreateAgentInfo>
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _tgsRepository;

        public GetCreateAgentQueryService(
            ITgsRepository tgsRepository,
            IPasswordHandler passwordHandler)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask<CreateAgentInfo> GetAsync(GetCreateAgentQuery query, CancellationToken cancellationToken = default)
        {
            var parentAgent = await _tgsRepository.Agents.AsQueryable()
                .FirstAsync(x => x.Id == query.ParentAgentId)
                .ConfigureAwait(false);
            var brand = await _tgsRepository.Brands.AsQueryable()
                .FirstAsync(x => x.Id == query.BrandId)
                .ConfigureAwait(false);

            var isAgentNameExist = _tgsRepository.Agents.Any(
                x => x.BrandId == query.BrandId
                    && EF.Functions.ILike(x.Username, $"%{query.UserName}%"));

            if (isAgentNameExist)
            {
                return new CreateAgentInfo()
                {
                    IsNew = false,
                    Message = "NameExisted"
                };
            }

            var invitationCode = CoreUtility.InvitationCodeGenerator.NewId();

            while (_tgsRepository.Agents.Any(x => x.InvitationCode == invitationCode))
            {
                invitationCode = CoreUtility.InvitationCodeGenerator.NewId();
            }

            var defaultWalletId = GuidExtensions.NewSequentialGuid();

            var agent = new CoreModels.Agent()
            {
                Username = query.UserName,
                LevelType = AgentLevelType.Agent,
                Type = parentAgent.Type,
                BrandId = parentAgent.BrandId,
                Status = AgentStatus.Enabled,
                ParentAgentId = parentAgent?.Id,
                DefaultWalletId = defaultWalletId,
                AgentStatus =
                    query.LiveDealerRebatePercentage == 0
                        && query.DividendPercentage == 0
                        && query.SlotRebatePercentage == 0
                        && query.LotteryRebatePercentage == 0
                        && query.CardGameRebatePercentage == 0
                        && query.SportsRebatePercentage == 0
                        && query.VirtualSportsRebatePercentage == 0
                            ? AgentConfigStatus.WaitForConfigurePercentage
                            : AgentConfigStatus.DoneForConfigurePercentage,

                DividendPercentage = query.DividendPercentage,
                LiveDealerRebatePercentage = query.LiveDealerRebatePercentage,
                SlotRebatePercentage = query.SlotRebatePercentage,
                LotteryRebatePercentage = query.LotteryRebatePercentage,
                CardGameRebatePercentage = query.CardGameRebatePercentage,
                SportsRebatePercentage = query.SportsRebatePercentage,
                VirtualSportsRebatePercentage = query.VirtualSportsRebatePercentage,
                InvitationCode = invitationCode,
                Remarks = query.Remarks,
                CreatedByUsername = parentAgent!.Username,
                UpdatedByUsername = parentAgent!.Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                RegisteredOn = DateTimeOffset.Now
            };

            var (salt, pwd) = _passwordHandler.GenerateCredential(query.Password);

            agent.Password = pwd;
            agent.Salt = salt;
            agent.WithdrawPassword = Array.Empty<byte>();
            agent.WithdrawSalt = Array.Empty<byte>();
            agent.Wallets = new AgentWallet[]
                {
                    new AgentWallet
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

            await _tgsRepository.Agents.AddAsync(agent);
            await _tgsRepository.SaveChangesAsync();

            return new CreateAgentInfo()
            {
                IsNew = true,
                AgentId = agent.Id.ToString()
            };
        }
    }
}
