namespace TGS.Core.DataServices.Tgs.Migrations
{
    internal static class Consts
    {
        private const string Namespace = "TGS.Core.DataServices.Tgs.SqlScripts";

        internal static readonly string StoreProcedureSqlFormat = $"{Namespace}.SP.{{0}}.sql";
        internal static readonly string TypeSqlFormat = $"{Namespace}.TYPE.{{0}}.sql";
    }
}