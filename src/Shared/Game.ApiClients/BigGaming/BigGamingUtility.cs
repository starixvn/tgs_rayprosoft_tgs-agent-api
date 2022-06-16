using Tgs.Core.Infrastructures.Security;
using Tgs.Core.Infrastructures.Security.Enums;

namespace TGS.Shared.Game.ApiClients.BigGaming;

public static class BigGamingUtility
{
    private static readonly IHashProvider _HashProvider;

    static BigGamingUtility()
    {
        _HashProvider = new HashProvider();
    }

    public static string GetSecretCode(string agentLoginIdPassword)
        => _HashProvider.Hash(agentLoginIdPassword, HashAlgorithmKind.Sha1, HashStringKind.Base64);
}