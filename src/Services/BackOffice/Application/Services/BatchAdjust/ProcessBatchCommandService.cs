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

namespace TGS.Services.BackOffice.Application.Services.BatchAdjust;

internal class ProcessBatchCommandService : ICommandService<ProcessBatchCommand>
{
    private readonly IBatchTempRepository _batchTempRepository;
    private readonly ILogger<ProcessBatchCommandService> _logger;
    private readonly IPlayerAdjustBalanceService _playerAdjustBalanceService;

    public ProcessBatchCommandService(
        IBatchTempRepository batchTempRepository,
        IPlayerAdjustBalanceService playerAdjustBalanceService,
        ILogger<ProcessBatchCommandService> logger)
    {
        _batchTempRepository = batchTempRepository ?? throw new ArgumentNullException(nameof(batchTempRepository));
        _playerAdjustBalanceService = playerAdjustBalanceService ?? throw new ArgumentNullException(nameof(playerAdjustBalanceService));
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
                    var applyInfo = new PlayerBalanceAdjustmentApplyInfo(
                        item.BrandId,
                        item.PlayerCode,
                        item.WalletId,
                        item.AdjustAmount >= 0M ? AdjustmentType.Add : AdjustmentType.Subtract,
                        command.AdjustTransferType,
                        AdjustTransferType.None,
                        Math.Abs(item.AdjustAmount),
                        item.Turnover,
                        command.IncludeInNetProfit,
                        item.Remark);

                    await _playerAdjustBalanceService.ProcessAdjustApplyAsync(
                        applyInfo,
                        command.ModifyUserId,
                        command.ModifyUserName,
                        cancellationToken).ConfigureAwait(false);

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
