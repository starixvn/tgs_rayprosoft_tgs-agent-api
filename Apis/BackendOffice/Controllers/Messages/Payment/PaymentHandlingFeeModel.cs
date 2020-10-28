namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class PaymentHandlingFeeModel
    {
        public decimal? HandlingFeePercentage { get; set; }
        public decimal? LowestHandlingFee { get; set; }
        public decimal? HighestHandlingFee { get; set; }
    }
}