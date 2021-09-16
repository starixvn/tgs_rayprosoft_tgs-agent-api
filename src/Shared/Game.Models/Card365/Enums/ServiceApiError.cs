namespace TGS.Shared.Game.Models.Card365.Enums
{
    public enum ServiceApiError
    {
        Fail = 0,
        Success = 1,
        NotAuthorized = 1001,
        InvalidAmount = 1002,
        NoData = 1003,
        InvalidUsername = 1004,
        UpdateFailed = 1005,
        InsufficientParameters = 1006,
        NotSupportedApi = 1007,
        InvalidParameters = 1008,
        InsufficientFunds = 1009,
        GameIdNotExist = 1011,
        AgentNotExist = 1012,
        QueryTooFast = 1014,
        InvalidUrl = 1015,
        InvalidAgent = 1016,
        AccountBlocked = 1018,
        InvalidWebsite = 1023,
        InvalidCurrencyCode = 1024,
        UnderMaintenance = 1028,
        AgentIsNotBelongToWebsite = 1030
    }
}