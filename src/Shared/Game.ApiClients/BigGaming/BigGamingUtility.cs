using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Shared.Game.ApiClients.BigGaming
{
    public static class BigGamingUtility
    {
        private static readonly IHashProvider _hashProvider;

        static BigGamingUtility()
        {
            _hashProvider = new HashProvider();
        }

        public static string GetSecretCode(string agentLoginIdPassword)
        {
            return _hashProvider.Hash(agentLoginIdPassword, HashAlgorithmKind.Sha1, HashStringKind.Base64);
        }
    }
}
