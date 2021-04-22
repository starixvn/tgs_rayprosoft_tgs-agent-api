namespace TGS.Core.Models.Settings
{
    public record PromotionTypeGroup
    {
        public string Name { get; set; }
        public PromotionType[] Types { get; set; }
        public Translation Trans { get; set; }
    }

    public record PromotionType
    {
        public string Name { get; set; }
        public string SettlementIntervalType { get; set; }
        public Translation Trans { get; set; }
    }

    public record Translation
    {
        public string En { get; set; }

        public string Zh { get; set; }
    }
}