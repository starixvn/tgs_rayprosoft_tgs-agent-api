using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Tgs.CashService.Client;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Logging;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.WalletService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class PlayerReduceAmountHandler : Handler<CreatePlayerReduceAmountRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IPlayerReduceAmountService _playerReduceAmountService;
        private readonly IPlayerService _playerService;
        private readonly ITgsLog _tgsLog;

        public PlayerReduceAmountHandler(
            IAgentService agentService,
            IAgentWalletRepository agentWalletRepository,
            ICashServiceClient cashServiceClient,
            IPlayerService playerService,
            IHttpContextAccessor accessor,
            ITgsLog tgsLog,
            IPlayerReduceAmountService playerReduceAmountService) : base(accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));

            _agentWalletRepository =
                agentWalletRepository ?? throw new ArgumentNullException(nameof(agentWalletRepository));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));

            _playerReduceAmountService = playerReduceAmountService ??
                throw new ArgumentNullException(nameof(playerReduceAmountService));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public override async Task<MessageResponse> Handle(
            CreatePlayerReduceAmountRequest request,
            CancellationToken cancellationToken)
        {
            var player = await _playerService.GetPlayerAsync(x => x.PlayerCode == request.PlayerCode);

            if (player is null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.NotFound);
            }

            var isTransferToAgent = IsTransferToAgent(request);

            var targetAgent = isTransferToAgent
                ? await _agentService
                    .GetAgentAsync(x => x.Id == player.AgentId)
                    .ConfigureAwait(false)
                : null;

            if (isTransferToAgent && targetAgent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.TargetAgentNotFound);
            }

            var wallet = player.Wallets.FirstOrDefault(x => x.Id == player.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            if (wallet.Balance - request.Amount < 0)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.BalanceMustGreaterThanZero);
            }

            var remark = JsonSerializer.Serialize(new
            {
                request.TransactionType,
                TargetAgent = targetAgent?.Username,
                request.Remark
            });

            using var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _playerReduceAmountService.CreateAsync(player.BrandId,
                player.PlayerCode,
                request.TransactionType,
                targetAgent?.Username,
                request.Amount,
                request.Remark,
                request.BucketAmount,
                Username).ConfigureAwait(false);

            if (request.Amount > 0M)
            {
                await AdjustAsync(request.TransactionType,
                    player.DefaultWalletId,
                    -request.Amount,
                    remark,
                    Guid.Parse(UserId));

                if (isTransferToAgent)
                {
                    await _agentWalletRepository.ExchangeAsync(
                        targetAgent.BrandId,
                        targetAgent.DefaultWalletId,
                        targetAgent.Id,
                        TransactionTypes.CompanyAdjust,
                        request.Amount).ConfigureAwait(false);
                }

                _tgsLog.LogInfo($"Reduce amount from {player.PlayerCode} wallet: {request.Amount}");
            }

            tranScope.Complete();

            return CreateValidResponse<MessageResponse>();
        }

        private static bool IsTransferToAgent(CreatePlayerReduceAmountRequest request) =>
            request.TransactionType == PlayerReduceAmountType.TransferAdjust;

        private async ValueTask<TransactionResponse> AdjustAsync(PlayerReduceAmountType type, Guid walletId,
            decimal amount, string remark, Guid userId) => type switch
        {
            PlayerReduceAmountType.AgentAdjust => await _cashServiceClient
                .AgentAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerReduceAmountType.SystemAdjust => await _cashServiceClient
                .SystemAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerReduceAmountType.CompanyAdjust => await _cashServiceClient
                .CompanyAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerReduceAmountType.TransferAdjust => await _cashServiceClient
                .TransferAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false)
        };
    }
}