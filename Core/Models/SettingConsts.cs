// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace TGS.Core.Models
{
    public static class SettingConsts
    {
        public const string Menus = "Menus";
        public const string Vip = "Vip";
        public const string SuperLicenseeCodes ="SuperLicenseeCodes";

        public static class GameProviderConfigurations
        {
            public const string Template = "{0}Configuration";

            public static readonly string SBOBET = string.Format(Template, Consts.GameProvider.SBOBET);
        }
    }
}