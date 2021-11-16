using System;
using System.Security.Cryptography;
using System.Text;

namespace TGS.Shared.Game.ApiClients.BetConstruct
{
    public interface IBetConstructUtility
    {
        string MD5Hash(string plainText);
    }

    internal class BetConstructUtility : IBetConstructUtility
    {
        private static readonly MD5 _Md5;

        string IBetConstructUtility.MD5Hash(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = _Md5.ComputeHash(data);

            return Convert.ToHexString(hash).ToLower();
        }
    }
}
