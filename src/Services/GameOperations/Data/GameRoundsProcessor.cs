using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    public interface IGameRoundsProcessor
    {
        ValueTask<bool> ExecuteAsync(GameRoundsData data);
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
            try
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
                    WinLoss = data.WinLoss,
                    TieBetAdjustment = data.TieBetAdjustment,
                    ClientStatus = data.ClientStatus
                }).ConfigureAwait(false);

                await _tgsRepository.RoundDetails.AddAsync(new RoundDetail()
                {
                    RoundId = roundsId,
                    AdditionalData = data.AdditionalData
                }).ConfigureAwait(false);

                await _tgsRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
