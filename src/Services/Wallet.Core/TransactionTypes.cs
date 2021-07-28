namespace TGS.Services.WalletService
{
    public enum TransactionTypes
    {
        Deposit = 0,
        Withdrawal = 1,
        Adjust = 2,
        Bonus = 3,
        Rebate = 4,
        Dividend = 5,
        Promotion = 6,
        GameProviderFundOut = 7,
        GameProviderFundIn = 8,
        CancelWithdrawal = 9,
        GameProviderDeposit = 10,
        GameProviderWithdrawal = 11,
        Other = 99
    }
}