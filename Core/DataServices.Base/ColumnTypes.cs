namespace TGS.Core.DataServices.Base
{
    public static class ColumnTypes
    {
        public const string DatetimeOffset = "datetimeoffset";
        public const string TinyInt = "tinyint";
        public const string SmallInt = "smallint";
        public const string Int = "int";
        public const string NvarcharMax = "NVARCHAR(max)";
        
        public static string Varbinary(int length) => $"varbinary({length})";
        public static string Nvarchar(int length) => $"nvarchar({length})";
        public static string Varchar(int? length = null) => length != null && length > 0 ? $"varchar({length})" : "varchar";
        public static string Decimal(int precision, int scale) => $"decimal({precision},{scale})";
    }
}
