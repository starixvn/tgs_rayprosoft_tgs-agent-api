using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.LY;
using TGS.Shared.Game.Models.LY.Enums;
using TGS.Shared.Game.Models.LY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.LY.Services
{
    public class LYGameProviderWalletAction : BaseGameProviderWalletAction
    {
        private readonly ILYApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILYService _service;
        private readonly ISettingOperations _settingOperations;

        public LYGameProviderWalletAction(
            ILYApiClient client,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            ICashServiceClient cashServiceClient,
            ILYService service)
            : base(
                playerOperations,
                gameProviderOperations,
                gameProviderFundTransferTransactionRepository,
                cashServiceClient)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public override async ValueTask<bool> CheckingAmountAsync(CachedPlayer player, decimal amount)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<LYConfiguration>(SettingConsts
                    .GameProviderConfigurations.LY);

            if (configuration.AmountMaximum < amount)
            {
                return false;
            }

            if (amount.Truncate(Convert.ToInt32(configuration.AmountFormat.Split(',')[1])) < amount)
            {
                return false;
            }

            return true;
        }

        public override async ValueTask<(bool Success, string OrderId, decimal? Balance)> FundInAsync(
            Guid transactionGameProviderId, CachedPlayer player, decimal amount)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return (false, null, null);
            }

            var agentId = brandMerchant.AgentMappers
                .Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status)
                .FirstOrDefault().AgentId;

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();

            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{player.PlayerCode}";

            var data = new LYFnudRequest
            {
                PlayerCode = player.PlayerCode,
                Amount = amount,
                OrderId = orderId
            };

            var response = await _client.FundAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                dateTimeOffset,
                LYAction.FundIn,
                data)
                .ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return (false, orderId, null);
            }

            decimal.TryParse(response.Data.Balance, out var balance);

            return new(true, orderId, balance);
        }

        public override async ValueTask<(bool Success, string OrderId, decimal? Balance)> FundOutAsync(
            Guid transactionGameProviderId, CachedPlayer player, decimal amount)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return (false, null, null);
            }

            var agentId = brandMerchant.AgentMappers
                .Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status)
                .FirstOrDefault().AgentId;

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();

            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{player.PlayerCode}";

            var data = new LYFnudRequest
            {
                PlayerCode = player.PlayerCode,
                Amount = amount,
                OrderId = orderId
            };

            var response = await _client.FundAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                dateTimeOffset,
                LYAction.FundOut,
                data)
                .ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return (false, orderId, null);
            }

            decimal.TryParse(response.Data.Balance, out var balance);

            return new(true, orderId, balance);
        }

        public override async ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync()
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.LY)
                .ConfigureAwait(false);

            return new(gameProviderId, Consts.GameProvider.LY);
        }

        public override async ValueTask<(bool Success, decimal Balance)> GetGameProviderBalanceAsync(
            CachedPlayer player)
        {
            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                return (false, 0);
            }

            var agentId = brandMerchant.AgentMappers
                .Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status)
                .FirstOrDefault().AgentId;

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();

            var data = new LYGetBalanceRequest
            {
                PlayerCode = player.PlayerCode
            };

            var response = await _client.GetBalanceAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Deskey,
                brandMerchant.Md5key,
                agentId,
                data).ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return new(false, 0);
            }

            return new(true, response.Data.Balance);
        }
    }
}
