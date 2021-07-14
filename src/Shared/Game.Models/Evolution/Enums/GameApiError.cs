using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Enums
{
    public enum GameApiError
    {
        [Description("OK")]
        OK = 0,
        [Description("TEMPORARY_ERROR")]
        TemporaryError = 1001,
        [Description("INVALID_TOKEN_ID")]
        InvalidTokenId = 10003,
        [Description("INVALID_SID")]
        InvalidSid = 10004,
        [Description("ACCOUNT_LOCKED")]
        AccountLocked = 10007,
        [Description("FATAL_ERROR_CLOSE_USER_SESSION")]
        FatalErrorCloseUserSession = 3004,
        [Description("UNKNOWN_ERROR")]
        UnknownError = 1049,
        [Description("INVALID_PARAMETER")]
        InvalidParameter = 10002,
        [Description("BET_DOES_NOT_EXIST")]
        BetDoesNotExist = 10005,
        [Description("INSUFFICIENT_FUNDS")]
        InsufficientFunds = 10008,
        [Description("FINAL_ERROR_ACTION_FAILED")]
        FinalErrorActionFailed = 2001,
        [Description("GEOLOCATION_FAIL")]
        GeolocationFail = 1100,
        [Description("BONUS_LIMIT_EXCEEDED")]
        BonusLimitExceeded = 1051,
        [Description("CASINO_LIMIT_EXCEEDED_TURNOVER")]
        CasinoLimitExceededTurnover = 4004,
        [Description("CASINO_LIMIT_EXCEEDED_SESSION_TIME")]
        CasinoLimitExceededSessionTime = 4001,
        [Description("CASINO_LIMIT_EXCEEDED_STAKE")]
        CasinoLimitExceededStake = 4002,
        [Description("CASINO_LIMIT_EXCEEDED_LOSS")]
        CasinoLimitExceededLoss = 4003,
        [Description("SESSION_ALREADY_CLOSED")]
        SessionAlreadyClosed = 1,
        [Description("SESSION_DOES_NOT_EXIST")]
        SessionDoesNotExist = 2,
        [Description("OPERATION_IN_PROCESS")]
        OperationInProcess = 2003
    }
}
