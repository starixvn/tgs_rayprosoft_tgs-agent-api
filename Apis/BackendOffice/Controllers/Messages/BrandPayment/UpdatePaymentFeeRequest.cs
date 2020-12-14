namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment
{
    public class UpdatePaymentFeeRequest
    {
        public decimal FeePercentage { get; set; }
        public decimal LowestFee { get; set; }
        public decimal HighestFee { get; set; }
    }
}