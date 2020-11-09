namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class PaymentFeeModel
    {
        public decimal? FeePercentage { get; set; }
        public decimal? LowestFee { get; set; }
        public decimal? HighestFee { get; set; }
    }
}