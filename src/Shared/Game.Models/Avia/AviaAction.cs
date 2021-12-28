namespace TGS.Shared.Game.Models.Avia
{
    public enum AviaAction
    {
        /// <summary>
        /// 會員登錄
        /// </summary>
        Login,

        /// <summary>
        /// 查詢餘額
        /// </summary>
        Balance,

        /// <summary>
        /// 轉賬(主錢包 -> Gp)
        /// </summary>
        TransferOut,

        /// <summary>
        /// 轉賬(GP -> 主錢包)
        /// </summary>
        TransferIn,

        /// <summary>
        /// 轉賬查詢
        /// </summary>
        Transferinfo,

        /// <summary>
        /// 會員下線
        /// </summary>
        Logout,

        /// <summary>
        /// The game record
        /// </summary>
        GameRecord,

        /// <summary>
        /// 會員註冊
        /// </summary>
        Register,
    }
}
