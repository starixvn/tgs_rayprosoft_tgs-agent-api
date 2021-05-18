using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IAgentService
    {
        Task CreateAgentAsync(CreateAgentRequest request);

        Task<bool> ExistsAsync(Expression<Func<Agent, bool>> predicate);

        Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> predicate,
                            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null);

        IQueryable<AgentAuditLog> GetAgentAuditLogQueries(
            Func<IQueryable<AgentAuditLog>, IQueryable<AgentAuditLog>> preQuery = null,
            bool tracking = false);

        IQueryable<Agent> GetAgentQueries(Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null,
                    bool tracking = false);

        Task UpdateAgentAsync(Agent agent, UpdateAgentRequest request);
    }

    public class AgentService : BaseRepositoryOperations<ITgsRepository>, IAgentService
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _repository;

        public AgentService(ITgsRepository repository, IPasswordHandler passwordHandler) : base(repository)
        {
            _passwordHandler = passwordHandler;
            _repository = repository;
        }

        async Task IAgentService.CreateAgentAsync(CreateAgentRequest request)
        {
            var invitationCode = InvitationCodeGenerator.NewId();

            while (await ExistsAsync<Agent>(x => x.InvitationCode == invitationCode))
            {
                invitationCode = InvitationCodeGenerator.NewId();
            }

            var parentAgent =
                await _repository.Agents.AsQueryable().FirstOrDefaultAsync(x => x.Username == request.ParentUsername);

            var brand = await _repository.Brands.AsQueryable().FirstAsync(x => x.Id == request.BrandId);
            var defaultWalletId = GuidExtensions.NewSequentialGuid();

            var agent = new Agent
            {
                Username = request.Username,
                LevelType = request.LevelType,
                Type = parentAgent?.Type ?? request.Type,
                BrandId = request.BrandId,
                Status = AgentStatus.Enabled,
                ParentAgentId = parentAgent?.Id,
                DefaultWalletId = defaultWalletId,
                AgentStatus =
                    request.LiveDealerRebatePercentage == 0 && request.BonusPercentage == 0
                    && request.SlotRebatePercentage == 0 && request.LotteryRebatePercentage == 0 &&
                    request.CardGameRebatePercentage == 0
                    && request.SportsRebatePercentage == 0 && request.VirtualSportsRebatePercentage == 0
                        ? AgentConfigStatus.WaitForConfigurePercentage
                        : AgentConfigStatus.DoneForConfigurePercentage,
                CommissionPercentage = request.CommissionPercentage,
                DividendPercentage = request.BonusPercentage,
                LiveDealerRebatePercentage = request.LiveDealerRebatePercentage,
                SlotRebatePercentage = request.SlotRebatePercentage,
                LotteryRebatePercentage = request.LotteryRebatePercentage,
                CardGameRebatePercentage = request.CardGameRebatePercentage,
                SportsRebatePercentage = request.SportsRebatePercentage,
                VirtualSportsRebatePercentage = request.VirtualSportsRebatePercentage,
                InvitationCode = invitationCode,
                Remarks = request.Remarks,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                RegisteredOn = DateTimeOffset.Now
            };

            var (salt, pwd) = _passwordHandler.GenerateCredential(request.Password);
            agent.Password = pwd;
            agent.Salt = salt;
            agent.WithdrawPassword = Array.Empty<byte>();
            agent.WithdrawSalt = Array.Empty<byte>();
            agent.Wallets = new[]
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
            }.ToHashSet();

            await CreateEntityAsync(agent);
        }

        Task<bool> IAgentService.ExistsAsync(Expression<Func<Agent, bool>> predicate) => ExistsAsync(predicate);

        Task<Agent> IAgentService.GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery) => GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<AgentAuditLog> IAgentService.GetAgentAuditLogQueries(
            Func<IQueryable<AgentAuditLog>, IQueryable<AgentAuditLog>> preQuery, bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        IQueryable<Agent> IAgentService.GetAgentQueries(Func<IQueryable<Agent>, IQueryable<Agent>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IAgentService.UpdateAgentAsync(Agent agent, UpdateAgentRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var parentAgent =
                await _repository.Agents.AsQueryable().FirstOrDefaultAsync(x =>
                    x.BrandId == request.BrandId && x.Username == request.ParentUsername);
            var operation = (AgentOperation)Enum.Parse(typeof(AgentOperation), request.UpdateType.ToString());

            if (request.ParentUsername.IsNotNullOrEmpty() && agent.ParentAgentId != parentAgent.Id)
            {
                operation = AgentOperation.Agent;
            }

            switch (request.UpdateType)
            {
                case MemberUpdateType.Info:
                    agent.RealName = request.RealName;
                    agent.ParentAgentId = parentAgent?.Id;
                    agent.Status = request.Status;
                    agent.LevelType = request.ParentUsername.IsNullOrEmpty()
                        ? AgentLevelType.Chief
                        : AgentLevelType.Agent;
                    agent.Type = request.Type;
                    agent.CommissionPercentage =
                        operation == AgentOperation.Agent ? -1 : request.CommissionPercentage;
                    agent.DividendPercentage = operation == AgentOperation.Agent ? -1 : request.BonusPercentage;
                    agent.LiveDealerRebatePercentage = operation == AgentOperation.Agent
                        ? -1
                        : request.LiveDealerRebatePercentage;
                    agent.SlotRebatePercentage =
                        operation == AgentOperation.Agent ? -1 : request.SlotRebatePercentage;
                    agent.LotteryRebatePercentage = operation == AgentOperation.Agent
                        ? -1
                        : request.LotteryRebatePercentage;
                    agent.CardGameRebatePercentage = operation == AgentOperation.Agent
                        ? -1
                        : request.CardGameRebatePercentage;
                    agent.SportsRebatePercentage =
                        operation == AgentOperation.Agent ? -1 : request.SportsRebatePercentage;
                    agent.VirtualSportsRebatePercentage = operation == AgentOperation.Agent
                        ? -1
                        : request.VirtualSportsRebatePercentage;
                    agent.AgentStatus = operation == AgentOperation.Agent
                            ? AgentConfigStatus.WaitForConfigurePercentage
                            : AgentConfigStatus.DoneForConfigurePercentage;
                    break;

                case MemberUpdateType.Contact:
                    agent.Phone = request.Phone;
                    agent.Email = request.Email;
                    agent.QQ = request.QQ;
                    agent.Wechat = request.Wechat;
                    break;

                case MemberUpdateType.Password:
                    var (salt, pwd) = _passwordHandler.GenerateCredential(request.Password);
                    agent.Password = pwd;
                    agent.Salt = salt;
                    break;
            }

            agent.UpdatedByUsername = request.EditorUsername;
            agent.UpdatedOn = DateTimeOffset.Now;
            agent.Remarks = request.Remarks;

            var auditLog = new AgentAuditLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                AgentId = agent.Id,
                CreatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                Operation = operation
            };

            await UpdateEntityAsync(agent).ConfigureAwait(false);
            await CreateEntityAsync(auditLog).ConfigureAwait(false);

            scope.Complete();
        }
    }
}
