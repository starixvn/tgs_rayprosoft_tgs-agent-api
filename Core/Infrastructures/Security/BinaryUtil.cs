using System;
using System.Linq;

namespace TGS.Core.Infrastructures.Security
{
    public static class BinaryUtil
    {
        public static byte[] HexStringToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] ConcatenateBytes(byte[] a, byte[] b)
        {
            var r = new byte[a.Length + b.Length];

            Buffer.BlockCopy(a, 0, r, 0, a.Length);
            Buffer.BlockCopy(b, 0, r, a.Length, b.Length);

            return r;
        }
    }
}