namespace TGS.Apis.BackendOffice.Models.Market
{
    public record AdditionalExpensesReportModel
    {
        public AdditionalExpensesType AdditionalExpensesType { get; init; }

        public decimal Amount { get; init; }
    }
}