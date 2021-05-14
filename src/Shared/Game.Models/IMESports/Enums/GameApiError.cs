using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameApiError
    {
        Success = 0,
        PlayerAlreadyExist = 503,
        PlayerDoesNotExist = 504,
        InvalidArguments = 505,
        InvalidPlayerId = 506,
        InvalidCurrency = 507,
        InsufficientAmount = 510,
        InvalidToken = 531,
        GameNotActive = 533,
        PlayerIsInactive = 542,
        TransactionIdNotFound = 545,
        GameNotExist = 577,
        ConcurrentRequestDetected = 579,
        SystemError = 998,
    }
}