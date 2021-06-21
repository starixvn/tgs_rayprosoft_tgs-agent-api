using System.ComponentModel;

namespace TGS.Shared.Game.Models.Skywind.Enums
{
    public enum GameApiError
    {
        [Description("No errors")]
        Success = 0,
        [Description(@"As we expect idempotency for all our transactions, we require that in the case of receiving a duplicate transaction from us (a duplicate debit/credit request with the same trx_id), you will respond with error_code=1 and the balance value as in the response to the original request.")]
        DuplicateTransaction = 1,
        [Description(@"An unexpected error was encountered while processing the request. It can be caused by a validation error, for example, if the merchant doesn’t exist. Operator can use this error when they don’t expect such behavior from the provider’s side.")]
        MerchantInternalError = -1,
        [Description(@"Returned when the game is launched with the playerCode which doesn’t exist on the merchant’s side or if the playerCode has an invalid value, for example, is empty or has a number value type.")]
        PlayerNotFound = -2,
        [Description(@"Player session has expired or has invalid value.")]
        GameTokenExpired = -3,
        [Description(@"Player account is inactive and they can’t launch games. This error is the extension of the -3 error.")]
        PlayerIsSuspended = -301,
        [Description(@"The bet limit for this player session has been exceeded. This error is the extension of -3 error")]
        BetLimitWasExceeded = -302,
        [Description(@"Player does not have sufficient balance to perform an operation.")]
        InsufficientBalance = -4,
        [Description(@"Returned when the number of free bets for the player is 0.")]
        InsufficientFreeBetsBalance = -5,
        [Description(@"Returned when the request contains the invalid value of the freebet parameter, for example, when the Operator expects the free bet promotion is stored on their side but the promotion was created on Provider’s side and the values of free bet parameters don’t match.")]
        InvalidFreeBet = -6,
        [Description(@"Can be returned in the case of a rollback operation when the transaction id in the request is invalid")]
        TransactionNotFound = -7,
        [Description("Player is on timeout.")]
        PlayerIsOnTimeout = -1500,
        [Description("Player is self-excluded.")]
        PlayerIsSelfExcluded = -1501,
        [Description("Player has reached his deposit limit.")]
        PlayerHasReachedHisDepositLimit = -1502,
        [Description("You have reached your session time limit.")]
        YouHaveReachedYourSessionTimeLimit = -1503,
        [Description("You have reached your daily time limit.")]
        YouHaveReachedYourDailyTimeLimit = -1504,
        [Description("Reality check error.")]
        RealityCheckError = -1505,
        [Description("Player is permanently blocked.")]
        PlayerIsPermanentlyBlocked = -1506,
        [Description("The bet amount you selected exceeds your loss limit for today.")]
        TheBetAmountYouSelectedExceedsYourLossLimitForToday = -1507,
        [Description("You’ve reached your loss limit for today.")]
        YouReachedYourLossLimitForToday = -1508,
        [Description("The bet amount you selected exceeds your loss limit for this week.")]
        TheBetAmountYouSelectedExceedsYourLossLimitForThisWeek = -1509,
        [Description("You’ve reached your loss limit for this week.")]
        YouReachedYourLossLimitForThisWeek = -1510,
        [Description("The bet amount you selected exceeds your loss limit for this month.")]
        TheBetAmountYouSelectedExceedsYourLossLimitForThisMonth = -1511,
        [Description("You’ve reached your loss limit for this month.")]
        YouReachedYourLossLimitForThisMonth = -1512,
        [Description("You’ve reached your session loss limit.")]
        YouReachedYourSessionLossLimit = -1513,
        [Description("Player has exceeded regulatory limits.")]
        PlayerHasExceededRegulatoryLimits = -1514,
        [Description("You’ve reached your session bet limit.")]
        YouReachedYourSessionBetLimit = -1515,
        [Description("You’ve reached your bet limit for today.")]
        YouReachedYourBetLimitForToday = -1516,
        [Description("You’ve reached your bet limit for this week.")]
        YouReachedYourBetLimitForThisWeek = -1517,
        [Description("You’ve reached your bet limit for this month.")]
        YouReachedYourBetLimitForThisMonth = -1518,
        [Description("You’ve reached your withdraw limit.")]
        YouReachedYourWithdrawLimit = -1519,
        [Description("Mandatory limit missing.")]
        MandatoryLimitMissing = -1520,
        [Description("You’ve reached your session game group loss limit.")]
        YouReachedYourSessionGameGroupLossLimit = -1521,
        [Description("You’ve reached your game group loss limit for today.")]
        YouReachedYourGameGroupLossLimitForToday = -1522,
        [Description("You’ve reached your game group loss limit for this week.")]
        YouReachedYourGameGroupLossLimitForThisWeek = -1523,
        [Description("You’ve reached your game group loss limit for this month.")]
        YouReachedYourGameGroupLossLimitForThisMonth = -1524,
        [Description("You have exceeded max bet for bonus funds.")]
        YouHaveExceededMaxBetForBonusFunds = -1525
    }
}
