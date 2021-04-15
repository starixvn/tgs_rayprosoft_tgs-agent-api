// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace TGS.Core.Models
{
    public static class SettingConsts
    {
        public const string Menus = "Menus";
        public const string Vip = "Vip";
        public const string SuperLicenseeCodes = "SuperLicenseeCodes";
        public const string PromotionType = "PromotionTypes";

        public static class GameProviderConfigurations
        {
            public static readonly string Template = "{0}Configuration";

            public static readonly string SBOBET = string.Format(Template, Consts.GameProvider.SBOBET);
            public static readonly string Habanero = string.Format(Template, Consts.GameProvider.Habanero);
        }
    }
}