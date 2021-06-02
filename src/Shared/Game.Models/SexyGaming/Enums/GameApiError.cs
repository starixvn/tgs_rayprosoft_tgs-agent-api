using System.ComponentModel;

namespace TGS.Shared.Game.Models.SexyGaming.Enums
{
    public enum GameApiError
    {
        [Description("系统繁忙")]
        SystemBusy = 9998,
        [Description("失败")]
        Fail = 9999,
        [Description("成功")]
        Success = 0000,
        [Description("请输入所有数据")]
        PleaseInputAllData = 10,
        [Description("无效的使用者账号")]
        InvalidUserId = 1000,
        [Description("帐号已存在")]
        AccountExisted = 1001,
        [Description("帐号不存在")]
        AccountIsNotExists = 1002,
        [Description("无效的货币")]
        InvalidCurrency = 1004,
        [Description("语言不存在")]
        LanguageIsNotExists = 1005,
        [Description("PT设定为空")]
        PTSettingIsEmpty = 1006,
        [Description("PT设定与上线冲突")]
        InvalidPTSettingWithParent = 1007,
        [Description("无效的token")]
        InvalidToken = 1008,
        [Description("无效时区")]
        InvalidTimezone = 1009,
        [Description("无效的数量")]
        InvalidAmount = 1010,
        [Description("无效的交易代码")]
        InvalidTxcode = 1011,
        [Description("有待处理的转帐")]
        HasPendingTransfer = 1012,
        [Description("帐号已锁")]
        AccountIsLock = 1013,
        [Description("帐号暂停")]
        AccountIsSuspend = 1014,
        [Description("交易代码已被执行过")]
        TxcodeAlreadyOperation = 1016,
        [Description("交易代码不存在")]
        TxcodeIsNotExist = 1017,
        [Description("余额不足")]
        NotEnoughBalance = 1018,
        [Description("没有资料")]
        NoData = 1019,
        [Description("无效的日期（时间）格式")]
        InvalidDateTimeFormat = 1024,
        [Description("无效的交易状态")]
        InvalidTransactionStatus = 1025,
        [Description("无效的投注限制设定")]
        InvalidBetLimitSetting = 1026,
        [Description("无效的认证码")]
        InvalidCertificate = 1027,
        [Description("无法执行指定的行为，请稍后再试")]
        UnableToProceedPleaseTryAgainLater = 1028,
        [Description("无效的IP通常发生于您的IP尚未加白, 或是AWC辨试不到您的AgentID")]
        InvalidIpAddress = 1029,
        [Description("使用无效的装置呼叫(例如:IE)")]
        InvalidDeviceToCallApi = 1030,
        [Description("系统维护中")]
        SystemIsUnderMaintenance = 1031,
        [Description("重复登入")]
        DuplicateLogin = 1032,
        [Description("无效的游戏代码")]
        InvalidGamecode = 1033,
        [Description("您使用的时间参数不符合此API规定格式")]
        TimeDoesNotMeet = 1034,
        [Description("无效的Agent Id")]
        InvalidAgentId = 1035,
        [Description("无效的参数")]
        InvalidParameters = 1036,
        [Description("错误的客户设定通常可能发生于您的目标回调钱包位置尚未通过AWC测试，请联系我方进行协助")]
        InvalidCustomerSetting = 1037,
        [Description("重复的交易")]
        DuplicateTransaction = 1038,
        [Description("无此交易")]
        TransactionNotFound = 1039,
        [Description("请求逾时")]
        RequestTimeout = 1040,
        [Description("HTTP状态错误")]
        HttpStatusError = 1041,
        [Description("HTTP请求空白")]
        HttpResponseIsEmpty = 1042,
        [Description("下注已被取消")]
        BetHasCanceled = 1043,
        [Description("无效的下注")]
        InvalidBet = 1044,
        [Description("帐便记录新增失败")]
        AddAccountStatementFailed = 1045,
        [Description("转帐失败！ 请立即联系客户支持。 给您造成的任何不便，请原谅。")]
        TransferFaile = 1046,
        [Description("游戏维护中")]
        GameIsUnderMaintenance = 1047
    }
}
