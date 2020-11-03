namespace TGS.Core.Models.Services.Data
{
    public record LocalizedStringResult
    {
        public string Context { get; init; }
        public string Key { get; set; }
        public string Translation { get; init; }
        public string CultureCode { get; init; }
    }
}
