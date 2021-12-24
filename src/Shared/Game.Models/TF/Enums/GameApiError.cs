using System.ComponentModel;

namespace TGS.Shared.Game.Models.TF.Enums
{
    public enum GameApiError
    {
        [Description("Undefined")]
        Undefined = 1,

        [Description("Withdrawal is not allowed within 3sec after member's last transaction")]
        WithdrawalIsNotAllowedWithin3secAfterMemberLastTransaction = 2,

        [Description("Unknown or invalid member id/code")]
        UnknownOrInvalidMemberId = 3,

        [Description("Invalid amount for withdrawal/deposit")]
        InvalidAmountForWithdrawalDeposit = 4,

        [Description("Trying to withdraw more than current balance")]
        TryingToWithdrawMoreThanCurrentBalance = 5,

        [Description("Invalid operator id")]
        InvalidOperatorId = 6,

        [Description("Value requested is more or less than the allowed range")]
        ValueRequestedIsMoreOrLessThanTheAllowedRange = 7,

        [Description("Post-data currency and Partner's currency is not matched")]
        PostDataCurrencyAndPartnerCurrencyIsNotMatched = 8
    }
}
