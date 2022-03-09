using System.ComponentModel;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Enums
{
    public enum GameApiError
    {
        [Description("操作成功")]
        OperationSucceeded = 0,

        [Description("参数错误")]
        ParameterError = 1,

        [Description("Token验证失败")]
        TokenVerificationFailed = 2,

        [Description("Command 未找到")]
        CommandNotFound = 3,

        [Description("非法操作")]
        IllegalOperation = 4,

        [Description("日期格式错误")]
        WrongDateFormat = 10,

        [Description("数据格式错误")]
        DataFormatError = 11,

        [Description("没有权限")]
        NoPermission = 97,

        [Description("操作失败")]
        OperationFailed = 98,

        [Description("未知错误")]
        UnknownError = 99,

        [Description("账号被锁定")]
        AccountsAreLocked = 100,

        [Description("账号格式错误")]
        AccountFormatError = 101,

        [Description("账号不存在")]
        AccountDoesNotExist = 102,

        [Description("此账号被占用")]
        ThisAccountIsOccupied = 103,

        [Description("密码格式错误")]
        PasswordFormatError = 104,

        [Description("密码错误")]
        BadPassword = 105,

        [Description("新旧密码相同")]
        OldAndNewPasswordsAreTheSame = 106,

        [Description("会员账号不可用")]
        MemberAccountUnavailable = 107,

        [Description("登入失败")]
        LoginFailed = 108,

        [Description("注册失败")]
        RegistrationFailed = 109,

        [Description("此账号已登入")]
        AccountIsAlreadyLoggedIn = 110,

        [Description("此账号已登出")]
        AccountIsLoggedOut = 111,

        [Description("此账号未登入")]
        AccountIsNotLoggedIn = 112,

        [Description("传入的代理账号不是代理")]
        IncomingProxyAccountIsNotAProxy = 113,

        [Description("找不到会员")]
        MemberNotFound = 114,

        [Description("账号已占用")]
        AccountIsOccupied = 116,

        [Description("找不到会员所属的分公司")]
        CouldNotFindTheBranchToWhichTheMemberBelongs = 117,

        [Description("找不到指定的代理")]
        SpecifiedProxyCouldNotBeFound = 118,

        [Description("余额不足")]
        InsufficientBalance = 120,

        [Description("免费试玩账号用完")]
        FreeTrialAccountExhausted = 150,

        [Description("系统维护")]
        SystemMaintenance = 300,

        [Description("API Key 错误")]
        ApiKeyError = 320,

        [Description("找不到相应的限红组")]
        CorrespondingRedLimitGroupCouldNotBeFound = 321,

        [Description("找不到指定的货币类型")]
        SpecifiedCurrencyTypeCouldNotBeFound = 322,

        [Description("转账流水号占用")]
        TransferSerialNumberOccupied = 323,

        [Description("转账失败")]
        TransferFailed = 324,

        [Description("代理状态不可用")]
        ProxyStatusUnavailable = 325,

        [Description("会员代理没有视频组")]
        MemberAgentsHaveNoVideoGroups = 326,

        [Description("客户端IP 受限")]
        ClientIpRestricted = 400,

        [Description("网络延迟")]
        NetworkDelay = 401,

        [Description("连接关闭")]
        ConnectionClosed = 402,

        [Description("客户端来源受限")]
        ClientSourceRestricted = 403,

        [Description("请求的资源不存在")]
        RequestedResourceDoesNotExist = 404,

        [Description("请求太频繁")]
        TooFrequentRequests = 405,

        [Description("请求超时")]
        RequestTimedOut = 406,

        [Description("找不到游戏地址")]
        GameAddressNotFound = 407,

        [Description("空指针异常")]
        NullPointerException = 500,

        [Description("系统异常")]
        SystemAbnormal = 501,

        [Description("系统忙")]
        SystemBusy = 502,

        [Description("数据操作异常")]
        AbnormalDataOperation = 503,
    }
}
