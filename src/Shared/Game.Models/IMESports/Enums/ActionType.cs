namespace TGS.Shared.Game.Models.IMESports.Enums
{
    public enum ActionType : short
    {
        PlaceBet = 1003,
        PlaceBetRefund = 1005,

        SettleFT = 4002,
        SettleParlay = 4004,
        
        UnsettleFT = 5002,
        UnsettleParlay = 5004,

        CancelFT = 6002,
        CancelSingleWager = 6011,
        CancelParlay = 6014,

        UncancelFT = 7002,
        UncancelSingleWager = 7011,
        UncancelParlay = 7014
    }
}
