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
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class PlayerIncreaseAmountHandler : Handler<CreatePlayerIncreaseAmountRequest, MessageResponse>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerIncreaseAmountService _playerIncreaseAmountService;
        private readonly IPlayerService _playerService;
        private readonly ITgsLog _tgsLog;

        public PlayerIncreaseAmountHandler(
            ICashServiceClient cashServiceClient,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository,
            IHttpContextAccessor accessor,
            ITgsLog tgsLog,
            IPlayerIncreaseAmountService playerIncreaseAmountService) : base(accessor)
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
            _playerIncreaseAmountService = playerIncreaseAmountService ??
                throw new ArgumentNullException(nameof(playerIncreaseAmountService));
        }

        public override async Task<MessageResponse> Handle(
            CreatePlayerIncreaseAmountRequest request,
            CancellationToken cancellationToken)
        {
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

            using var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _playerIncreaseAmountService.CreateAsync(player.BrandId,
                player.PlayerCode,
                request.TransactionType,
                request.Amount,
                request.Remark,
                request.BucketAmount,
                Username).ConfigureAwait(false);

            if (request.Amount > 0M)
            {
                await AdjustAsync(request.TransactionType,
                    player.DefaultWalletId,
                    request.Amount,
                    remark,
                    Guid.Parse(UserId)).ConfigureAwait(false);

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

            tranScope.Complete();

            return CreateValidResponse<MessageResponse>();
        }

        private async ValueTask<TransactionResponse> AdjustAsync(PlayerIncreaseAmountType type, Guid walletId,
            decimal amount, string remark, Guid userId) => type switch
        {
            PlayerIncreaseAmountType.AgentAdjust => await _cashServiceClient
                .AgentAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerIncreaseAmountType.SystemAdjust => await _cashServiceClient
                .SystemAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerIncreaseAmountType.CompanyAdjust => await _cashServiceClient
                .CompanyAdjustAsync(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false)
        };
    }
}