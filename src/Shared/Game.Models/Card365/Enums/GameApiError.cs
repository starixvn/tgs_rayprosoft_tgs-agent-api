namespace TGS.Shared.Game.Models.Card365.Enums
{
    public enum GameApiError
    {
        Success = 200,
        NotAuthorized = 1001,
        InvalidAmount = 1002,
        NoData = 1003,
        InvalidUsername = 1004,
        InvalidParameters = 1008,
        InsufficientFunds = 1009,
        GameIdNotExist = 1011,
        InvalidAgent = 1016,
        AccountBlocked = 1018,
        InvalidCurrencyCode = 1024,
        UnderMaintenance = 1028,
        SystemError = 9999
    }
}