using System;
using System.Threading;

namespace TGS.Apis.OperatorApi
{
    public static class CodeGenerator
    {
        private static readonly object Instance = new object();

        public static string NewId(int length)
        {
            lock (Instance)
            {
                Thread.Sleep(1);
                var ticks = DateTime.Now.Ticks;
                return ConvertToBase(ticks, length);
            }
        }

        private static string ConvertToBase(long num, int nbase)
        {
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            // check if we can convert to another base
            if (nbase < 2 || nbase > chars.Length)
                return "";

            var newNumber = "";

            // in r we have the offset of the char that was converted to the new base
            while (num >= nbase)
            {
                var r = num % nbase;
                newNumber = chars[(int)r] + newNumber;
                num = num / nbase;
            }
            // the last number to convert
            newNumber = chars[(int)num] + newNumber;

            return newNumber.ToLower();
        }
    }
}
