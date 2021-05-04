using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.AgentAdjust;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Wallet;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class AdjustAgentAmountHandler : Handler<CreateAgentAdjustmentRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentTransactionService _agentTransactionService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly IPlayerService _playerService;
        private readonly Func<ITgsRepository> _repoFactory;
        private readonly ITransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;


        public AdjustAgentAmountHandler(IAgentService agentService, IAgentTransactionService agentTransactionService,
            IAgentWalletRepository agentWalletRepository,
            IWalletRepository walletRepository,
            ITransactionManager transactionManager,
            IPlayerService playerService,
            Func<ITgsRepository> repoFactory,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _agentTransactionService = agentTransactionService;
            _agentWalletRepository = agentWalletRepository;
            _walletRepository = walletRepository;
            _transactionManager = transactionManager;
            _playerService = playerService;
            _repoFactory = repoFactory;
        }

        public override async Task<MessageResponse> Handle(CreateAgentAdjustmentRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _agentService.GetAgentAsync(x => x.Id == request.AgentId);

            if (agent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
            }

            var targetAgent = IsTranslateToAgent(request)
                ? await _agentService.GetAgentAsync(x => x.Username == request.TargetName)
                : null;

            if (targetAgent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
            }

            var targetPlayer = IsTranslateToPlayer(request)
                ? await _playerService.GetPlayerAsync(x => x.Username == request.TargetName)
                : null;

            if (targetPlayer is null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.NotFound);
            }

            var wallet = agent.Wallets.First(x => x.Id == agent.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            if (request.AdjustType == AdjustType.Subtract && wallet.Balance - request.Amount < 0)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.BalanceMustGreaterThanZero);
            }

            await _transactionManager.ExecuteTransactionAsync(_repoFactory,
                IsolationLevel.ReadCommitted,
                Guid.NewGuid().ToString(),
                async repo =>
                {
                    await _agentWalletRepository.ExchangeAsync(agent.BrandId, agent.DefaultWalletId, agent.Id,
                        TransactionTypes.Adjust,
                        request.AdjustType == AdjustType.Subtract ? -request.Amount : request.Amount);

                    if (IsTranslateToAgent(request))
                    {
                        await _agentWalletRepository.ExchangeAsync(targetAgent.BrandId, targetAgent.DefaultWalletId,
                            targetAgent.Id,
                            TransactionTypes.Adjust, request.Amount);
                    }

                    if (IsTranslateToPlayer(request))
                    {
                        await _walletRepository.ExchangeAsync(targetPlayer.BrandId, targetPlayer.DefaultWalletId,
                            targetPlayer.PlayerCode,
                            TransactionTypes.Adjust, request.Amount);
                    }

                    return Task.CompletedTask;
                });

            return CreateValidResponse<MessageResponse>();
        }

        private static bool IsTranslateToAgent(CreateAgentAdjustmentRequest request) =>
            request.AdjustType == AdjustType.Subtract && request.AccountType == AccountType.Agent;

        private static bool IsTranslateToPlayer(CreateAgentAdjustmentRequest request) =>
            request.AdjustType == AdjustType.Subtract && request.AccountType == AccountType.Player;
    }
}