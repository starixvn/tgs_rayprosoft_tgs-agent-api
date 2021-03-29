using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayHashProvider
    {
        string ComputeHash(IFormCollection collections, params string[] additionalData);
    }

    public sealed class PragmaticPlayHashProvider : IPragmaticPlayHashProvider
    {
        private readonly string[] _bypassHashKeys =
        {
            PragmaticPlayConsts.Parameters.Hash, PragmaticPlayConsts.Parameters.BypassHashVerify
        };

        private readonly IHashProvider _hashProvider;

        public PragmaticPlayHashProvider(IHashProvider hashProvider)
        {
            _hashProvider = hashProvider;
        }

        string IPragmaticPlayHashProvider.ComputeHash(IFormCollection collections, params string[] additionalData)
        {
            var parameters = collections.Where(c => _bypassHashKeys.IgnoreCaseNotContains(c.Key))
                .ToDictionary(c => c.Key, c => c.Value.ToString());
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