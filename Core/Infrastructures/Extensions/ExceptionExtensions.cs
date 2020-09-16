using System;
using System.Text;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetMessageChain(this Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            var i = 0;
            var sb = new StringBuilder();
            while (ex != null)
            {
                if (i > 0)
                {
                    sb.Append("; INNER ").Append(i).Append(':').Append(' ');
                }

                sb.Append(ex.Message);
                ex = ex.InnerException;
                ++i;
            }

            return sb.ToString();
        }
    }
}