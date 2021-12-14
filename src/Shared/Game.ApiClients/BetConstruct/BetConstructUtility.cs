using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TGS.Shared.Game.ApiClients.BetConstruct
{
    public interface IBetConstructUtility
    {
        string ComputeSignature(Dictionary<string, object> hashDict, string sharedKey);
    }

    internal class BetConstructUtility : IBetConstructUtility
    {
        private readonly MD5 _md5;
        private readonly ILogger<BetConstructUtility> _logger;

        public BetConstructUtility(ILogger<BetConstructUtility> logger)
        {
            _md5 = MD5.Create();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        string MD5Hash(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = _md5.ComputeHash(data);

            return Convert.ToHexString(hash).ToLower();
        }

        string IBetConstructUtility.ComputeSignature(Dictionary<string, object> hashDict, string sharedKey)
        {
            _logger.LogInformation($"ComputeSignature hashDict:{JsonSerializer.Serialize(hashDict)}");

            var content = string.Join(
                "",
                hashDict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));

            var plainText = $"{content}{sharedKey}";
            _logger.LogInformation($"ComputeSignature plainText:{plainText}");
            var hash = MD5Hash(plainText);            
            _logger.LogInformation($"ComputeSignature hash:{hash}");

            return hash;
        }
    }
}
