using System.ComponentModel;

namespace TGS.Shared.Game.Models.BetConstruct.Enums
{
    public enum GameApiError
    {        
        [Description("")]
        Success = 0,

        [Description("Internal Error")]
        InternalError = 1000,
        [Description("User is blocked")]
        UserIsBlocked = 1003,
        [Description("User is dismissed")]
        UserIsDismissed = 1004,
        [Description("Password/AuthToken error")]
        PasswordOrAuthTokenError = 1005,
        [Description("Logging in the page is not possible, since you have not activated your account  from your e-mail.")]
        LoggingNotPossible = 1008,
        [Description("Wrong login or E-mail")]
        WrongLogin = 1117,
        [Description("The sum exceeds maximum allowable limit")]
        SumExceedsMaxLimit = 1550,
        [Description("The sum is less than minimum allowable limit")]
        SumLessMinLimit = 1560,
        [Description("API wrong access exception")]
        ApiWrongAccessException = 1700,
        [Description("Client limit exception")]
        ClientLimitException = 2200,
        [Description("Office limit exception")]
        OfficeLimitException = 2300,
        [Description("Client balance is less")]
        ClientBalanceIsLess = 2400
    }
}
