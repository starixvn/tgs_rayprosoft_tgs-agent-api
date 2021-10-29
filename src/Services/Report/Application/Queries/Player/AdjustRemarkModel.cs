using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Queries.Player
{
    public class AdjustRemarkModel
    {
        public AdjustmentTransactionType TransactionType { get; set; }
        public string TargetAgent { get; set; }
        public string Remark { get; set; }
    }
}