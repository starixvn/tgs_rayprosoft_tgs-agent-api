using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Shared.Game.Models.WGSports.Commons;
using TGS.Shared.Game.Models.WGSports.Outgoing;

namespace TGS.Shared.Game.ApiClients.WGSports
{
    public interface IWGSportsApiClient
    {
        ValueTask<WGSportsPrepareLoginResponse> GetPrepareLoginAsync(WGSportsConfiguration configuration, WGSportsPrepareLoginRequest source);

        ValueTask<WGSportsLoginResponse> GetLoginAsync(WGSportsConfiguration configuration, WGSportsLoginRequest source);

        ValueTask<WGSportsAccessMoneyResponse> AccessMoneyAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyRequest source);

        ValueTask<WGSportsAccessMoneyQueryResponse> AccessMoneyQueryAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyQueryRequest source);

        ValueTask<WGSportsGetBalanceResponse> GetBalanceAsync(WGSportsConfiguration configuration, WGSportsGetBalanceRequest source);

    }
    public class WGSportsApiClient : BaseApiClient, IWGSportsApiClient
    {
        private readonly string _gameProviderCode = Consts.GameProvider.WGSports;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public WGSportsApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json, IHashProvider hashProvider)
            : base(client, logger, json)
        {
            _jsonSerializationProvider = json ?? throw new System.ArgumentNullException(nameof(json));
        }

        ValueTask<WGSportsPrepareLoginResponse> IWGSportsApiClient.GetPrepareLoginAsync(WGSportsConfiguration configuration, WGSportsPrepareLoginRequest source)
        {
            var data = $"{source.Timestamp}|{source.PlayerCode}|{source.PlayerNickname}|{source.CurrencyCode}|{source.Handicap}";
            var dataEncrypt = WGSportsUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }.PrepareLogin";

            return ExecuteJsonRequestAsync<object, WGSportsPrepareLoginResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGSportsLoginResponse> IWGSportsApiClient.GetLoginAsync(WGSportsConfiguration configuration, WGSportsLoginRequest source)
        {
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(source.Token)}&PartnerCode={configuration.PartnerCode}&Lang={source.Lang}";
            var prefix = $"{_gameProviderCode }.Login";

            return ExecuteJsonRequestAsync<object, WGSportsLoginResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGSportsAccessMoneyResponse> IWGSportsApiClient.AccessMoneyAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyRequest source)
        {
            var data = $"{source.PlayerCode}|{(int)source.AmountType}|{source.OrderId}|{source.Amount}";
            var dataEncrypt = WGSportsUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }.AccessMoney";

            return ExecuteJsonRequestAsync<object, WGSportsAccessMoneyResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGSportsAccessMoneyQueryResponse> IWGSportsApiClient.AccessMoneyQueryAsync(WGSportsConfiguration configuration, WGSportsAccessMoneyQueryRequest source)
        {
            var data = source.OrderId;
            var dataEncrypt = WGSportsUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }.AccessMoneyQuery";

            return ExecuteJsonRequestAsync<object, WGSportsAccessMoneyQueryResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGSportsGetBalanceResponse> IWGSportsApiClient.GetBalanceAsync(WGSportsConfiguration configuration, WGSportsGetBalanceRequest source)
        {
            var data = source.PlayerCode;
            var dataEncrypt = WGSportsUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }.GetBalance";

            return ExecuteJsonRequestAsync<object, WGSportsGetBalanceResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }
    }
}
