using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Shared.Game.Models.WGHorse.Commons;
using TGS.Shared.Game.Models.WGHorse.Outgoing;

namespace TGS.Shared.Game.ApiClients.WGHorse
{
    public interface IWGHorseApiClient
    {
        ValueTask<WGHorsePrepareLoginResponse> GetPrepareLoginAsync(WGHorseConfiguration configuration, WGHorsePrepareLoginRequest source);

        ValueTask<WGHorseLoginResponse> GetLoginAsync(WGHorseConfiguration configuration, WGHorseLoginRequest source);

        ValueTask<WGHorseResponse> AccessMoneyAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyRequest source);

        ValueTask<WGHorseResponse> AccessMoneyQueryAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyQueryRequest source);

        ValueTask<WGHorseGetBalanceResponse> GetBalanceAsync(WGHorseConfiguration configuration, WGHorseGetBalanceRequest source);

    }
    public class WGHorseApiClient : BaseApiClient, IWGHorseApiClient
    {
        private readonly string _gameProviderCode = Consts.GameProvider.WGHorse;

        public WGHorseApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json, IHashProvider hashProvider)
            : base(client, logger, json)
        {
        }

        ValueTask<WGHorsePrepareLoginResponse> IWGHorseApiClient.GetPrepareLoginAsync(WGHorseConfiguration configuration, WGHorsePrepareLoginRequest source)
        {
            var data = $"{source.Timestamp}|{source.PlayerCode}|{source.PlayerNickname}|{source.CurrencyCode}";
            var dataEncrypt = WGHorseUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=PrepareLogin&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }_PrepareLogin";

            return ExecuteJsonRequestAsync<object, WGHorsePrepareLoginResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGHorseLoginResponse> IWGHorseApiClient.GetLoginAsync(WGHorseConfiguration configuration, WGHorseLoginRequest source)
        {
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=Login&Token={System.Web.HttpUtility.UrlEncode(source.Token)}&PartnerCode={configuration.PartnerCode}&Lang={source.Lang}";
            var prefix = $"{_gameProviderCode }_Login";

            return ExecuteJsonRequestAsync<object, WGHorseLoginResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGHorseResponse> IWGHorseApiClient.AccessMoneyAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyRequest source)
        {
            var data = $"{source.PlayerCode}|{source.AmountType}|{source.OrderId}|{source.Amount}";
            var dataEncrypt = WGHorseUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=AccessMoney&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }_AccessMoney";

            return ExecuteJsonRequestAsync<object, WGHorseResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGHorseResponse> IWGHorseApiClient.AccessMoneyQueryAsync(WGHorseConfiguration configuration, WGHorseAccessMoneyQueryRequest source)
        {
            var data = source.OrderId;
            var dataEncrypt = WGHorseUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=AccessMoneyQuery&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }_AccessMoneyQuery";

            return ExecuteJsonRequestAsync<object, WGHorseResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }

        ValueTask<WGHorseGetBalanceResponse> IWGHorseApiClient.GetBalanceAsync(WGHorseConfiguration configuration, WGHorseGetBalanceRequest source)
        {
            var data = source.PlayerCode;
            var dataEncrypt = WGHorseUtility.AesEncrypt(data, configuration.PartnerKey, configuration.IV);
            var path = $"{configuration.ApiUrl}?m=api&c=index&a=GetBalance&Hash={dataEncrypt}&PartnerCode={configuration.PartnerCode}";
            var prefix = $"{_gameProviderCode }_GetBalance";

            return ExecuteJsonRequestAsync<object, WGHorseGetBalanceResponse>(HttpMethod.Get, path, null, logPrefix: prefix);
        }
    }
}
