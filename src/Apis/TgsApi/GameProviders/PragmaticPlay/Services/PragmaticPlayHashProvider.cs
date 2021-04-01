using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Shared.Game.Models.PragmaticPlay.Common;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayHashProvider
    {
        string ComputeHash(IFormCollection collections, params string[] additionalData);

        string ComputeHash<T>(T request, params string[] additionalData) where T : HashRequest;
    }

    public sealed class PragmaticPlayHashProvider : IPragmaticPlayHashProvider
    {
        private readonly string[] _bypassHashKeys =
        {
            PragmaticPlayConsts.Parameters.Hash, PragmaticPlayConsts.Parameters.BypassHashVerify
        };

        private readonly IHashProvider _hashProvider;
        private readonly IJsonSerializationProvider _json;

        public PragmaticPlayHashProvider(IHashProvider hashProvider, IJsonSerializationProvider json)
        {
            _hashProvider = hashProvider;
            _json = json;
        }

        string IPragmaticPlayHashProvider.ComputeHash(IFormCollection collections, params string[] additionalData)
        {
            var parameters = collections.Where(c => _bypassHashKeys.IgnoreCaseNotContains(c.Key))
                .ToDictionary(c => c.Key, c => c.Value.ToString());
            return ComputeHash(parameters, additionalData);
        }

        string IPragmaticPlayHashProvider.ComputeHash<T>(T request, params string[] additionalData)
        {
            var options = new JsonSerializerOptions {WriteIndented = false};
            var value = _json.SerializeToString(request, options);
            var items = _json.DeserializeFromString<Dictionary<string, object>>(value, options);
            var parameters = items.Where(c => _bypassHashKeys.IgnoreCaseNotContains(c.Key))
                .ToDictionary(p => p.Key, p => p.Value.ToString());

            return ComputeHash(parameters, additionalData);
        }

        private string ComputeHash(Dictionary<string, string> collections, string[] additionalData)
        {
            var sortedParameters = new SortedDictionary<string, string>(collections, StringComparer.OrdinalIgnoreCase);
            var raw = sortedParameters.Select(p => $"{p.Key}={p.Value}").JoinAsString("&");

            if (additionalData?.Length > 0)
            {
                raw += additionalData.JoinAsString(string.Empty);
            }

            return _hashProvider.Hash(raw, HashAlgorithmKind.Md5);
        }
    }
}