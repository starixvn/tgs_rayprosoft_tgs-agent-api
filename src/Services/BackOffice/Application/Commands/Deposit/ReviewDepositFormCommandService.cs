using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using Tgs.ServiceUtility;
using Tgs.Promotion.Clients;
using GetGPBalanceService = Tgs.ServiceUtility.IGetService<TGS.Services.BackOffice.Application.Models.Player.GetPlayerGameProviderBalanceCommand
        , TGS.Services.BackOffice.Application.Models.Player.GetPlayerGameProviderBalanceResponse[]>;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    internal class ReviewDepositFormCommandService : ICommandService<ReviewDepositFormCommand>
    {
        private readonly IBrandService _brandService;

        private readonly GetGPBalanceService _getPlayerGameProviderBalanceService;

        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerService _playerService;
        private readonly IPromotionClient _promotionClient;
        private readonly IDatabase _redisDatabase;

        public ReviewDepositFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository,
            IPromotionClient promotionClient,
            IRedisConnectionFactory redisConnectionFactory,
            IBrandService brandService,
            GetGPBalanceService getPlayerGameProviderBalanceService)
        {
            if (redisConnectionFactory is null)
                throw new ArgumentNullException(nameof(redisConnectionFactory));
            _paymentServiceClient =
                paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));

            _playerBucketRepository =
                playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
            _promotionClient = promotionClient ?? throw new ArgumentNullException(nameof(promotionClient));
            _redisDatabase = redisConnectionFactory.GetConnection().GetDatabase();
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _getPlayerGameProviderBalanceService = getPlayerGameProviderBalanceService ??
                throw new ArgumentNullException(nameof(getPlayerGameProviderBalanceService));
        }

        public async ValueTask ExecuteAsync(
            ReviewDepositFormCommand command,
            CancellationToken cancellationToken = default)
        {
            var isSaveSuccess = false;
            try
            {
                var form = await _paymentServiceClient
                    .GetDepositFormAsync(command.BrandId, command.FormId)
                    .ConfigureAwait(false);

                if (form == null
                    || form.Status == PaymentFlowState.Approved
                    || form.Status == PaymentFlowState.Reject)
                    return;

                isSaveSuccess = await _redisDatabase
                    .StringSetAsync(
                        $"Tgs:BucketDeposit:{command.FormId:N}",
                        command.FormId.ToString(),
                        when: When.NotExists)
                    .ConfigureAwait(false);

                if (!isSaveSuccess)
                    return;

                if (command.ActualAmount != null)
                    await _paymentServiceClient
                        .SubmitDepositFormStatusAsync(
                            form.Id,
                            actualAmount: command.ActualAmount)
                        .ConfigureAwait(false);

                if (command.Status == ReviewState.Approve)
                {
                    var brand = await _brandService.GetBrandAsync(x => x.Id == form.BrandId);

                    if (brand.AutoClearWageringRequirement)
                    {
                        var playerAllBalances = await _getPlayerGameProviderBalanceService.GetAsync(
                            new GetPlayerGameProviderBalanceCommand(form.PlayerCode),
                            cancellationToken).ConfigureAwait(false);

                        var balance = playerAllBalances.Sum(x => x.Balance) ?? 0M;

                        if (balance <= brand.AutoClearWageringRequirementBalance)
                        {
                            await _playerBucketRepository.ClearPlayerBucketsAsync(form.PlayerCode)
                                .ConfigureAwait(false);
                        }
                    }
                }

                await _paymentServiceClient
                    .ReviewDepositFormAsync(
                        command.FormId,
                        command.Status,
                        command.ReviewUserId,
                        command.ReviewUserName,
                        command.Remark)
                    .ConfigureAwait(false);

                if (command.Status == ReviewState.Approve)
                {
                    var player = await _playerService
                        .GetPlayerAsync(x => x.PlayerCode == form.PlayerCode)
                        .ConfigureAwait(false);

                    var promotionResponse = await _playerBucketRepository
                        .CreatePlayerBucketWithDepositAsync(new CreatePlayerBucketWithDepositRequest(
                            command.BrandId,
                            form.PromotionId,
                            player.PlayerCode,
                            player.DefaultWalletId,
                            command.ActualAmount ?? form.Amount,
                            command.Remark,
                            command.FormId,
                            string.IsNullOrEmpty(command.ReviewUserId) ? null : Guid.Parse(command.ReviewUserId)))
                        .ConfigureAwait(false);

                    await _paymentServiceClient
                        .SubmitDepositFormStatusAsync(
                            command.FormId,
                            promotionPlayerId: promotionResponse.PromotionPlayerId)
                        .ConfigureAwait(false);

                    await _playerService
                        .TopUpAccumulatedDepositAsync(form.PlayerCode, command.ActualAmount ?? form.Amount)
                        .ConfigureAwait(false);

                    if (form.PromotionId != null)
                        await _promotionClient.DepositJoinAsync(form.PromotionId.Value, form.PlayerCode)
                            .ConfigureAwait(false);
                }
                else
                {
                    if (form.PromotionId != null)
                        await _promotionClient
                            .RevertPromotionAsync(form.PromotionId.Value, form.Amount)
                            .ConfigureAwait(false);
                }
            }
            finally
            {
                if (isSaveSuccess)
                    await _redisDatabase
                        .KeyDeleteAsync(
                            $"Tgs:BucketDeposit:{command.FormId:N}")
                        .ConfigureAwait(false);
            }
        }
    }
}
