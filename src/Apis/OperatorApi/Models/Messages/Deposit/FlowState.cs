namespace TGS.Apis.OperatorApi.Models.Messages.Deposit
{
    public enum FlowState
    {
        Unkown = 0,
        WaitingPayment,
        Review,
        Approved,
        Rejected
    }
}