namespace TGS.Core.Models.Enums
{
    public enum AgentTransactionType
    {
        Deposit = 0,
        Withdrawal = 1,
        CompanyAdjust = 2,
        Bonus = 3,
        Rebate = 4,
        Dividend = 5,
        Promotion = 6,
        CancelWithdrawal = 9,

        Other = 99,

        TransferAdjust = 5300
    }
}