using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.AgentAdjust;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Wallet;
using TGS.Services.WalletService;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class AdjustAgentAmountHandler : Handler<CreateAgentAdjustmentRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPlayerService _playerService;
        private readonly ITgsRepository _repository;
        private readonly SystemTransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;


        public AdjustAgentAmountHandler(IAgentService agentService,
            IAgentWalletRepository agentWalletRepository,
            IWalletRepository walletRepository,
            SystemTransactionManager transactionManager,
            IPlayerService playerService,
            ITgsRepository repository,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _agentWalletRepository = agentWalletRepository;
            _walletRepository = walletRepository;
            _transactionManager = transactionManager;
            _playerService = playerService;
            _repository = repository;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<MessageResponse> Handle(CreateAgentAdjustmentRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _agentService.GetAgentAsync(x => x.Id == request.AgentId);

            if (agent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
            }

            var targetAgent = IsTransferToAgent(request)
                ? await _agentService.GetAgentAsync(x => x.Username == request.TargetName)
                : null;

            if (IsTransferToAgent(request))
            {
                if (targetAgent is null)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.TargetAgentNotFound);
                }

                if (!(targetAgent.ParentAgentId == agent.Id || targetAgent.Id == agent.ParentAgentId))
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.TargetAgentNotHomologue);
                }
            }

            var targetPlayer = IsTransferToPlayer(request)
                ? await _playerService.GetPlayerAsync(x => x.Username == request.TargetName)
                : null;

            if (IsTransferToPlayer(request))
            {
                if (targetPlayer is null)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.TargetPlayerNotFound);
                }

                if (targetPlayer.AgentId != agent.Id)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.TargetPlayerNotUnderAgent);
                }
            }

            var wallet = agent.Wallets.First(x => x.Id == agent.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            if (request.AdjustmentType == AdjustmentType.Subtract && wallet.Balance - request.Amount < 0)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.BalanceMustGreaterThanZero);
            }

            var remark = _jsonSerializationProvider.SerializeToString(new
            {
                request.TransactionType, TargetAgent = targetAgent, TargetPlayer = targetPlayer, request.Remark
            });

            await _agentWalletRepository.ExchangeAsync(agent.BrandId, agent.DefaultWalletId, agent.Id,
                TransactionTypes.Adjust,
                request.AdjustmentType == AdjustmentType.Subtract ? -request.Amount : request.Amount, remark);

            if (IsTransferToAgent(request))
            {
                await _agentWalletRepository.ExchangeAsync(targetAgent.BrandId, targetAgent.DefaultWalletId,
                    targetAgent.Id,
                    TransactionTypes.Adjust, request.Amount);
            }

            if (IsTransferToPlayer(request))
            {
                await _walletRepository.ExchangeAsync(targetPlayer.BrandId, targetPlayer.DefaultWalletId,
                    targetPlayer.PlayerCode,
                    TransactionTypes.Adjust, request.Amount);
            }

            return CreateValidResponse<MessageResponse>();
        }

        private static bool IsTransferToAgent(CreateAgentAdjustmentRequest request) =>
            request.AdjustmentType == AdjustmentType.Subtract &&
            request.TransactionType == AdjustmentTransactionType.Transfer && request.AccountType == AccountType.Agent;

        private static bool IsTransferToPlayer(CreateAgentAdjustmentRequest request) =>
            request.AdjustmentType == AdjustmentType.Subtract &&
            request.TransactionType == AdjustmentTransactionType.Transfer && request.AccountType == AccountType.Player;
    }
}