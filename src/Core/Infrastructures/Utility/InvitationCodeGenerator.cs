using System;
using System.Linq;
using System.Threading;

namespace TGS.Core.Infrastructures.Utility
{
    public static class InvitationCodeGenerator
    {
        private static readonly object Instance = new();

        public static string NewId()
        {
            lock (Instance)
            {
                Thread.Sleep(1);
                return GenerateString(6);
            }
        }

        private static string GenerateString(int num)
        {
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            
            var bytes =  Guid.NewGuid().ToByteArray();
            var values = Enumerable.Range(0, num)
                .Select(i => chars[(bytes[i] + bytes[num + i]) % 35]);

            return string.Concat(values).ToLower();
        }
    }
}
