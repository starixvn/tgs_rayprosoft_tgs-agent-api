using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Enums
{
    public enum TCGTransactionType
    {
        /// <summary>
        /// 第三方游戏中奖
        /// </summary>
        ThirdParty = 3100,
        /// <summary>
        /// 第三方奖池游戏中奖
        /// </summary>
        ThirdPartyJackpotWin = 3110,
        /// <summary>
        /// 第三方游戏取消投注
        /// </summary>
        ThirdPartyCancelBook = 3111,
        /// <summary>
        /// 中獎/加錢
        /// </summary>
        Credit = 3150,
        /// <summary>
        /// 取消下注
        /// </summary>
        CancelBet = 3151,
        /// <summary>
        /// 第三方游戏投注
        /// </summary>
        ThirdPartyBetting = 3200,
        /// <summary>
        /// 投注/扣錢
        /// </summary>
        Debit = 3250,
        /// <summary>
        /// 彩票游戏中奖
        /// </summary>
        Win = 4100,
        /// <summary>
        /// 彩票游戏彩金
        /// </summary>
        Promotion = 4104,
        /// <summary>
        /// 彩票投注(消费)反点
        /// </summary>
        Rebate = 4105,
        /// <summary>
        /// 追中撤单
        /// </summary>
        HitCancelBet = 4106,
        /// <summary>
        /// 出号撤单
        /// </summary>
        ShownCancelBet = 4107,
        /// <summary>
        /// 空开撤单
        /// </summary>
        EmptyCancelBet = 4108,
        /// <summary>
        /// 系統撤单
        /// </summary>
        SystemCancelBet = 4109,
        /// <summary>
        /// 超級撤单
        /// </summary>
        SuperCancelBet = 4110,
        /// <summary>
        /// 个人撤单
        /// </summary>
        UserCancelBet = 4111,
        /// <summary>
        /// 和局退款
        /// </summary>
        TieRefund = 4112,
        /// <summary>
        /// 彩票游戏投注
        /// </summary>
        Bet = 4200,
        /// <summary>
        /// 彩票超额中奖
        /// </summary>
        ExcessWinRemoval = 4202,
        /// <summary>
        /// 投注确认
        /// </summary>
        BetConfirm = 4203,
        /// <summary>
        /// 彩票投注(消费)返点 退回
        /// </summary>
        CancelRebate = 4205,
        /// <summary>
        /// 追号单扣费
        /// </summary>
        ChaseBetTd = 4206,
        /// <summary>
        /// 回退獎金
        /// </summary>
        CancelWin = 4207,
        /// <summary>
        /// 撒单手续费
        /// </summary>
        CancelOrderCharge = 4208,
        /// <summary>
        /// Elott 中奖
        /// </summary>
        ElottWinning = 4120,
        /// <summary>
        /// Elott 个人撤单
        /// </summary>
        ElottUserCancelOrder = 4121,
        /// <summary>
        /// Elott 个人撤单
        /// </summary>
        ElottTie = 4122,
        /// <summary>
        /// Elott 追中撤单
        /// </summary>
        ElottHitDrawBack = 4123,
        /// <summary>
        /// Elott 出號撤單
        /// </summary>
        ElottShownDrawBack = 4124,
        /// <summary>
        /// Elott 空開撤單
        /// </summary>
        ElottEmptyDrawBack = 4125,
        /// <summary>
        /// Elott 系統撤單
        /// </summary>
        ElottSystemDrawBack = 4126,
        /// <summary>
        /// Elott 和局
        /// </summary>
        ElottSuperDrawBack = 4127,
        /// <summary>
        /// Elott 彩票投注
        /// </summary>
        ELottBetting = 4220,
        /// <summary>
        /// Elott 彩票投注確認
        /// </summary>
        ElottConfirmBetting = 4223,
        /// <summary>
        /// Elott 彩票超額中獎
        /// </summary>
        ElottExcessWinningRemoval = 4224,
        /// <summary>
        /// Elott 追號單扣費
        /// </summary>
        ElottChaseBet = 4225,
        /// <summary>
        /// Elott 回退獎金
        /// </summary>
        ElottWinAmountCancel = 4226,
        /// <summary>
        /// SEA东南亚彩票中奖
        /// </summary>
        SEAWinning = 4140,
        /// <summary>
        /// SEA东南亚彩票个人撤单
        /// </summary>
        SEAUserDrawBack = 4141,
        /// <summary>
        /// SEA东南亚彩票和局
        /// </summary>
        SEATie = 4142,
        /// <summary>
        /// SEA东南亚彩票追中撤单
        /// </summary>
        SEAHitDrawBack = 4143,
        /// <summary>
        /// SEA东南亚彩票出號撤單
        /// </summary>
        SEAWinAmountCancel = 4144,
        /// <summary>
        /// SEA东南亚彩票空開撤單
        /// </summary>
        SEASystemDrawBack = 4145,
        /// <summary>
        /// SEA东南亚彩票系統撤單
        /// </summary>
        SEASystemDrawBack2 = 4146,
        /// <summary>
        /// SEA东南亚彩票超級撤單
        /// </summary>
        SEASuperDrawBack = 4147,
        /// <summary>
        /// SEA东南亚彩票投注失敗退款
        /// </summary>
        SEABettingFailedRefund = 4149,
        /// <summary>
        /// VN越南彩票中獎
        /// </summary>
        VNWinning = 4150,
        /// <summary>
        /// VN越南彩个人撤单
        /// </summary>
        VNUserDrawBack = 4151,
        /// <summary>
        /// VN越南彩和局
        /// </summary>
        VNTie = 4152,

        /// <summary>
        /// VN越南彩追中撤单
        /// </summary>
        VNHitDrawBack = 4153,
        /// <summary>
        /// VN越南彩出號撤單
        /// </summary>
        VNWinAmtCancel = 4154,
        /// <summary>
        /// VN越南彩空開撤單
        /// </summary>
        VNEmptyDrawBack = 4155,
        /// <summary>
        /// VN越南彩系統撤單
        /// </summary>
        VNSystemDrawBack = 4156,
        /// <summary>
        /// VN越南彩超級撤單
        /// </summary>
        VNSuperDrawBack = 4157,
        /// <summary>
        /// VN越南彩盤口彩票中獎
        /// </summary>
        VNElottWinning = 4160,
        /// <summary>
        /// VN越南彩盤口个人撤单
        /// </summary>
        VNElottUserDrawBack = 4161,
        /// <summary>
        /// VN越南彩盤口和局
        /// </summary>
        VNElottTie = 4162,
        /// <summary>
        /// VN越南彩盤口追中撤单
        /// </summary>
        VNElottHitDrawBack = 4163,
        /// <summary>
        /// VN越南彩盤口出號撤單
        /// </summary>
        VNElottWinAmtCancel = 4164,
        /// <summary>
        /// VN越南彩盤口空開撤單
        /// </summary>
        VNElottEmptyDrawBack = 4165,
        /// <summary>
        /// VN越南彩盤口系統撤單
        /// </summary>
        VNElottSystemDrawBack = 4166,
        /// <summary>
        /// VN越南彩盤口超級撤單
        /// </summary>
        VNElottSuperDrawBack = 4167,
        /// <summary>
        /// SEA东南亚彩票投注
        /// </summary>
        SEABetting = 4240,
        /// <summary>
        /// SEA东南亚彩票彩票投注確認
        /// </summary>
        SEAConfirmBetting = 4233,
        /// <summary>
        /// SEA东南亚彩票超额中奖
        /// </summary>
        SEAExcessWinningRemoval = 4234,
        /// <summary>
        /// SEA东南亚彩票追號單扣費
        /// </summary>
        SEAChaseBet = 4235,
        /// <summary>
        /// SEA东南亚彩票回退獎金
        /// </summary>
        SEAWinAmountCancel2 = 4236,
        /// <summary>
        /// 越南彩票彩票投注
        /// </summary>
        VNBetting = 4250,
        /// <summary>
        /// 越南彩票彩票投注確認
        /// </summary>
        VNConfirmBetting = 4253,
        /// <summary>
        /// 越南彩票超額中獎
        /// </summary>
        VNExcessWinningRemoval = 4254,
        /// <summary>
        /// 越南彩票追號單扣費
        /// </summary>
        VNChaseBet = 4255,
        /// <summary>
        /// 越南彩票回退獎金
        /// </summary>
        VNWinAmountCancel = 4256,
        /// <summary>
        /// 越南盘口彩票投注
        /// </summary>
        VNElottBetting = 4260,
        /// <summary>
        /// 越南盤口彩票投注確認
        /// </summary>
        VNElottConfirmBetting = 4263,
        /// <summary>
        /// 越南盤口彩票超額中獎
        /// </summary>
        VNElottExcessWinningRemoval = 4264,
        /// <summary>
        /// 越南盤口追號單扣費
        /// </summary>
        VNElottChaseBet = 4265,
        /// <summary>
        /// 越南盤口回退獎金
        /// </summary>
        VNElottWinAmtCancel2 = 4266
    }
}
