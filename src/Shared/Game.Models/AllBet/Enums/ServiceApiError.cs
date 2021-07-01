// ReSharper disable InconsistentNaming

namespace TGS.Shared.Game.Models.AllBet.Enums
{
    public enum ServiceApiError
    {
        OK,
        INTERNAL_ERROR,
        INVALID_OPERATORID,
        INVALID_SIGN,
        FORBIDDEN,
        ILLEGAL_ARGUMENT,
        SYSTEM_MAINTENANCE,
        AGENT_NOT_EXIST,
        PLAYER_EXIST,
        PLAYER_NOT_EXIST,
        TOO_FREQUENT_REQUEST
    }
}