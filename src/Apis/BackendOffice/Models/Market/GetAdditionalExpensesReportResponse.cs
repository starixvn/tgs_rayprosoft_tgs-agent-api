namespace TGS.Apis.BackendOffice.Models.Market
{
    public record GetAdditionalExpensesReportResponse
    {
        public AdditionalExpensesReportModel[] AdditionalExpenses { get; init; }
    }
}