using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Log.Migrations
{
    internal static class Consts
    {
        private const string Namespace = "TGS.Core.DataServices.Log.SqlScripts";
        internal static readonly string StoreProcedureSqlForamt = $"{Namespace}.SP.{{0}}.sql";
    }
}
