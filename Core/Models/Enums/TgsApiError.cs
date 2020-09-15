namespace TGS.Core.Models.Enums
{
    public enum TgsApiError
    {
        None = 0,
        InvalidOrExpiredLicenseeToken = 12,
        InvalidOrExpiredResellerToken = 16,
        UserBlocked = 20,
        CountryRestricted = 21,
        InvalidCredentials = 30,
        CurrencyMismatch = 50,
        InvalidAmount = 60,
        InsufficientFunds = 100,
        InvalidSystemCode = 102,
        ClientIdNotFound = 103,
        InvalidSignature = 104,
        TransactionIdNotFound = 200,
        DuplicateTransaction = 210,
        InvalidGameAction = 250,
        InvalidArguments = 300,
        IncorrectFormat = 500,
        PlayerNotFound = 600,
        UnderMaintenance = 700,
        ConfiguredTimeoutExceeded = 800,
        ConfiguredGameProviderTimeoutExceeded = 801,
        SystemError = 900,
        ConnectionError = 901,
        OperationNotAllowed = 902,
        TestPlayerStakeAmountCapExceeded = 904,
        BrowserIncompatibility = 905,
        GameNotSupportTestPlayer = 906,
    }
}