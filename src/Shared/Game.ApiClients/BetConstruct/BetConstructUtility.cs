using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TGS.Shared.Game.ApiClients.BetConstruct
{
    public interface IBetConstructUtility
    {
        string ComputeSignature(Dictionary<string, object> hashDict, string sharedKey);
    }

    internal class BetConstructUtility : IBetConstructUtility
    {
        private readonly MD5 _md5;

        public BetConstructUtility()
        {
            _md5 = MD5.Create();
        }

        string MD5Hash(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = _md5.ComputeHash(data);

            return Convert.ToHexString(hash).ToLower();
        }

        string IBetConstructUtility.ComputeSignature(Dictionary<string, object> hashDict, string sharedKey)
        {
            var content = string.Join(
                "",
                hashDict.Where(x => x.Value != null && x.Value.ToString() != "")
                    .Select(x =>
                    {
                        return $"{x.Key}{x.Value}";
                    }));

            var plainText = $"{content}{sharedKey}";

            var hash = MD5Hash(plainText);

            return hash;
        }
    }
}
