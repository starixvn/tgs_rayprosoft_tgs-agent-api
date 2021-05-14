
namespace TGS.Services.Payment
{
    public enum PaymentFlowState
    {
        Init = 0,
        WaitApprove = 1000,
        Reject = 7000,
        Approved = 8000
    }
}
