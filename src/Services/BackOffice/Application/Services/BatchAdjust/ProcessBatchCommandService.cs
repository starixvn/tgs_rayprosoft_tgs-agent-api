using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ProcessBatchCommandService> _logger;
    private readonly IPlayerBucketRepository _playerBucketRepository;

    public ProcessBatchCommandService(
        IBatchTempRepository batchTempRepository,
        ICashServiceClient cashServiceClient,
        IPlayerBucketRepository playerBucketRepository,
        ILogger<ProcessBatchCommandService> logger)
    {
        _batchTempRepository = batchTempRepository ?? throw new ArgumentNullException(nameof(batchTempRepository));
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask ExecuteAsync(ProcessBatchCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _batchTempRepository
            .BidBatchAsync(command.BatchId, cancellationToken)
            .ConfigureAwait(false))
            return;

        var batch = await _batchTempRepository.GetAdjustBalanceBatchAsync(command.BatchId, cancellationToken)
            .Select(async item =>
            {
                try
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

                    return new BatchAdjustBalanceItemResult(item.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Process batch item fail.");

                    return new BatchAdjustBalanceItemResult(item.Id, false, ex);
                }
            })
            .SelectMany(t => t.ToAsyncEnumerable())
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        await _batchTempRepository.SaveAdjustBalanceBatchResultAsync(
            command.BatchId,
            batch,
            cancellationToken).ConfigureAwait(false);

        await _batchTempRepository
            .CompleteBatchAsync(command.BatchId, cancellationToken)
            .ConfigureAwait(false);
    }
}
