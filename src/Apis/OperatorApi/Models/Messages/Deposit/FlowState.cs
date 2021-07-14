namespace TGS.Apis.OperatorApi.Models.Messages.Deposit
{
    public enum FlowState
    {
        Unknown = 0,
        WaitingPayment,
        Review,
        Approved,
        Rejected
    }
}