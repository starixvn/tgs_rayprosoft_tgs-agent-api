using System;
using System.Text;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToBase64String(this byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        public static string ToHexString(this byte[] byteArray)
        {
            var sb = new StringBuilder();
            foreach (byte t in byteArray)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
