using System;
using System.Text;
using System.Threading;

namespace TGS.Core.Infrastructures.Utility
{
    public static class PlayerCodeGenerator
    {
        private static readonly object Instance = new object();

        public static string NewId()
        {
            lock (Instance)
            {
                Thread.Sleep(1);
                var ticks = DateTime.Now.Ticks;
                return ConvertToBase(ticks, 36);
            }
        }

        private static string ConvertToBase(long num, int nbase)
        {
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // check if we can convert to another base
            if (nbase < 2 || nbase > chars.Length)
                return string.Empty;

            var newNumber = new StringBuilder();

            // in r we have the offset of the char that was converted to the new base
            while (num >= nbase)
            {
                var r = num % nbase;
                newNumber.Insert(0, chars[(int)r]);
                num = num / nbase;
            }
            // the last number to convert
            newNumber.Insert(0, chars[(int)num]);

            return newNumber.ToString().ToLower();
        }
    }
}
