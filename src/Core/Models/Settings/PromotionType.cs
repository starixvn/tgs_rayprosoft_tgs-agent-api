namespace TGS.Core.Models.Settings
{
    public record PromotionType
    {
        public string Name { get; set; }
        public PromotionTypeModel[] Types { get; set; }
    }

    public record PromotionTypeModel
    {
        public string Name { get; set; }
        public string SettlementIntervalType { get; set; }
    }
}