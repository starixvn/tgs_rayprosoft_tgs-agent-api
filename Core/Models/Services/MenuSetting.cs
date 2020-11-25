namespace TGS.Core.Models.Services
{
    public class MenuSetting
    {
        public MenuSetting(string code)
        {
            Code = code;
            IsDisplay = true;
        }

        public string Code { get; set; }
        public bool IsDisplay { get; set; }
        public MenuSetting[] SubMenus { get; set; }
    }
}