namespace TGS.Core.Models
{
    public static class SettingConsts
    {
        public static class GameProviderConfigurations
        {
            public const string Template = "{0}Configuration";

            public static readonly string SBOBET = string.Format(Template, Consts.GameProvider.SBOBET);
        }        
    }
}
