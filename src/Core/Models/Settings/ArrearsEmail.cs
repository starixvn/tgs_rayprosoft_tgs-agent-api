namespace TGS.Core.Models.Settings
{
    public record ArrearsEmail
    {
        public ManagerEmailInfo Manager { get; init; }
        public ArrearsEmailInfo SettlementDay { get; init; }
        public ArrearsEmailInfoDaily[] Daily { get; init; }
    }

    public record ArrearsEmailInfoDaily
    {
        public decimal SendPercentage { get; init; }
        public string Subject { get; init; }
        public string Content { get; init; }
    }

    public record ArrearsEmailInfo
    {
        public string Subject { get; init; }
        public string Content { get; init; }
    }

    public record ManagerEmailInfo
    {
        public string[] Emails { get; set; }
        public string Subject { get; init; }
        public string Content { get; init; }
    }
}