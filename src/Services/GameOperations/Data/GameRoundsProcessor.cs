using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    public interface IGameRoundsProcessor
    {
        ValueTask<bool> ExecuteAsync(GameRoundsData data);

        ValueTask<(Round round, RoundDetail roundsDetail)> GetAsync(Guid roundsId);

        ValueTask<(Round roundsData, RoundDetail roundsDetail)> GetExternalRoundIdAsync(string externalRoundId, string playerCode, Guid gameProviderId, int gameId);
    }

    internal sealed partial class GameRoundsProcessor : IGameRoundsProcessor
    {
        private readonly ITgsRepository _tgsRepository;

        public GameRoundsProcessor(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<bool> ExecuteAsync(GameRoundsData data)
        {
            if (data.Success)
            {
                await SuccessProcessorAsync(data).ConfigureAwait(false);

            }
            else
            {
                if (data.Status == RoundStatus.Closed)
                {
                    await FailedProcessorAsync(data).ConfigureAwait(false);
                }
            }

            return true;
        }

        public async ValueTask<(Round round, RoundDetail roundsDetail)> GetAsync(Guid roundsId)
        {
            var round = _tgsRepository.Rounds.Where(x => x.Id == roundsId).FirstOrDefault();
            var roundsDetail = _tgsRepository.RoundDetails.Where(x => x.RoundId == roundsId).FirstOrDefault();
            return new(round, roundsDetail);
        }

        public async ValueTask<(Round roundsData, RoundDetail roundsDetail)> GetExternalRoundIdAsync(string externalRoundId, string playerCode, Guid gameProviderId, int gameId)
        {
            var round = _tgsRepository.Rounds.Where(x =>
            x.ExternalRoundId == externalRoundId
            && x.PlayerCode == playerCode
            && x.GameProviderId == gameProviderId
            && x.GameId == gameId).FirstOrDefault();
            if (round == null)
            {
                return new();
            }
            else
            {
                var roundsDetail = _tgsRepository.RoundDetails.Where(x => x.RoundId == round.Id).FirstOrDefault();

                return (round, roundsDetail);
            }
        }

        #region private
        private async Task SuccessProcessorAsync(GameRoundsData data)
        {
            var round = _tgsRepository.Rounds.Where(x =>
            x.ExternalRoundId == data.ExternalRoundId
            && x.PlayerCode == data.PlayerCode
            && x.GameProviderId == data.GameProviderId
            && x.GameId == data.GameId).FirstOrDefault();
            if (round == null)
            {
                await SuccessInsertAsync(data).ConfigureAwait(false);
            }
            else
            {
                if (round.Status == RoundStatus.Open && data.Status == RoundStatus.Closed)
                {
                    await SuccessUpdateColseAsync(round, data).ConfigureAwait(false);
                }
                else
                {
                    // 改單
                    if (round.Status == RoundStatus.Closed)
                    {
                        if (round.BetAmount != data.BetAmount ||
                        round.ValidBetAmount != data.ValidBetAmount ||
                        round.SettleAmount != data.SettleAmount ||
                         round.WinLoss != (data.PlayerWinLoss ?? 0).Negate()
                        )
                        {
                            await SuccessUpdateReopenedAsync(round, data).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        private async Task SuccessInsertAsync(GameRoundsData data)
        {
            var roundsId = GuidExtensions.NewSequentialGuid();

            _tgsRepository.Rounds.Add(new Round()
            {
                Id = roundsId,
                ExternalRoundId = data.ExternalRoundId,
                GameProviderId = data.GameProviderId,
                PlayerCode = data.PlayerCode,
                GameId = data.GameId,
                CurrencyCode = data.CurrencyCode,
                Status = data.Status,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                WalletId = data.WalletId,
                CreatedOn = data.CreatedOn,
                ClosedOn = data.ClosedOn,
                UpdatedOn = data.UpdatedOn,
                BrandId = data.BrandId,
                ExternalCreatedOn = data.ExternalCreatedOn,
                ExternalClosedOn = data.ExternalClosedOn,
                BetAmount = data.BetAmount,
                ValidBetAmount = data.ValidBetAmount,
                SettleAmount = data.SettleAmount,
                Commission = data.Commission,
                Turnover = data.Turnover,
                WinLoss = (data.PlayerWinLoss ?? 0).Negate(),
                TieBetAdjustment = data.TieBetAdjustment,
                ClientStatus = data.ClientStatus
            });

            _tgsRepository.RoundDetails.Add(new RoundDetail()
            {
                RoundId = roundsId,
                AdditionalData = data.AdditionalData
            });

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

        }

        private async Task SuccessUpdateColseAsync(Round round, GameRoundsData data)
        {
            round.Status = data.Status;
            round.ClosedOn = data.ClosedOn;
            round.UpdatedOn = data.UpdatedOn;
            round.ExternalClosedOn = data.ExternalClosedOn;
            round.BetAmount = data.BetAmount;
            round.ValidBetAmount = data.ValidBetAmount;
            round.SettleAmount = data.SettleAmount;
            round.Commission = data.Commission;
            round.Turnover = data.Turnover;
            round.WinLoss = (data.PlayerWinLoss ?? 0).Negate();
            round.TieBetAdjustment = data.TieBetAdjustment;
            round.ClientStatus = data.ClientStatus;

            _tgsRepository.Rounds.Update(round);

            var roundDetail = new RoundDetail()
            {
                RoundId = round.Id,
                AdditionalData = data.AdditionalData
            };
            _tgsRepository.RoundDetails.Update(roundDetail);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

        }

        private async Task SuccessUpdateReopenedAsync(Round round, GameRoundsData data)
        {
            round.ClosedOn = data.ClosedOn;
            round.UpdatedOn = data.UpdatedOn;
            round.ExternalClosedOn = data.ExternalClosedOn;
            round.BetAmount = data.BetAmount;
            round.ValidBetAmount = data.ValidBetAmount;
            round.SettleAmount = data.SettleAmount;
            round.Commission = data.Commission;
            round.Turnover = data.Turnover;
            round.WinLoss = (data.PlayerWinLoss ?? 0).Negate();
            round.TieBetAdjustment = data.TieBetAdjustment;
            round.ClientStatus = data.ClientStatus;
            round.ReopenedOn = DateTimeOffset.UtcNow;
            _tgsRepository.Rounds.Update(round);

            var roundDetail = new RoundDetail()
            {
                RoundId = round.Id,
                AdditionalData = data.AdditionalData
            };
            _tgsRepository.RoundDetails.Update(roundDetail);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

        }

        private async Task FailedProcessorAsync(GameRoundsData data)
        {
            var failed = _tgsRepository.FailedGameRecordOperations.Where(x => x.ExternalRoundId == data.ExternalRoundId && x.PlayerCode == data.PlayerCode && x.GameProviderId == data.GameProviderId && x.GameId == data.GameId).FirstOrDefault();
            if (failed == null)
            {
                await FailedInsertAsync(data).ConfigureAwait(false);
            }
            else
            {
                if (failed.ProcesStatus == FailedGameRecordOperationProcesStatus.Process)
                {
                    await FailedUpdateAsync(failed, data).ConfigureAwait(false);
                }
            }
        }

        private async Task FailedInsertAsync(GameRoundsData data)
        {
            var item = new FailedGameRecordOperation()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = data.GameProviderId,
                GameProviderCode = data.GameProviderCode,
                ExternalRoundId = data.ExternalRoundId,
                PlayerCode = data.PlayerCode,
                GameId = data.GameId,
                ExternalId = data.ExternalId,
                Detail = data.AdditionalData,
                AutoStatus = FailedGameRecordOperationAutoStatus.Manual,
                ProcesStatus = FailedGameRecordOperationProcesStatus.Process,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow,
            };
            _tgsRepository.FailedGameRecordOperations.Add(item);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task FailedUpdateAsync(FailedGameRecordOperation failed, GameRoundsData data)
        {
            failed.Detail = data.AdditionalData;
            failed.UpdatedOn = DateTimeOffset.UtcNow;
            _tgsRepository.FailedGameRecordOperations.Update(failed);
            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion
    }
}
