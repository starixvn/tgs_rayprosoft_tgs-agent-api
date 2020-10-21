namespace TGS.Core.DataServices.Bet.Migrations
{
    internal static class Consts
    {
        private const string Namespace = "TGS.Core.DataServices.Bet.SqlScripts";

        internal static readonly string TypeSqlForamt = $"{Namespace}.TYPE.{{0}}.sql";
        internal static readonly string StoreProcedureSqlForamt = $"{Namespace}.SP.{{0}}.sql";
        internal static readonly string ViewSqlForamt = $"{Namespace}.View.{{0}}.sql";
    }
}