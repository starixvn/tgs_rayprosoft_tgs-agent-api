using TGS.Services.Report.Domain;

namespace TGS.Services.Report.Application.Models.Market
{
    public class AdditionalExpensesModel
    {
        public AdditionalExpensesType AdditionalExpensesType { get; set; }

        public decimal Amount { get; set; }
    }
}