using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Wallet;
using TGS.Services.WalletService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class AdjustPlayerAmountHandler : Handler<CreatePlayerAdjustmentRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPlayerService _playerService;
        private readonly Func<ITgsRepository> _repoFactory;
        private readonly ITransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;

        public AdjustPlayerAmountHandler(IAgentService agentService,
            IAgentWalletRepository agentWalletRepository,
            IWalletRepository walletRepository,
            ITransactionManager transactionManager,
            IPlayerService playerService,
            Func<ITgsRepository> repoFactory,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _agentWalletRepository = agentWalletRepository;
            _walletRepository = walletRepository;
            _transactionManager = transactionManager;
            _playerService = playerService;
            _repoFactory = repoFactory;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<MessageResponse> Handle(CreatePlayerAdjustmentRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode);

            if (player is null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.NotFound);
            }

            var targetAgent = IsTransferToAgent(request)
                ? await _agentService.GetAgentAsync(x => x.Id == player.AgentId)
                : null;

            if (IsTransferToAgent(request) && targetAgent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.TargetAgentNotFound);
            }

            var wallet = player.Wallets.First(x => x.Id == player.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            if (request.AdjustType == AdjustmentType.Subtract && wallet.Balance - request.Amount < 0)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.BalanceMustGreaterThanZero);
            }

            await _transactionManager.ExecuteTransactionAsync(_repoFactory,
                IsolationLevel.ReadCommitted,
                Guid.NewGuid().ToString(),
                async repo =>
                {
                    var remark = _jsonSerializationProvider.SerializeToString(new
                    {
                        request.TransactionType, TargetAgent = targetAgent?.Username, request.Remark
                    });

                    await _walletRepository.ExchangeAsync(player.BrandId, player.DefaultWalletId, player.PlayerCode,
                        TransactionTypes.Adjust,
                        request.AdjustType == AdjustmentType.Subtract ? -request.Amount : request.Amount,
                        remark);

                    if (IsTransferToAgent(request))
                    {
                        await _agentWalletRepository.ExchangeAsync(targetAgent.BrandId, targetAgent.DefaultWalletId,
                            targetAgent.Id,
                            TransactionTypes.Adjust, request.Amount);
                    }

                    return Task.CompletedTask;
                });

            return CreateValidResponse<MessageResponse>();
        }

        private static bool IsTransferToAgent(CreatePlayerAdjustmentRequest request) =>
            request.AdjustType == AdjustmentType.Subtract &&
            request.TransactionType == AdjustmentTransactionType.Transfer;
    }
}