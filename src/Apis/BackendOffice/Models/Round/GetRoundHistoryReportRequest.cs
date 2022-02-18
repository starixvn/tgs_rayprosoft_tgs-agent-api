namespace TGS.Apis.BackendOffice.Models.Round
{
    public record GetRoundHistoryReportRequest
    {
        public string GameProviderCode { get; set; }

        public string PlayerCode { get; set; }

        public string Language { get; set; }
    }
}