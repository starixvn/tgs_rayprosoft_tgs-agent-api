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
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class PlayerIncreaseAmountHandler : Handler<CreatePlayerIncreaseAmountRequest, MessageResponse>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IAdjustPlayerAccountService _playerAdjustAccountService;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerService _playerService;
        private readonly ITgsLog _tgsLog;

        public PlayerIncreaseAmountHandler(
            ICashServiceClient cashServiceClient,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository,
            IHttpContextAccessor accessor,
            ITgsLog tgsLog,
            IAdjustPlayerAccountService playerAdjustAccountService) : base(accessor)
        {
            if (accessor is null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }

            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));

            _playerBucketRepository =
                playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));

            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
            _playerAdjustAccountService = playerAdjustAccountService ??
                throw new ArgumentNullException(nameof(playerAdjustAccountService));
        }

        public override async Task<MessageResponse> Handle(
            CreatePlayerIncreaseAmountRequest request,
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

            var wallet = player.Wallets.FirstOrDefault(x => x.Id == player.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            var remark = JsonSerializer.Serialize(new
            {
                request.TransactionType,
                request.Remark
            });

            var record = await _playerAdjustAccountService.CreateAsync(
                player.BrandId,
                player.PlayerCode,
                AdjustmentType.Add,
                (PlayerAdjustmentTransactionType)request.TransactionType,
                AdjustTransferType.None,
                request.Amount,
                request.Remark,
                request.BucketAmount,
                Username).ConfigureAwait(false);

            if (request.Amount > 0M)
            {
                var response = await AdjustAsync((PlayerAdjustmentTransactionType)request.TransactionType,
                    player.DefaultWalletId,
                    request.Amount,
                    remark,
                    Guid.Parse(UserId)).ConfigureAwait(false);

                record.TransactionId = response.TransactionId;

                await _playerAdjustAccountService.UpdateAdjustPlayerRecordAsync(record).ConfigureAwait(false);

                _tgsLog.LogInfo($"Increase amount to {player.PlayerCode} wallet: {request.Amount}");
            }

            if (request.BucketAmount > 0M)
            {
                await _playerBucketRepository
                    .CreatePlayerBucketsAsync(new[]
                    {
                        new CreatePlayerBucketRequest(
                            player.BrandId,
                            player.PlayerCode,
                            player.DefaultWalletId,
                            BucketType.Adjustment,
                            request.BucketAmount)
                    })
                    .ConfigureAwait(false);

                _tgsLog.LogInfo($"Adjust add bucket {player.PlayerCode}: {request.BucketAmount}");
            }

            return CreateValidResponse<MessageResponse>();
        }

        private async ValueTask<TransactionResponse> AdjustAsync(PlayerAdjustmentTransactionType type, Guid walletId,
            decimal amount, string remark, Guid userId) => type switch
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
                .ConfigureAwait(false)
        };
    }
}