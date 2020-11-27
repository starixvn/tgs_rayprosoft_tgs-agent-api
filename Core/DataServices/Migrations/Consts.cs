namespace TGS.Core.DataServices.Migrations
{
    internal static class Consts
    {
        internal const string StoreProcedureSqlFormat = "TGS.Core.DataServices.SqlScripts.SP.{0}.sql";
        internal const string TypeSqlFormat = "TGS.Core.DataServices.SqlScripts.TYPE.{0}.sql";

        public static object GameProvider { get; internal set; }
    }
}