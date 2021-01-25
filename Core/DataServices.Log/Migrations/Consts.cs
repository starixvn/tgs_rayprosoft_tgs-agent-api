namespace TGS.Core.DataServices.Log.Migrations
{
    internal static class Consts
    {
        private const string Namespace = "TGS.Core.DataServices.Log.SqlScripts";
        internal static readonly string StoreProcedureSqlForamt = $"{Namespace}.Procedures.{{0}}.sql";
        internal static readonly string FunctionSqlFormat = $"{Namespace}.Functions.{{0}}.sql";
        internal static readonly string IndexSqlFormat = $"{Namespace}.Indexes.{{0}}.sql";
    }
}