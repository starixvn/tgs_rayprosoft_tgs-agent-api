using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Shared.Game.Models.AllBet.Outgoing;

namespace TGS.Shared.Game.ApiClients.AllBet
{
    public interface IAllBetApiClient
    {
        ValueTask<LoginResponse> LoginAsync(Uri apiUrl, string propertyId, string apiKey,
            LoginRequest request);

        ValueTask<CheckOrCreateMemberResponse> CheckOrCreateMemberAsync(Uri apiUrl, string propertyId,
            string apiKey, CheckOrCreateMemberRequest request);
    }

    public class AllBetApiClient : BaseApiClient, IAllBetApiClient
    {
        private readonly IHashProvider _hashProvider;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions {WriteIndented = false};

        public AllBetApiClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json,
            IHashProvider hashProvider)
            : base(client, logger, json)
        {
            _hashProvider = hashProvider;
        }

        ValueTask<LoginResponse> IAllBetApiClient.LoginAsync(Uri apiUrl, string propertyId, string apiKey,
            LoginRequest request)
        {
            var method = HttpMethod.Post;
            var path = "/Login";
            var (headers, contentHeaders) = GetHeaders(method, propertyId, apiKey, path, request);

            return ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(method, apiUrl, path,
                request, headers, contentHeaders, options: _options);
        }

        ValueTask<CheckOrCreateMemberResponse> IAllBetApiClient.CheckOrCreateMemberAsync(Uri apiUrl, string propertyId,
            string apiKey, CheckOrCreateMemberRequest request)
        {
            var method = HttpMethod.Post;
            var path = "/CheckOrCreate";
            var (headers, contentHeaders) = GetHeaders(method, propertyId, apiKey, path, request);

            return ExecuteJsonRequestAsync<CheckOrCreateMemberRequest, CheckOrCreateMemberResponse>(
                method, apiUrl, path, request, headers, contentHeaders, options: _options);
        }

        private (IReadOnlyDictionary<string, string> Headers, IReadOnlyDictionary<string, string> ContentHeaders)
            GetHeaders<T>(HttpMethod method, string propertyId, string apiKey, string path, T request)
        {
            var body = Json.SerializeToString(request, _options);
            var md5 = _hashProvider.Hash(body, HashAlgorithmKind.Md5, HashStringKind.Base64, Encoding.UTF8);
            var contentType = "application/json";

            // RFC 1123 pattern
            var date = DateTime.Now.ToUniversalTime().ToString("R", new DateTimeFormatInfo());

            var stringToSign = string.Join("\n", method.ToString(), md5, contentType, date, path);

            using var hasher = new HMACSHA1(Convert.FromBase64String(apiKey));
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            var encrypted = hash.ToBase64String();

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"AB {propertyId}:{encrypted}",
                ["Date"] = date
            };

            var contentHeaders = new Dictionary<string, string>
            {
                ["Content-MD5"] = md5,
                ["Content-Type"] = contentType
            };

            return (headers, contentHeaders);
        }
    }
}