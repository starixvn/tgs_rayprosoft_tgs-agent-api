namespace TGS.Core.Models.Settings
{
    public record PromotionType
    {
        public string Name { get; set; }
        public string[] Types { get; set; }
    }
}