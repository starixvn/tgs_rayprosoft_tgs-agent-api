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
        /// <summary>
        /// TEMPORARY_ERROR
        /// </summary>
        [Description("Failed adapter operation could be retried after getting such error")]
        TemporaryError = 1001,
        /// <summary>
        /// INVALID_TOKEN_ID
        /// </summary>
        [Description("if we send incorrect authToken")]
        InvalidTokenId = 10003,
        /// <summary>
        /// INVALID_SID
        /// </summary>
        [Description("if we send incorrect sid")]
        InvalidSid = 10004,
        /// <summary>
        /// ACCOUNT_LOCKED
        /// </summary>
        [Description("There has been a problem with the Live Casino. User authentication failed or your session may be expired, please close the browser and try again.")]
        AccountLocked = 10007,
        /// <summary>
        /// FATAL_ERROR_CLOSE_USER_SESSION
        /// </summary>
        [Description("There has been a problem with the Live Casino. User authentication failed or your session may be expired, please close the browser and try again.")]
        FatalErrorCloseUserSession = 3004,
        /// <summary>
        /// UNKNOWN_ERROR
        /// </summary>
        [Description("Please contact Customer Support for assistance.")]
        UnknownError = 1049,
        /// <summary>
        /// INVALID_PARAMETER
        /// </summary>
        [Description("if we send a fake user id")]
        InvalidParameter = 10002,
        /// <summary>
        /// BET_DOES_NOT_EXIST
        /// </summary>
        [Description("1.when transaction.refId in /credit request can't be mapped to an open debit transaction 2.when transaction.Id in / cancel request can't be mapped to an open debit transaction")]
        BetDoesNotExist = 10005,
        /// <summary>
        /// INSUFFICIENT_FUNDS
        /// </summary>
        [Description("if we ask to debit a player more than he has")]
        InsufficientFunds = 10008,
        /// <summary>
        /// FINAL_ERROR_ACTION_FAILED
        /// </summary>
        [Description("Requested action failed. Press OK to continue.")]
        FinalErrorActionFailed = 2001,
        /// <summary>
        /// GEOLOCATION_FAIL
        /// </summary>
        [Description("Operation not allowed because of geolocation check.")]
        GeolocationFail = 1100,
        /// <summary>
        /// BONUS_LIMIT_EXCEEDED
        /// </summary>
        [Description("Bet rejected. Your stake exceeds the maximum allowed with this bonus. Please lower your stake and try again.")]
        BonusLimitExceeded = 1051,
        /// <summary>
        /// CASINO_LIMIT_EXCEEDED_TURNOVER
        /// </summary>
        [Description("This bet amount would exceed your personal limit within the specified period of time. Meant for user to specify limit for amount that is wagered")]
        CasinoLimitExceededTurnover = 4004,
        /// <summary>
        /// CASINO_LIMIT_EXCEEDED_SESSION_TIME
        /// </summary>
        [Description("Your play time exceeds your personal limit of time that may be played in a session. Meant for user to specify limit for game session length (time frame)")]
        CasinoLimitExceededSessionTime = 4001,
        /// <summary>
        /// CASINO_LIMIT_EXCEEDED_STAKE
        /// </summary>
        [Description("This bet amount would exceed your personal single bet limit. Optional. Meant for user to specify limit for single bet")]
        CasinoLimitExceededStake = 4002,
        /// <summary>
        /// CASINO_LIMIT_EXCEEDED_LOSS
        /// </summary>
        [Description("This bet amount would exceed your personal limit within the specified period of time. Meant for user to specify limit for amount he lost (sum of all losses)")]
        CasinoLimitExceededLoss = 4003,
        [Description("SESSION_ALREADY_CLOSED")]
        SessionAlreadyClosed = 1,
        [Description("SESSION_DOES_NOT_EXIST")]
        SessionDoesNotExist = 2,
        /// <summary>
        /// OPERATION_IN_PROCESS
        /// </summary>
        [Description("Transaction is being process and no result is currently available.")]
        OperationInProcess = 2003,
        /// <summary>
        /// BET_ALREADY_EXIST
        /// </summary>
        [Description("when receive a repeat /debit request, judgeing by transaction.id")]
        BetAlreadyExist = 4,
        /// <summary>
        /// BET_ALREADY_SETTLED
        /// </summary>
        [Description("when receive a repeat /credit or /cancel request, , judgeing by transaction.id")]
        BetAlreadySettled = 5,
    }
}
