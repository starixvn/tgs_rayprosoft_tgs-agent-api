// unset

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Shared.Game.ApiClients.JDB
{
    public interface IJDBApiClient
    {
        ValueTask<GetTokenResponse> GetTokenAsync(GetTokenRequest request);

        ValueTask<BetHistoryResponse> GetBetHistoryUrlAsync(BetHistoryRequest request);
    }

    public class JDBApiClient : BaseApiClient, IJDBApiClient
    {
        private readonly ISymmetricCryptoProvider _cryptoProvider;
        private readonly ISettingOperations _settingOperations;

        public JDBApiClient(
            ISymmetricCryptoProviderFactory cryptoProviderFactory,
            ISettingOperations settingOperations,
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);
            _settingOperations = settingOperations;
        }

        #region Game Provider Client Api

       ValueTask<GetTokenResponse> IJDBApiClient.GetTokenAsync(GetTokenRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.GetToken";
            
            return ExecuteRequestAsync<GetTokenRequest, GetTokenResponse>(request, prefix);
        }

        ValueTask<BetHistoryResponse> IJDBApiClient.GetBetHistoryUrlAsync(BetHistoryRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.BetHistory";
            return ExecuteRequestAsync<BetHistoryRequest, BetHistoryResponse>(request, prefix);
        }

        private async ValueTask<TResponse> ExecuteRequestAsync<TRequest, TResponse>(TRequest data, string logPrefix)
        {
            var jsonString = Json.SerializeToString(data);
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts.GameProviderConfigurations.JDB);
            var encryptedString =
                await _cryptoProvider.EncryptStringAsync(jsonString, configuration.AESIV, configuration.AESKEY, encoding: Encoding.UTF8);

            var parameters = new Dictionary<string, string>
            {
                ["dc"] = configuration.ClientDomain,
                ["x"] = encryptedString
            };

            return await ExecuteFormRequestAsync<TResponse>(configuration.ApiUrl, "apiRequest.do", parameters, logPrefix: logPrefix);
        }

        #endregion
    }
}