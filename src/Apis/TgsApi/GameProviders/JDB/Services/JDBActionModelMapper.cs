using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public interface IJDBActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);
    }

    public sealed class JDBActionModelMapper : IJDBActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        PlayerValidationData IJDBActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        #region Private Methods

        private static BetData GetBetData(string playerCode, string currencyCode, GameProviderConfigurationData data,
            TransactionData[] transactions) => new()
        {
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            AutoCloseRound = data.AutoCloseRound,
            PlayerCode = playerCode,
            PlatformType = data.PlatformType,
            HasCurrencyFromGameProvider = true,
            CurrencyCode = currencyCode,
            Transactions = transactions
        };

        

        #endregion
    }
}