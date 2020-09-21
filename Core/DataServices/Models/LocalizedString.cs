namespace TGS.Core.DataServices.Models
{
    public class LocalizedString
    {
        public string Context { get; set; }
        public string Code { get; set; }
        public string Translation { get; set; }
        public string CultureCode { get; set; }
        public Language Language { get; set; }
    }
}