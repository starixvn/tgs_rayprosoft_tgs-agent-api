namespace TGS.Services.Payment
{
    public interface IWithdrawProcessorFactory
    {
        IWithdrawProcessor GetWithdrawProcessor(string paymentCode);
    }
}
