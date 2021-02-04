using System;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    internal static class Consts
    {
        internal const string BetSchema = "bet";

        private const string Namespace = "TGS.Core.DataServices.Tgs.SqlScripts";

        internal static readonly string ProcedureSqlFormat = $"{Namespace}.Procedures.{{0}}.sql";
        internal static readonly string FunctionSqlFormat = $"{Namespace}.Functions.{{0}}.sql";
        internal static readonly string IndexSqlFormat = $"{Namespace}.Indexes.{{0}}.sql";
        internal static readonly string ViewSqlFormat = $"{Namespace}.Views.{{0}}.sql";
    }
}