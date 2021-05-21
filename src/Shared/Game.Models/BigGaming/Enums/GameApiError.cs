using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Enums
{
    public enum GameApiError
    {
        [Description("未知异常")]
        UnknownException = 000,
        [Description("网络错误")]
        NetworkError = 001,
        [Description("应用层未知异常")]
        UnknownExceptionAtTheApplicationLayer = 002,
        [Description("数据访问未知异常")]
        UnknownDataAccessException = 003,
        [Description("缓存访问未知异常")]
        UnknownExceptionToCacheAccess = 004,
        [Description("RPC调用未知异常")]
        RpcCallUnknownException = 005,

        [Description("传入的参数不对")]
        ParametersPassedInAreIncorrect = 100,

        [Description("缺少参数")]
        MissingParameters = 700,
        [Description("required缺少参数")]
        RequiredMissingParameters = 701,
        [Description("integer: 没有或者必须为整数 无参数")]
        IntegerNoneOrMustBeAnIntegerNoParameters = 702,
        [Description("integerRange:整数范围必须在参数0和参数1之间; param0和param1必须能被转化成整数")]
        IntegerrangeTheIntegerRangeMustBeBetweenParameter0AndParameter1 = 703,
        [Description("date: 必须为日期格式, param0必须为yyyy-mm-dd")]
        DateMustBeADateFormat = 704,
        [Description("enum: 必须是一系列枚举值中的一个(param0中用逗号分割出来的集合)")]
        EnumMustBeOneOfASeriesOfEnumeratedValues = 705,
        [Description("minLength: 参数最小长度不正确")]
        MinlengthTheMinimumLengthOfTheParameterIsIncorrect = 706,
        [Description("maxLength: 参数最大长度不正确")]
        MaxlengthTheMaximumLengthOfTheParameterIsIncorrect = 707,
        [Description("mask: 允许自定义正则表达式来进行校验, param0为表达式字符串")]
        MaskAllowsCustomRegularExpressionsToBeChecked = 708,
        [Description("double: 允许为空或者必须为double数 无参数")]
        DoubleItIsAllowedToBeEmptyOrMustBeADoubleNumber = 709,
        [Description("doubleRange:浮点范围必须在参数0和参数1之间; param0和param1必须能被转化成浮点数")]
        DoublerangeTheFloatingpointRangeMustBeBetweenParameter0AndParameter1 = 710,
        [Description("参数类型不匹配")]
        ParameterTypeDoesNotMatch = 711,
        [Description("变量值不相等")]
        VariableValuesAreNotEqual = 712,
        [Description("格式化数据异常")]
        FormatDataAbnormal = 713,
        [Description("对象序列化异常")]
        ObjectSerializationException = 717,
        [Description("对象反序列化异常")]
        ObjectDeserializationException = 718,
        [Description("未找到目标对象")]
        TargetNotFound = 719,
        [Description("编码解码数据异常")]
        ENCODINGANDDECODINGDATAISABNORMAL = 720,
        [Description("加解密数据异常")]
        EncryptionAndDecryptionDataIsAbnormal = 721,

        [Description("appId[platform] 未知异常")]
        AppidPlatformUnknownException = 2200,
        [Description("认证令牌(auth token)无效")]
        AuthenticationTokenIsInvalid = 2202,
        [Description("认证令牌(auth token)的签名(sign)无效")]
        AuthenticationSignIsInvalid = 2203,
        [Description("认证令牌(auth token)的加密种子(seed)无效")]
        AuthenticationSeedIsInvalid = 2204,
        [Description("无法注册用户")]
        UnableToRegisterUser = 2205,
        [Description("登录名(loginId)已存在, 注册用户失败")]
        LoginNameAlreadyExists = 2206,
        [Description("注册来源IP无效")]
        InvalidRegistrationSourceIP = 2207,
        [Description("创建用户鉴权失败, 注册用户失败")]
        FailedToCreateUserAuthentication = 2208,
        [Description("创建用户信息失败, 注册用户失败")]
        FailedToCreateUserInformation = 2209,
        [Description("appId参数不存在, 注册用户失败")]
        AppidParameterDoesNotExist = 2210,
        [Description("注册来源IP不正确, 注册用户失败")]
        RegistrationSourceIpIsIncorrect = 2211,
        [Description("您好, 您的账号不存在, 请重新确认, 谢谢.")]
        AccountDoesNotExist = 2212,
        [Description("当前用户ID不存在auth数据, 登录失败")]
        CurrentUserIdDoesNotHaveAuthData = 2213,
        [Description("当前用户已注销, 登录失败")]
        CurrentUserHasBeenLoggedOut = 2214,
        [Description("当前用户已被封停, 登录失败.请与客服联系.")]
        CurrentUserHasBeenBannedAndLoginFailed = 2215,
        [Description("当前用户未激活, 请您先激活后登录.")]
        CurrentUserIsNotActivated = 2216,
        [Description("帐号已被禁用, 请联系客服.")]
        AccountHasBeenDisabled = 2217,
        [Description(" 密码错误, 登录失败.")]
        PasswordIsWrong = 2218,
        [Description("登录来源IP无效")]
        InvalidLoginSourceIp = 2219,
        [Description("登录会话不存在")]
        LoginSessionDoesNotExist = 2220,
        [Description("登录会话已过期, 请重新登录")]
        LoginSessionExpired = 2221,
        [Description("登录会话子已失效, 请重新登录.")]
        LoginSessionHasExpired = 2222,
        [Description("当前用户ID不存在profile数据")]
        ProfileDataDoesNotExistForTheCurrentUserId = 2229,
        [Description("登录来源IP无效")]
        InvalidLoginSourceIp2230 = 2230,
        [Description("当前用户ID不存在auth数据")]
        CurrentUserIdDoesNotHaveAuthData2231 = 2231,
        [Description("创建用户会话异常")]
        AbnormalUserSessionCreation = 2263,
        [Description("会话已经被强制登出")]
        SessionHasBeenForciblyLoggedOut = 2264,
        [Description("厅主账号(sn)状态异常, 不能登录")]
        SNStatusOfTheHallIsAbnormalAndCannotLogIn = 2270,
        [Description("厅主账号(sn)记录不存在")]
        SNRecordDoesNotExist = 2271,
        [Description(" 厅主账号(sn)内部唯一标记数据异常")]
        SNHallIsAbnormal = 2272,
        [Description("新增代理失败")]
        FailedToAddAgent = 2276,
        [Description("批量资金流水入库异常")]
        AbnormalFlowOfBulkFundsIntoTheWarehouse = 2400,
        [Description("账户扣款不成功, 更新余额失败")]
        AccountDeductionWasUnsuccessful = 2402,
        [Description("账户充款不成功, 更新余额失败")]
        AccountRechargeIsUnsuccessful = 2403,
        [Description("批量账户入款列表为空")]
        BatchAccountDepositListIsEmpty = 2404,
        [Description("不存在此用户账户")]
        AccountDoesNotExist2405 = 2405,
        [Description("批量资金流水入库操作超时")]
        BulkCapitalFlowIntoTheWarehouseOperationTimedOut = 2406,
        [Description("批量资金流水入库操作中, 前后金额不一致")]
        BulkCapitalFlowAndStoragOperation = 2407,
        [Description("批量资金流水入库操作中, 提交记录数与操作成功记录数不一致")]
        BatchTransferOfFundsIntoTheDatabase = 2408,
        [Description("同一用户发生多个业务同时操作账户余额异常")]
        AccountBalanceIsAbnormal = 2412,
        [Description(" 帐号已被禁用, 请联系客服(验证余额加密摘要失败)")]
        AccountHasBeenDisabled2446 = 2446,
        [Description("处理用户提交批量下注异常")]
        HandleExceptionsSubmittedByUsersForBatchBetting = 2500,
        [Description("批量下注单入库异常")]
        BatchBettingOrderStorageException = 2501,
        [Description("处理用户提交批量结算异常")]
        ExceptionOfBatchSettlementSubmittedByUsers = 2502,
        [Description("非法下注单结算状态")]
        IllegalBetBillSettlementStatus = 2503,
        [Description("下注余额不足")]
        InsufficientBettingBalance = 2504,
        [Description("下注只能扣款, 不能充款")]
        BetsCanOnlyBeDeductedNotRecharged = 2505,
        [Description("下注中的现金流入库单数量与下注单入库单数量不一致异常")]
        BettingIsInconsistentWithTheNumberOfBettingSlips = 2506,
        [Description("结算只能充款, 不能扣款")]
        SettlementCanOnlyBeRecharged, NotDeducted = 2507,
        [Description("结算中的现金流入库数量不一致异常")]
        SettlementIsInconsistentAndAbnormal = 2508,
        [Description("结算中的结算单数量与入库数量不一致异常")]
        SettlementIsInconsistentWithTheNumberOfWarehoused = 2509,
        [Description("重新结算中验证操作令牌失败")]
        FailedToVerifyOperationTokenDuringReSettlement = 2510,
        [Description("  注单不存在(注单ID不存在)")]
        BetSlipDoesNotExist = 2511
    }
}
