namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.Enums
{
    public static class ColumnTypes
    {
        public const string Varchar = "VARCHAR";
        public const string DatetimeOffset = "datetimeoffset";
        public const string Varbinary = "VARBINARY";
        public const string TinyInt = "tinyint";
        public static string Nvarchar(int length) => $"NVARCHAR({length})";
    }
}
