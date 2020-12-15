namespace TGS.Core.Models.Services.Games
{
    public record PlayerValidationResult
    {
        public decimal Balance { get; init; }
        public string GameProviderCurrency { get; init; }
        public string Language { get; init; }
        public string Username { get; init; }
        public bool IsTestPlayer { get; init; }
        public string BrandCode { get; init; }
        public string LicenseeCode { get; init; }
        public string ResellerCode { get; init; }
    }
}