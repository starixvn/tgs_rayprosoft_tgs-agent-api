using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IAdjustTransferService _adjustTransferService;
        private readonly IAgentService _agentService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IAdjustPlayerAccountService _playerAdjustAccountService;
        private readonly IPlayerService _playerService;
        private readonly ITgsLog _tgsLog;
        private readonly ITransactionService _transactionService;

        public PlayerReduceAmountHandler(
            IAgentService agentService,
            IAgentWalletRepository agentWalletRepository,
            ICashServiceClient cashServiceClient,
            IPlayerService playerService,
            IHttpContextAccessor accessor,
            IAdjustPlayerAccountService playerAdjustAccountService,
            IAdjustTransferService adjustTransferService,
            ITransactionService transactionService,
            ITgsLog tgsLog) : base(accessor)
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

            _playerAdjustAccountService = playerAdjustAccountService ??
                throw new ArgumentNullException(nameof(playerAdjustAccountService));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _adjustTransferService = adjustTransferService ??
                throw new ArgumentNullException(nameof(adjustTransferService));
            _transactionService = transactionService ??
                throw new ArgumentNullException(nameof(transactionService));
        }

        public override async Task<MessageResponse> Handle(
            CreatePlayerReduceAmountRequest request,
            CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(request.TransactionType) ||
                !Enum.IsDefined((PlayerAdjustmentTransactionType)request.TransactionType))
            {
                return CreateInvalidResponse<MessageResponse>("is not define type");
            }

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

            var record = await _playerAdjustAccountService.CreateAsync(player.BrandId,
                player.PlayerCode,
                AdjustmentType.Subtract,
                (PlayerAdjustmentTransactionType)request.TransactionType,
                isTransferToAgent ? AdjustTransferType.PlayerToAgent : AdjustTransferType.None,
                request.Amount,
                request.Remark,
                request.BucketAmount,
                Username,
                targetAgent?.Username).ConfigureAwait(false);

            if (request.Amount > 0M)
            {
                var response = await AdjustAsync((PlayerAdjustmentTransactionType)request.TransactionType,
                    player.DefaultWalletId,
                    -request.Amount,
                    remark,
                    Guid.Parse(UserId));

                var transaction = await _transactionService.GetTransactionAsync(x => x.Id == response.TransactionId);

                record.TransactionId = response.TransactionId;
                record.TransactionRecordedOn = transaction.RecordedOn;

                await _playerAdjustAccountService.UpdateAdjustPlayerRecordAsync(record).ConfigureAwait(false);

                if (isTransferToAgent)
                {
                    var agentResponse = await _agentWalletRepository.ExchangeAsync(
                        targetAgent.BrandId,
                        targetAgent.DefaultWalletId,
                        targetAgent.Id,
                        TransactionTypes.CompanyAdjust,
                        request.Amount).ConfigureAwait(false);

                    await _adjustTransferService.CreateAsync(response.TransactionId,
                        agentResponse.TransactionId,
                        AdjustTransferType.PlayerToAgent,
                        player.Username,
                        targetAgent.Username,
                        request.Amount);
                }

                _tgsLog.LogInfo($"Reduce amount from {player.PlayerCode} wallet: {request.Amount}");
            }

            return CreateValidResponse<MessageResponse>();
        }

        private static bool IsTransferToAgent(CreatePlayerReduceAmountRequest request) =>
            request.TransactionType == PlayerReduceTransactionType.TransferAdjust;

        private async ValueTask<TransactionResponse> AdjustAsync(
            PlayerAdjustmentTransactionType type,
            Guid walletId,
            decimal amount,
            string remark,
            Guid userId)
            => type switch
            {
                PlayerAdjustmentTransactionType.AgentAdjust => await _cashServiceClient
                    .AdjustAgentAsync(
                        walletId,
                        amount,
                        remark,
                        null,
                        userId)
                    .ConfigureAwait(false),
                PlayerAdjustmentTransactionType.SystemAdjust => await _cashServiceClient
                    .AdjustSystemAsync(
                        walletId,
                        amount,
                        remark,
                        null,
                        userId)
                    .ConfigureAwait(false),
                PlayerAdjustmentTransactionType.CompanyAdjust => await _cashServiceClient
                    .AdjustCompanyAsync(
                        walletId,
                        amount,
                        remark,
                        null,
                        userId)
                    .ConfigureAwait(false),
                PlayerAdjustmentTransactionType.TransferAdjust => await _cashServiceClient
                    .AdjustTransferAsync(
                        walletId,
                        amount,
                        remark,
                        null,
                        userId)
                    .ConfigureAwait(false),
                _ => throw new NotSupportedException()
            };
    }
}