namespace TGS.Core.DataServices.Bet.Migrations
{
    internal static class Consts
    {
        private const string Namespace = "TGS.Core.DataServices.Bet.SqlScripts";

        internal static readonly string TypeSqlFormat = $"{Namespace}.TYPE.{{0}}.sql";
        internal static readonly string StoreProcedureSqlFormat = $"{Namespace}.SP.{{0}}.sql";
        internal static readonly string ViewSqlFormat = $"{Namespace}.View.{{0}}.sql";
    }
}