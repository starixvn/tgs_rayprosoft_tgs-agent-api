using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Domain;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Services.BackOffice.Application.Services.BatchAdjust;

internal class ProcessBatchCommandService : ICommandService<ProcessBatchCommand>
{
    private readonly IBatchTempRepository _batchTempRepository;
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IPlayerBucketRepository _playerBucketRepository;

    public ProcessBatchCommandService(
        IBatchTempRepository batchTempRepository,
        ICashServiceClient cashServiceClient,
        IPlayerBucketRepository playerBucketRepository)
    {
        _batchTempRepository = batchTempRepository ?? throw new ArgumentNullException(nameof(batchTempRepository));
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
    }

    public async ValueTask ExecuteAsync(ProcessBatchCommand command, CancellationToken cancellationToken = default)
    {
        var batchItems = _batchTempRepository.GetAdjustBalanceBatchAsync(command.BatchId, cancellationToken);

        await foreach (var item in batchItems.ConfigureAwait(false))
        {
            if (item.AdjustAmount != 0M)
                switch (command.AdjustTransferType)
                {
                    case PlayerAdjustmentTransactionType.CompanyAdjust:
                        _ = await _cashServiceClient.AdjustCompanyAsync(
                            item.WalletId,
                            item.AdjustAmount,
                            item.Remark,
                            null,
                            command.ModifyUserId,
                            cancellationToken).ConfigureAwait(false);
                        break;

                    case PlayerAdjustmentTransactionType.AgentAdjust:
                        _ = await _cashServiceClient.AdjustAgentAsync(
                            item.WalletId,
                            item.AdjustAmount,
                            item.Remark,
                            null,
                            command.ModifyUserId,
                            cancellationToken).ConfigureAwait(false);
                        break;

                    case PlayerAdjustmentTransactionType.SystemAdjust:
                        throw new NotSupportedException();

                    case PlayerAdjustmentTransactionType.TransferAdjust:
                        _ = await _cashServiceClient.AdjustTransferAsync(
                            item.WalletId,
                            item.AdjustAmount,
                            item.Remark,
                            null,
                            command.ModifyUserId,
                            cancellationToken).ConfigureAwait(false);
                        break;

                    case PlayerAdjustmentTransactionType.BonusAdjust:
                        _ = await _cashServiceClient.AdjustBonusAsync(
                            item.WalletId,
                            item.AdjustAmount,
                            item.Remark,
                            null,
                            command.ModifyUserId,
                            cancellationToken).ConfigureAwait(false);
                        break;
                }

            if (item.Turnover > 0M)
                await _playerBucketRepository
                    .CreatePlayerBucketsAsync(new[]
                    {
                        new CreatePlayerBucketRequest(
                            command.BrandId,
                            item.PlayerCode,
                            item.WalletId,
                            BucketType.Adjustment,
                            item.Turnover)
                    })
                    .ConfigureAwait(false);
        }
    }
}
