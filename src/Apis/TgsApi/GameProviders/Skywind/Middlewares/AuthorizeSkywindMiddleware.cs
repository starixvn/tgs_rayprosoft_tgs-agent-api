using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories.Models;
using TGS.Shared.Game.Models.Skywind.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Middlewares
{
    public sealed class AuthorizeSkywindMiddleware : IMiddleware
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IReadOnlyDictionary<string, Func<Dictionary<string, object>, ValueTask<PlayerLoginRecordModel>>> _methodMapper;
        private readonly ISkywindService _services;

        public AuthorizeSkywindMiddleware(
            IJsonSerializationProvider jsonSerializationProvider,
            ISkywindService services)
        {
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _methodMapper = new Dictionary<string, Func<Dictionary<string, object>, ValueTask<PlayerLoginRecordModel>>>()
            {
                { "/sw/api/validate_ticket",ValidateTicketAsync},
                { "/sw/api/validate_terminal_ticket",ValidateTicketAsync},
                { "/sw/api/get_balance",ValidateBalanceAsync},
                { "/sw/api/debit",ValidateBetAsync},
                { "/sw/api/credit",ValidateSettleAsync},
                { "/sw/api/rollback",ValidateRollbackAsync},
            };
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration = await _services.GetConfigurationAsync().ConfigureAwait(false);
            var gameProviderId = await _services.GetGameProviderIdAsync().ConfigureAwait(false);

            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            var value = await context.Request.ReadAsStringAsync();
            var parameters = _jsonSerializationProvider.DeserializeFromString<Dictionary<string, object>>(_jsonSerializationProvider.SerializeToString(JsonDocument.Parse(value)));

            if (parameters["merch_id"].ToString().IgnoreCaseNotEquals(configuration.MerchId) || parameters["merch_pwd"].ToString().IgnoreCaseNotEquals(configuration.MerchPwd))
            {
                throw new InvalidOperationException(GameApiError.MerchantInternalError.Description());
            }

            if (!_methodMapper.TryGetValue(context.Request.Path.Value.ToLower(), out var execute))
            {
                throw new InvalidOperationException(GameApiError.MerchantInternalError.Description());
            }

            var loginRecord = await execute(parameters).ConfigureAwait(false);
            if (loginRecord == null)
            {
                throw new InvalidOperationException(GameApiError.MerchantInternalError.Description());
            }

            var platformType = loginRecord.PlatformType;
            var brandId = await _services.GetBrandIdByPlayerCodeAsync(loginRecord.PlayerCode);
            var bgpc = await _services.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, loginRecord.PlayerCode);
            context.Items.Add(Consts.ContextItems.PlatformType, platformType);
            await next(context);
        }

        private async ValueTask<PlayerLoginRecordModel> ValidateBalanceAsync(Dictionary<string, object> parameters)
            => await _services.DecryptTokenAsync(parameters["cust_session_id"].ToString()).ConfigureAwait(false);

        private async ValueTask<PlayerLoginRecordModel> ValidateBetAsync(Dictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue("trx_id", out var trxId) || string.IsNullOrEmpty(trxId.ToString()))
            {
                throw new InvalidOperationException(GameApiError.MerchantInternalError.Description());
            }

            return !parameters.TryGetValue("amount", out var amount) || !decimal.TryParse(amount.ToString(), out var decimalAmount) || decimalAmount < 0
                ? throw new InvalidOperationException(GameApiError.MerchantInternalError.Description())
                : await _services.DecryptTokenAsync(parameters["cust_session_id"].ToString()).ConfigureAwait(false);
        }

        private ValueTask<PlayerLoginRecordModel> ValidateRollbackAsync(Dictionary<string, object> parameters)
        {
            return !parameters.TryGetValue("trx_id", out var trxId) || string.IsNullOrEmpty(trxId.ToString())
                ? throw new InvalidOperationException(GameApiError.MerchantInternalError.Description())
                : ValueTask.FromResult(new PlayerLoginRecordModel()
                {
                    PlayerCode = parameters["cust_id"].ToString(),
                    PlatformType = PlatformType.Desktop,
                    Status = LoginStatus.Enabled
                });
        }

        private ValueTask<PlayerLoginRecordModel> ValidateSettleAsync(Dictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue("trx_id", out var trxId) || string.IsNullOrEmpty(trxId.ToString()))
            {
                throw new InvalidOperationException(GameApiError.MerchantInternalError.Description());
            }

            return !parameters.TryGetValue("amount", out var amount) || !decimal.TryParse(amount.ToString(), out var decimalAmount) || decimalAmount < 0
                ? throw new InvalidOperationException(GameApiError.MerchantInternalError.Description())
                : ValueTask.FromResult(new PlayerLoginRecordModel()
                {
                    PlayerCode = parameters["cust_id"].ToString(),
                    PlatformType = PlatformType.Desktop,
                    Status = LoginStatus.Enabled
                });
        }

        private async ValueTask<PlayerLoginRecordModel> ValidateTicketAsync(Dictionary<string, object> parameters)
            => await _services.DecryptTokenAsync(parameters["ticket"].ToString()).ConfigureAwait(false);
    }
}
