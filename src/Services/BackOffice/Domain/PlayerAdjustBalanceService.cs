using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using TGS.Core.Models.Enums;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Services.BackOffice.Domain;

internal class PlayerAdjustBalanceService : IPlayerAdjustBalanceService
{
    private readonly IBackOfficeRepository _backOfficeRepository;
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IPlayerBucketRepository _playerBucketRepository;

    public PlayerAdjustBalanceService(
        IBackOfficeRepository backOfficeRepository,
        ICashServiceClient cashServiceClient,
        IPlayerBucketRepository playerBucketRepository)
    {
        _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
    }

    public async ValueTask ProcessAdjustApplyAsync(
        PlayerBalanceAdjustmentApplyInfo applyInfo,
        Guid modifyUserId,
        string modifyUser,
        CancellationToken cancellationToken = default)
    {
        var transactionId = applyInfo.Amount > 0M
            ? (await AdjustAsync(
                applyInfo.TransactionType,
                applyInfo.WalletId,
                applyInfo.Amount,
                applyInfo.Remark,
                modifyUserId).ConfigureAwait(false)).TransactionId
            : (Guid?)null;

        if (applyInfo.BucketAmount > 0M)
        {
            await _playerBucketRepository
                .CreatePlayerBucketsAsync(new[]
                {
                    new CreatePlayerBucketRequest(
                        applyInfo.BrandId,
                        applyInfo.PlayerCode,
                        applyInfo.WalletId,
                        BucketType.Adjustment,
                        applyInfo.BucketAmount)
                })
            .ConfigureAwait(false);
        }

        var recordCreateInfo = new AdjustPlayerRecordCreateInfo(
            applyInfo.BrandId,
            applyInfo.PlayerCode,
            applyInfo.AdjustmentType,
            applyInfo.TransactionType,
            applyInfo.TransferType,
            applyInfo.Amount,
            applyInfo.BucketAmount,
            applyInfo.IncludeInNetProfit,
            applyInfo.Remark,
            transactionId,
            applyInfo.TargetAgentUserName);

        await _backOfficeRepository.InsertAdjustPlayerRecordAsync(
            recordCreateInfo,
            modifyUser,
            cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<TransactionResponse> AdjustAsync(
        PlayerAdjustmentTransactionType type,
        Guid walletId,
        decimal amount,
        string remark,
        Guid userId)
        => type switch
        {
            PlayerAdjustmentTransactionType.AgentAdjust => await _cashServiceClient
                .AdjustAgentV2Async(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerAdjustmentTransactionType.SystemAdjust => await _cashServiceClient
                .AdjustSystemV2Async(
                    walletId,
                    amount,
                    0M,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerAdjustmentTransactionType.CompanyAdjust => await _cashServiceClient
                .AdjustCompanyV2Async(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            PlayerAdjustmentTransactionType.BonusAdjust => await _cashServiceClient
                .AdjustBonusV2Async(
                    walletId,
                    amount,
                    remark,
                    null,
                    userId)
                .ConfigureAwait(false),
            _ => throw new NotSupportedException()
        };
}
