namespace TGS.Core.Models.Settings
{
    public record NetProfitEmail
    {
        public decimal SendPercentage { get; init; }
        public string Subject { get; init; }
        public string Content { get; init; }
    }
}