using TGS.Services.Report.Domain;

namespace TGS.Clients.Report.Client.Models.Market
{
    public class AdditionalExpensesModel
    {
        public AdditionalExpensesType AdditionalExpensesType { get; set; }

        public decimal Amount { get; set; }
    }
}