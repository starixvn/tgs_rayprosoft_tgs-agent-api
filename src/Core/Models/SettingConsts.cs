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
        public const string CurrencyNetProfit = "CurrencyNetProfit";
        public const string ArrearsEmail = "ArrearsEmail";

        public static class GameProviderConfigurations
        {
            public static readonly string Template = "{0}Configuration";

            public static readonly string SBOBET = string.Format(Template, Consts.GameProvider.SBOBET);
            public static readonly string Habanero = string.Format(Template, Consts.GameProvider.Habanero);
            public static readonly string PragmaticPlay = string.Format(Template, Consts.GameProvider.PragmaticPlay);
            public static readonly string AsiaGaming = string.Format(Template, Consts.GameProvider.AsiaGaming);
            public static readonly string JDB = string.Format(Template, Consts.GameProvider.JDB);
            public static readonly string Saba = string.Format(Template, Consts.GameProvider.Saba);
            public static readonly string KY = string.Format(Template, Consts.GameProvider.KY);
            public static readonly string VR = string.Format(Template, Consts.GameProvider.VR);
            public static readonly string IMESports = string.Format(Template, Consts.GameProvider.IMESports);
            public static readonly string BigGaming = string.Format(Template, Consts.GameProvider.BigGaming);
            public static readonly string SexyGaming = string.Format(Template, Consts.GameProvider.SexyGaming);
            public static readonly string Skywind = string.Format(Template, Consts.GameProvider.Skywind);
            public static readonly string Evolution = string.Format(Template, Consts.GameProvider.Evolution);
        }
    }
}