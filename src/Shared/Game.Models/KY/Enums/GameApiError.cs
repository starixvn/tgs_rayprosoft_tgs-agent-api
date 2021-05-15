using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Enums
{
    public enum GameApiError
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("Success")]
        Success = 0,
        /// <summary>
        /// TOKEN 丢失（重新调⽤登录接⼝获取）
        /// </summary>
        [Description("TOKEN loss (reinvocation of the login interface)")]
        TokenLoss = 1,
        /// <summary>
        /// 渠道不存在（请检查渠道 ID 是否正确）
        /// </summary>
        [Description("The channel non-existent (check the channel ID is correct or not)")]
        ChannelNonExistent = 2,
        /// <summary>
        /// 验证时间超时（请检查 timestamp 是否正确）
        /// </summary>
        [Description("Authentication time timeout (check if timestamp is correct or not)")]
        AuthenticationTimeTimeout = 3,
        /// <summary>
        /// 验证错误
        /// </summary>
        [Description("Authentication error")]
        AuthenticationError = 4,
        /// <summary>
        /// 渠道⽩名单错误（请联系客服添加服务器⽩名单）
        /// </summary>
        [Description("Channel white list error (contact customer service add server white list)")]
        ChannelWhiteListError = 5,
        /// <summary>
        /// 验证字段丢失（请检查参数完整性）
        /// </summary>
        [Description("Authentication field loss (please check parameter integrity)")]
        AuthenticationFieldLoss = 6,
        /// <summary>
        /// TOKEN失敗（重新調用登錄界面）
        /// </summary>
        [Description("TOKEN failure (reinvocation of the login interface)")]
        TokenFailure = 7,
        /// <summary>
        /// 不存在的请求（请检查⼦操作类型是否正确）
        /// </summary>
        [Description("A non-existent request (check the suboperation type is correct or not)")]
        ANonExistentRequest = 8,
        /// <summary>
        /// 玩家同時處於不同的遊戲中。
        /// </summary>
        [Description("The player are in different games at the same time.")]
        PlayerInDifferentGames = 10,
        /// <summary>
        /// 玩家的帳戶不存在。
        /// </summary>
        [Description("The player’s account is non-existence.")]
        AccountNonExistence = 11,
        /// <summary>
        /// 玩家的賬戶是不存在的通道。
        /// </summary>
        [Description("The player’s account is non-existence in channel.")]
        AccountNonExistenceChannel = 12,
        /// <summary>
        /// 玩家登錄帳戶不匹配
        /// </summary>
        [Description("Player login account mismatch")]
        AccountMismatch = 13,
        /// <summary>
        /// 玩家正在玩遊戲。
        /// </summary>
        [Description("The player is playing in the game.")]
        PlayingInGame = 14,
        /// <summary>
        /// 渠道验证错误（1.MD5key 值是否正确；2.⽣成 key 值中的 timestamp 与
        /// 参数中的是否⼀致；3. ⽣成 key 值中的 timestamp 与代理编号以字符串形式拼接）
        /// </summary>
        [Description("Channel authentication error（1.MD5key is correct or not; 2. Does the timestamp.")]
        ChannelAuthenticationError = 15,
        /// <summary>
        /// 数据不存在（当前没有注单）
        /// </summary>
        [Description("Data non-existence(no bet list currently)")]
        DataNonExistence = 16,
        /// <summary>
        /// 遊戲返回碼不存在
        /// </summary>
        [Description("Game return code non-existence")]
        GameCodeNonExistence = 17,
        /// <summary>
        /// 提款金額與訂單金額不一致。
        /// </summary>
        [Description("The sum of withdraw money is not consistent with the amount of order.")]
        WithdrawMoneyNotConsistentOrder = 18,
        /// <summary>
        /// 遊戲中設置的金額與訂單金額不一致。
        /// </summary>
        [Description("The sum of money set in the game is not consistent with the amount of order.")]
        MoneyInGameNotConsistentOrder = 19,
        /// <summary>
        /// 账号禁⽤
        /// </summary>
        [Description("Account forbidden")]
        AccountForbidden = 20,
        /// <summary>
        /// 開戶
        /// </summary>
        [Description("Account kick-out")]
        AccountKickOut = 21,
        /// <summary>
        /// AES 解密失败
        /// </summary>
        [Description("AES decrypt failure")]
        AESDecryptFailure = 22,
        /// <summary>
        /// URL解碼失敗
        /// </summary>
        [Description("URL decoding failure")]
        URLDecodingFailure = 23,
        /// <summary>
        /// 渠道拉取数据超过时间范围
        /// </summary>
        [Description("Channel pulls data overtime")]
        ChannelPullsDataOvertime = 24,
        /// <summary>
        /// 遊戲不存在
        /// </summary>
        [Description("Game non-existence")]
        GameNonExistence = 25,
        /// <summary>
        /// 订单号不存在
        /// </summary>
        [Description("Order number non-existence")]
        OrderNumberNonExistence = 26,
        /// <summary>
        /// 数据库异常
        /// </summary>
        [Description("Database exception")]
        DatabaseException = 27,
        /// <summary>
        /// ip 禁⽤
        /// </summary>
        [Description("IP fobidden")]
        IPFobidden = 28,
        /// <summary>
        /// 订单号与订单规则不符
        /// </summary>
        [Description("The order number is not in conformity with the order rule")]
        OrderNumberNotConformity = 29,
        /// <summary>
        /// 获取玩家在线状态失败
        /// </summary>
        [Description("Gain access player’s online status failure")]
        OnlineStatusFailure = 30,
        /// <summary>
        /// 更新的分数⼩于或者等于 0
        /// </summary>
        [Description("The updated score is less than or equal to 0")]
        UpdatedScoreLess = 31,
        /// <summary>
        /// 更新玩家信息失败
        /// </summary>
        [Description("Update player information failure")]
        UpdatePlayerInformationFailure = 32,
        /// <summary>
        /// 更新玩家⾦币失败
        /// </summary>
        [Description("Update player’s gold coin failed")]
        UpdatePlayerGoldCoinFailed = 33,
        /// <summary>
        /// 订单重复
        /// </summary>
        [Description("Order duplication")]
        OrderDuplication = 34,
        /// <summary>
        /// 获取玩家信息失败（请调⽤登录接⼝创建账号）
        /// </summary>
        [Description("Gain access player’s infomation failure（Please call the login interface to create ")]
        AccessInfomationFailure = 35,
        /// <summary>
        /// KindID 不存在
        /// </summary>
        [Description("KindID inexistence")]
        KindIDInexistence = 36,
        /// <summary>
        /// 登录瞬间禁⽌下分，导致下分失败
        /// </summary>
        [Description("Login moment is forbid refunding, result in fail to refund")]
        LoginMomentIsForbid = 37,
        /// <summary>
        /// 余额不⾜导致下分失败
        /// </summary>
        [Description("Insufficient balance result in fail to refund")]
        InsufficientBalanceResultInFail = 38,
        /// <summary>
        /// 禁⽌同⼀账号登录带分、上分、下分并发请求，后⼀个请求被拒
        /// </summary>
        [Description("Forbid same account login to get points, charge points, refund and send")]
        ForbidSameAccountLogin = 39,
        /// <summary>
        /// 单次上下分数量不能超过⼀千万
        /// </summary>
        [Description("The amount of charge points and refund cannot more than 10 million at one ")]
        AmountCannotMoreThan10Million = 40,
        /// <summary>
        /// 拉取对局汇总统计时间范围有误
        /// </summary>
        [Description("Pull the data of the range of total time of the match have error")]
        PullTheDataOfTheRange = 41,
        /// <summary>
        /// 代理被禁⽤
        /// </summary>
        [Description("Agent has been forbid")]
        AgentHasBeenForbid = 42,
        /// <summary>
        /// 拉单过于频繁(两次拉单时间间隔，外测服必须⼤于 10 秒，正式服必须⼤于 5 秒)
        /// </summary>
        [Description("Pull the order too frequent（there must more than 1 second between 2 times of ")]
        PullTheOrderTooFrequent = 43,
        /// <summary>
        /// 订单正在处理中
        /// </summary>
        [Description("Have the same order being processed")]
        HaveTheSameOrderBeingProcessed = 44,
        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("Parameter error")]
        ParameterError = 45,
        /// <summary>
        /// 注册会员账号系统异常
        /// </summary>
        [Description("Registered membership account system exception")]
        RegisteredMembershipAccountSystemException = 1001,
        /// <summary>
        /// 代理商⾦额不⾜
        /// </summary>
        [Description("Insufficient balance of agent")]
        InsufficientBalanceOfAgent = 1002,
    }
}
