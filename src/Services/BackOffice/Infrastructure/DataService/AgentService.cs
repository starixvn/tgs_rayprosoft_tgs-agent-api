using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IAgentService
    {
        Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null);

        IQueryable<Agent> GetAgentQueries(Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null,
            bool tracking = false);

        IQueryable<AgentAuditLog> GetAgentAuditLogQueries(
            Func<IQueryable<AgentAuditLog>, IQueryable<AgentAuditLog>> preQuery = null,
            bool tracking = false);

        Task CreateAgentAsync(CreateAgentRequest request);
        Task UpdateAgentAsync(Agent agent, UpdateAgentRequest request);
        Task<bool> ExistsAsync(Expression<Func<Agent, bool>> predicate);
    }

    public class AgentService : BaseRepositoryOperations<ITgsRepository>, IAgentService
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _repository;
        private readonly ITransactionManager _transactionManager;

        public AgentService(ITgsRepository repository, IPasswordHandler passwordHandler,
            ITransactionManager transactionManager) : base(repository)
        {
            _passwordHandler = passwordHandler;
            _repository = repository;
            _transactionManager = transactionManager;
        }

        Task IAgentService.CreateAgentAsync(CreateAgentRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            return _transactionManager.ExecuteTransactionAsync(() => _repository,
                IsolationLevel.ReadCommitted, id,
                async _ =>
                {
                    var invitationCode = InvitationCodeGenerator.NewId();

                    while (await ExistsAsync<Agent>(x => x.InvitationCode == invitationCode))
                    {
                        invitationCode = InvitationCodeGenerator.NewId();
                    }

                    var parentAgent =
                        await _repository.Agents.FirstOrDefaultAsync(x => x.Username == request.ParentUsername);

                    var agent = new Agent
                    {
                        Username = request.Username,
                        LevelType = request.LevelType,
                        Type = request.Type,
                        BrandId = request.BrandId,
                        Status = Status.Active,
                        ParentAgentId = parentAgent?.Id,
                        AgentStatus =
                            request.PositionTakingPercentage == 0 && request.BonusPercentage == 0
                                ? AgentStatus.WaitForConfigurePercentage
                                : AgentStatus.DoneForConfigurePercentage,
                        PositionTakingPercentage = request.PositionTakingPercentage,
                        BonusPercentage = request.BonusPercentage,
                        InvitationCode = invitationCode,
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
                            Agent = agent,
                            Balance = 0,
                            CurrencyCode = request.CurrencyCode,
                            CreatedOn = DateTimeOffset.Now,
                            UpdatedOn = DateTimeOffset.Now
                        }
                    }.ToHashSet();

                    return CreateEntityAsync(agent);
                });
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

        Task IAgentService.UpdateAgentAsync(Agent agent, UpdateAgentRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            return _transactionManager.ExecuteTransactionAsync(() => _repository,
                IsolationLevel.ReadCommitted, id,
                async _ =>
                {
                    var parentAgent =
                        await _repository.Agents.FirstOrDefaultAsync(x => x.Username == request.ParentUsername);
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
                            agent.LevelType = request.ParentUsername.IsNotNullOrEmpty()
                                ? AgentLevelType.Chief
                                : AgentLevelType.Agent;
                            agent.Type = request.Type;
                            agent.PositionTakingPercentage = request.PositionTakingPercentage;
                            agent.BonusPercentage = request.BonusPercentage;
                            agent.AgentStatus = request.PositionTakingPercentage == 0 && request.BonusPercentage == 0
                                ? AgentStatus.WaitForConfigurePercentage
                                : AgentStatus.DoneForConfigurePercentage;
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

                    var auditLog = new AgentAuditLog
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        AgentId = agent.Id,
                        CreatedByUsername = request.EditorUsername,
                        CreatedOn = DateTimeOffset.Now,
                        Operation = operation
                    };

                    await UpdateEntityAsync(agent);
                    await CreateEntityAsync(auditLog);

                    return Task.CompletedTask;
                });
        }
    }
}