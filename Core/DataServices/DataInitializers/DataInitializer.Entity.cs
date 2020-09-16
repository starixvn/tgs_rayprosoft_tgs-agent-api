namespace TGS.Core.DataServices.DataInitializers
{
    public sealed partial class DataInitializer
    {
        public class _Language
        {
            public _Language(string cultureCode, string languageCode, string name, string localizedName)
            {
                CultureCode = cultureCode;
                LanguageCode = languageCode;
                Name = name;
                LocalizedName = localizedName;
            }

            public string CultureCode { get; set; }
            public string LanguageCode { get; set; }
            public string Name { get; set; }
            public string LocalizedName { get; set; }
        }

        public class _Currency
        {
            public _Currency(string code, string name, string nativeName, string symbol)
            {
                Code = code;
                Name = name;
                NativeName = nativeName;
                Symbol = symbol;
            }

            public string Code { get; set; }
            public string Name { get; set; }
            public string NativeName { get; set; }
            public string Symbol { get; set; }
        }
    }
}