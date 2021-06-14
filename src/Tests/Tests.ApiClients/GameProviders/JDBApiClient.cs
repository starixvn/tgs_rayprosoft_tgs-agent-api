// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IJDBApiClient
    {
        ValueTask<BaseActionResponse> GetPlayerBalanceAsync(GetBalanceRequest request);
        ValueTask<BaseActionResponse> PlaceAndSettleBetAsync(PlaceAndSettleBetRequest request);
        ValueTask<BaseActionResponse> CancelTransactionsAsync(CancelTransactionRequest request);
    }

    public sealed class JDBApiClient : BaseApiClient, IJDBApiClient
    {
        private static readonly string RoutePrefix = "api/jdb/entrance";
        private readonly ISymmetricCryptoProvider _cryptoProvider;
        private readonly ISettingOperations _settingOperations;

        public JDBApiClient(
            HttpClient client, 
            IApiClientLogger logger, 
            IJsonSerializationProvider json, 
            ISymmetricCryptoProviderFactory cryptoProviderFactory,
            ISettingOperations settingOperations)
            : base(client, logger, json)
        {
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);
            _settingOperations = settingOperations;
        }

        ValueTask<BaseActionResponse> IJDBApiClient.GetPlayerBalanceAsync(GetBalanceRequest request) =>
            SendRequestAsync(Json.SerializeToString(request));

        ValueTask<BaseActionResponse> IJDBApiClient.PlaceAndSettleBetAsync(PlaceAndSettleBetRequest request) =>
            SendRequestAsync(Json.SerializeToString(request));
        
        ValueTask<BaseActionResponse> IJDBApiClient.CancelTransactionsAsync(CancelTransactionRequest request) =>
            SendRequestAsync(Json.SerializeToString(request));

        private async ValueTask<BaseActionResponse> SendRequestAsync(string jsonString)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts
                    .GameProviderConfigurations.JDB);
            
            var x = await _cryptoProvider.EncryptStringAsync(
                jsonString, configuration.AESIv, configuration.AESKey);
            
            const string path = "api/jdb/entrance";
            return await ExecuteFormRequestAsync<BaseActionResponse>(
                path,
                new Dictionary<string, string> {{"x", ToUrlSafeAesEncryptedString(x)}});

        }

        private string ToUrlSafeAesEncryptedString(string encryptedString)
        {
            return encryptedString.TrimEnd(new[] {'='}).Replace('+', '-').Replace('/', '_');
        }
    }
}