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
        private readonly MD5 _md5;

        public BetConstructUtility()
        {
            _md5 = MD5.Create();
        }

        string IBetConstructUtility.MD5Hash(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = _md5.ComputeHash(data);

            return Convert.ToHexString(hash).ToLower();
        }
    }
}
