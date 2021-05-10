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
            var round = _tgsRepository.Rounds.Where(x =>
            x.ExternalRoundId == data.ExternalRoundId
            && x.PlayerCode == data.PlayerCode
            && x.GameProviderId == data.GameProviderId
            && x.GameId == data.GameId).FirstOrDefault();
            if (round == null)
            {
                var roundsId = GuidExtensions.NewSequentialGuid();

                await _tgsRepository.Rounds.AddAsync(new Round()
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
                    WinLoss = (data.WinLoss ?? 0).Negate(),
                    TieBetAdjustment = data.TieBetAdjustment,
                    ClientStatus = data.ClientStatus
                }).ConfigureAwait(false);

                if (data.Status == RoundStatus.Closed)
                {
                    await _tgsRepository.RoundDetails.AddAsync(new RoundDetail()
                    {
                        RoundId = roundsId,
                        AdditionalData = data.AdditionalData
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                if (round.Status == RoundStatus.Open && data.Status == RoundStatus.Closed)
                {
                    round.ClosedOn = data.ClosedOn;
                    round.UpdatedOn = data.UpdatedOn;
                    round.ExternalClosedOn = data.ExternalClosedOn;
                    round.BetAmount = data.BetAmount;
                    round.ValidBetAmount = data.ValidBetAmount;
                    round.SettleAmount = data.SettleAmount;
                    round.Commission = data.Commission;
                    round.Turnover = data.Turnover;
                    round.WinLoss = (data.WinLoss ?? 0).Negate();
                    round.TieBetAdjustment = data.TieBetAdjustment;
                    round.ClientStatus = data.ClientStatus;

                    _tgsRepository.Rounds.Update(round);

                    await _tgsRepository.RoundDetails.AddAsync(new RoundDetail()
                    {
                        RoundId = round.Id,
                        AdditionalData = data.AdditionalData
                    }).ConfigureAwait(false);
                }
                else
                {
                    // 改單
                    if (round.Status == RoundStatus.Closed)
                    {
                        if (round.BetAmount != data.BetAmount ||
                        round.ValidBetAmount != data.ValidBetAmount ||
                        round.SettleAmount != data.SettleAmount ||
                         round.WinLoss != data.WinLoss
                        )
                        {
                            round.ClosedOn = data.ClosedOn;
                            round.UpdatedOn = data.UpdatedOn;
                            round.ExternalClosedOn = data.ExternalClosedOn;
                            round.BetAmount = data.BetAmount;
                            round.ValidBetAmount = data.ValidBetAmount;
                            round.SettleAmount = data.SettleAmount;
                            round.Commission = data.Commission;
                            round.Turnover = data.Turnover;
                            round.WinLoss = (data.WinLoss ?? 0).Negate();
                            round.TieBetAdjustment = data.TieBetAdjustment;
                            round.ClientStatus = data.ClientStatus;

                            _tgsRepository.Rounds.Update(round);

                            var roundDetail = new RoundDetail()
                            {
                                RoundId = round.Id,
                                AdditionalData = data.AdditionalData
                            };
                        }
                    }
                }
            }
            await _tgsRepository.SaveChangesAsync();
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
    }
}
