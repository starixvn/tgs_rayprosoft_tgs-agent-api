namespace TGS.Core.Models.Enums
{
    public enum TgsApiError
    {
        None = 0,

        InvalidCredentials = 101,
        InvalidOrExpiredPlayerToken = 102,
        InvalidCaptcha = 103,
        ClientIdNotFound = 104,

        PlayerNotFound = 201,
        UserBlocked = 202,
        CountryRestricted = 203,
        CurrencyMismatch = 204,
        InsufficientFunds = 205,
        InvalidInvitationCode = 206,
        InsufficientBucket = 207,
        GameBalanceFundInFailed = 208,
        GameBalanceFundOutFailed = 209,

        InvalidGameAction = 301,
        TransactionIdNotFound = 302,
        DuplicateTransaction = 303,
        RoundIdNotFound = 304,
        InvalidAmount = 305,

        LoginFail = 400,
        InvalidArguments = 401,
        IncorrectFormat = 402,
        UnderMaintenance = 403,
        InvalidStatus = 404,
        ValidateCodeNotVerify = 405,
        EmailNotVerify = 406,
        PromotionCannotJoin = 407,
        FavoriteFull = 408,

        SystemError = 900,
        OperationNotAllowed = 901,
        ProviderApiError = 902
    }
}