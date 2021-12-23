// unset

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using Tgs.Core.Infrastructures.Security;
using Tgs.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.JDB;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Shared.Game.ApiClients.JDB
{
    public class JDBApiClient : BaseApiClient, IJDBApiClient
    {
        private readonly ISymmetricCryptoProvider _cryptoProvider;

        public JDBApiClient(
            ISymmetricCryptoProviderFactory cryptoProviderFactory,
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);
        }

        #region Game Provider Client Api

        ValueTask<BetHistoryResponse> IJDBApiClient.GetBetHistoryUrlAsync(
            Uri apiUrl,
            JDBAgent agent,
            BetHistoryRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.BetHistory";

            return ExecuteRequestAsync<BetHistoryRequest, BetHistoryResponse>(apiUrl, agent, request, prefix);
        }

        ValueTask<GetTokenResponse> IJDBApiClient.GetTokenAsync(
            Uri apiUrl,
            JDBAgent agent,
            GetTokenRequest request)
        {
            var prefix = $"{Consts.GameProvider.JDB}.GetToken";

            return ExecuteRequestAsync<GetTokenRequest, GetTokenResponse>(apiUrl, agent, request, prefix);
        }

        private async ValueTask<TResponse> ExecuteRequestAsync<TRequest, TResponse>(
            Uri apiUrl,
            JDBAgent agent,
            TRequest data,
            string logPrefix)
        {
            var jsonString = Json.SerializeToString(data);

            var encryptedString =
                await _cryptoProvider.EncryptStringAsync(jsonString,
                    agent.AESIv,
                    agent.AESKey,
                    encoding: Encoding.UTF8);

            var parameters = new Dictionary<string, string>
            {
                ["dc"] = agent.ClientDomain,
                ["x"] = encryptedString
            };

            return await ExecuteFormRequestAsync<TResponse>(apiUrl,
                "apiRequest.do",
                parameters,
                logPrefix: logPrefix);
        }

        #endregion Game Provider Client Api
    }
}
