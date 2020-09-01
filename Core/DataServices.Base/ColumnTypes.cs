namespace TGS.Core.DataServices.Base
{
    public static class ColumnTypes
    {
        public const string Varchar = "VARCHAR";
        public const string DatetimeOffset = "datetimeoffset";
        public const string TinyInt = "tinyint";
        public const string SmallInt = "smallint";
        public const string Int = "int";
        public static string Varbinary(int length) => $"VARBINARY({length})";
        public static string Nvarchar(int length) => $"NVARCHAR({length})";
        public static string NvarcharMax => $"NVARCHAR(max)";
    }
}
