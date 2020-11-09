using System.ComponentModel;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Enums
{
    public enum GameApiError
    {
        [Description("No Error")]
        Success = 0,

        [Description("Member not exist")]
        MemberNotExist = 1,

        [Description("Invalid Ip")]
        InvalidIp = 2,

        [Description("userName empty")]
        PlayerNotFound = 3,

        [Description("CompanyKey Error")]
        InvalidCredentials = 4,

        [Description("Not enough balance")]
        InsufficientFunds = 5,

        [Description("Bet not exists")]
        BetNotExists = 6,

        [Description("Internal Error")]
        SystemError = 7,

        [Description("Bet Already Settled")]
        BetAlreadySettled = 2001,

        [Description("Bet Already Canceled")]
        BetAlreadyCanceled = 2002,

        [Description("Bet Already Rollback")]
        BetAlreadyRollback = 2003,


        // custom errors

        UserBlocked = 2004,
        IncorrectFormat = 2005,
        InvalidArguments = 2006,
        InvalidAmount = 2007,
        InvalidGameAction = 2008,
        UnderMaintenance = 2009,
        PlayerNotLogin = 2010,
        [Description("Bet Already Exists")]
        BetAlreadyExists = 2011,
    }
}
