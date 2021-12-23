// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Tgs.Core.Infrastructures.Security;
using Tgs.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IJDBApiClient
    {
        ValueTask<BaseActionResponse> CancelTransactionsAsync(CancelTransactionRequest request, string currencyCode);

        ValueTask<BaseActionResponse> GetPlayerBalanceAsync(GetBalanceRequest request, string currencyCode);

        ValueTask<BaseActionResponse> PlaceAndSettleBetAsync(PlaceAndSettleBetRequest request, string currencyCode);
    }

    public sealed class JDBApiClient : BaseApiClient, IJDBApiClient
    {
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

        ValueTask<BaseActionResponse> IJDBApiClient.CancelTransactionsAsync(
            CancelTransactionRequest request,
            string currencyCode)
            => SendRequestAsync(Json.SerializeToString(request), currencyCode);

        ValueTask<BaseActionResponse> IJDBApiClient.GetPlayerBalanceAsync(
            GetBalanceRequest request,
            string currencyCode)
            => SendRequestAsync(Json.SerializeToString(request), currencyCode);

        ValueTask<BaseActionResponse> IJDBApiClient.PlaceAndSettleBetAsync(
            PlaceAndSettleBetRequest request,
            string currencyCode)
            => SendRequestAsync(Json.SerializeToString(request), currencyCode);

        private static string ToUrlSafeAesEncryptedString(string encryptedString)
        {
            var base64 = Convert.FromBase64String(encryptedString);

            return WebEncoders.Base64UrlEncode(base64);
        }

        private async ValueTask<BaseActionResponse> SendRequestAsync(string jsonString, string currencyCode)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts
                    .GameProviderConfigurations.JDB);

            var agent = configuration.Agents.FirstOrDefault(a => a.CurrencyCode == currencyCode)!;

            var encryptedString = await _cryptoProvider.EncryptStringAsync(
                jsonString,
                agent.AESIv,
                agent.AESKey,
                padding: PaddingMode.Zeros,
                encoding: Encoding.UTF8);

            var path = $"api/jdb/entrance/{currencyCode.ToLower()}";

            return await ExecuteFormRequestAsync<BaseActionResponse>(
                path,
                new Dictionary<string, string>
                {
                    {
                        "x", ToUrlSafeAesEncryptedString(encryptedString)
                    }
                });
        }
    }
}
