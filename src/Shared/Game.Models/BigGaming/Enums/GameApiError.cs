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
        Unknown_Exception = 000,
        [Description("网络错误")]
        Network_Error = 001,
        [Description("应用层未知异常")]
        Unknown_Exception_At_The_Application_Layer = 002,
        [Description("数据访问未知异常")]
        Unknown_Data_Access_Exception = 003,
        [Description("缓存访问未知异常")]
        Unknown_Exception_To_Cache_Access = 004,
        [Description("RPC调用未知异常")]
        Rpc_Call_Unknown_Exception = 005,

        [Description("传入的参数不对")]
        Parameters_Passed_In_Are_Incorrect = 100,

        [Description("缺少参数")]
        Missing_Parameters = 700,
        [Description("required缺少参数")]
        Required_Missing_Parameters = 701,
        [Description("integer: 没有或者必须为整数 无参数")]
        Integer_None_Or_Must_Be_An_Integer_No_Parameters = 702,
        [Description("integerRange:整数范围必须在参数0和参数1之间; param0和param1必须能被转化成整数")]
        Integerrange_The_Integer_Range_Must_Be_Between_Parameter_0_And_Parameter_1 = 703,
        [Description("date: 必须为日期格式, param0必须为yyyy-mm-dd")]
        Date_Must_Be_A_Date_Format = 704,
        [Description("enum: 必须是一系列枚举值中的一个(param0中用逗号分割出来的集合)")]
        Enum_Must_Be_One_Of_A_Series_Of_Enumerated_Values = 705,
        [Description("minLength: 参数最小长度不正确")]
        Minlength_The_Minimum_Length_Of_The_Parameter_Is_Incorrect = 706,
        [Description("maxLength: 参数最大长度不正确")]
        Maxlength_The_Maximum_Length_Of_The_Parameter_Is_Incorrect = 707,
        [Description("mask: 允许自定义正则表达式来进行校验, param0为表达式字符串")]
        Mask_Allows_Custom_Regular_Expressions_To_Be_Checked = 708,
        [Description("double: 允许为空或者必须为double数 无参数")]
        Double_It_Is_Allowed_To_Be_Empty_Or_Must_Be_A_Double_Number = 709,
        [Description("doubleRange:浮点范围必须在参数0和参数1之间; param0和param1必须能被转化成浮点数")]
        Doublerange_The_Floating_point_Range_Must_Be_Between_Parameter_0_And_Parameter_1 = 710,
        [Description("参数类型不匹配")]
        Parameter_Type_Does_Not_Match = 711,
        [Description("变量值不相等")]
        Variable_Values_Are_Not_Equal = 712,
        [Description("格式化数据异常")]
        Format_Data_Abnormal = 713,
        [Description("对象序列化异常")]
        Object_Serialization_Exception = 717,
        [Description("对象反序列化异常")]
        Object_Deserialization_Exception = 718,
        [Description("未找到目标对象")]
        Target_Not_Found = 719,
        [Description("编码解码数据异常")]
        ENCODING_AND_DECODING_DATA_IS_ABNORMAL = 720,
        [Description("加解密数据异常")]
        Encryption_And_Decryption_Data_Is_Abnormal = 721,

        [Description("appId[platform] 未知异常")]
        Appid_Platform_Unknown_Exception = 2200,
        [Description("认证令牌(auth token)无效")]
        Authentication_Token_Is_Invalid = 2202,
        [Description("认证令牌(auth token)的签名(sign)无效")]
        Authentication_Sign_Is_Invalid = 2203,
        [Description("认证令牌(auth token)的加密种子(seed)无效")]
        Authentication_Seed_Is_Invalid = 2204,
        [Description("无法注册用户")]
        Unable_To_Register_User = 2205,
        [Description("登录名(loginId)已存在, 注册用户失败")]
        Login_Name_Already_Exists = 2206,
        [Description("注册来源IP无效")]
        Invalid_Registration_Source_IP = 2207,
        [Description("创建用户鉴权失败, 注册用户失败")]
        Failed_To_Create_User_Authentication = 2208,
        [Description("创建用户信息失败, 注册用户失败")]
        Failed_To_Create_User_Information = 2209,
        [Description("appId参数不存在, 注册用户失败")]
        Appid_Parameter_Does_Not_Exist = 2210,
        [Description("注册来源IP不正确, 注册用户失败")]
        Registration_Source_Ip_Is_Incorrect = 2211,
        [Description("您好, 您的账号不存在, 请重新确认, 谢谢.")]
        Account_Does_Not_Exist = 2212,
        [Description("当前用户ID不存在auth数据, 登录失败")]
        Current_User_Id_Does_Not_Have_Auth_Data = 2213,
        [Description("当前用户已注销, 登录失败")]
        Current_User_Has_Been_Logged_Out = 2214,
        [Description("当前用户已被封停, 登录失败.请与客服联系.")]
        Current_User_Has_Been_Banned_And_Login_Failed = 2215,
        [Description("当前用户未激活, 请您先激活后登录.")]
        Current_User_Is_Not_Activated = 2216,
        [Description("帐号已被禁用, 请联系客服.")]
        Account_Has_Been_Disabled = 2217,
        [Description(" 密码错误, 登录失败.")]
        Password_Is_Wrong = 2218,
        [Description("登录来源IP无效")]
        Invalid_Login_Source_Ip = 2219,
        [Description("登录会话不存在")]
        Login_Session_Does_Not_Exist = 2220,
        [Description("登录会话已过期, 请重新登录")]
        Login_Session_Expired = 2221,
        [Description("登录会话子已失效, 请重新登录.")]
        Login_Session_Has_Expired = 2222,
        [Description("当前用户ID不存在profile数据")]
        Profile_Data_Does_Not_Exist_For_The_Current_User_Id = 2229,
        [Description("登录来源IP无效")]
        Invalid_Login_Source_Ip_2230 = 2230,
        [Description("当前用户ID不存在auth数据")]
        Current_User_Id_Does_Not_Have_Auth_Data_2231 = 2231,
        [Description("创建用户会话异常")]
        Abnormal_User_Session_Creation = 2263,
        [Description("会话已经被强制登出")]
        Session_Has_Been_Forcibly_Logged_Out = 2264,
        [Description("厅主账号(sn)状态异常, 不能登录")]
        SN_Status_Of_The_Hall_Is_Abnormal_And_Cannot_Log_In = 2270,
        [Description("厅主账号(sn)记录不存在")]
        SN_Record_Does_Not_Exist = 2271,
        [Description(" 厅主账号(sn)内部唯一标记数据异常")]
        SN_Hall_Is_Abnormal = 2272,
        [Description("新增代理失败")]
        Failed_To_Add_Agent = 2276,
        [Description("批量资金流水入库异常")]
        Abnormal_Flow_Of_Bulk_Funds_Into_The_Warehouse = 2400,
        [Description("账户扣款不成功, 更新余额失败")]
        Account_Deduction_Was_Unsuccessful = 2402,
        [Description("账户充款不成功, 更新余额失败")]
        Account_Recharge_Is_Unsuccessful = 2403,
        [Description("批量账户入款列表为空")]
        Batch_Account_Deposit_List_Is_Empty = 2404,
        [Description("不存在此用户账户")]
        Account_Does_Not_Exist_2405 = 2405,
        [Description("批量资金流水入库操作超时")]
        Bulk_Capital_Flow_Into_The_Warehouse_Operation_Timed_Out = 2406,
        [Description("批量资金流水入库操作中, 前后金额不一致")]
        Bulk_Capital_Flow_And_Storag_Operation = 2407,
        [Description("批量资金流水入库操作中, 提交记录数与操作成功记录数不一致")]
        Batch_Transfer_Of_Funds_Into_The_Database = 2408,
        [Description("同一用户发生多个业务同时操作账户余额异常")]
        Account_Balance_Is_Abnormal = 2412,
        [Description(" 帐号已被禁用, 请联系客服(验证余额加密摘要失败)")]
        Account_Has_Been_Disabled_2446 = 2446,
        [Description("处理用户提交批量下注异常")]
        Handle_Exceptions_Submitted_By_Users_For_Batch_Betting = 2500,
        [Description("批量下注单入库异常")]
        Batch_Betting_Order_Storage_Exception = 2501,
        [Description("处理用户提交批量结算异常")]
        Exception_Of_Batch_Settlement_Submitted_By_Users = 2502,
        [Description("非法下注单结算状态")]
        Illegal_Bet_Bill_Settlement_Status = 2503,
        [Description("下注余额不足")]
        Insufficient_Betting_Balance = 2504,
        [Description("下注只能扣款, 不能充款")]
        Bets_Can_Only_Be_Deducted_Not_Recharged = 2505,
        [Description("下注中的现金流入库单数量与下注单入库单数量不一致异常")]
        Betting_Is_Inconsistent_With_The_Number_Of_Betting_Slips = 2506,
        [Description("结算只能充款, 不能扣款")]
        Settlement_Can_Only_Be_Recharged, _Not_Deducted = 2507,
        [Description("结算中的现金流入库数量不一致异常")]
        Settlement_Is_Inconsistent_And_Abnormal = 2508,
        [Description("结算中的结算单数量与入库数量不一致异常")]
        Settlement_Is_Inconsistent_With_The_Number_Of_Warehoused = 2509,
        [Description("重新结算中验证操作令牌失败")]
        Failed_To_Verify_Operation_Token_During_Re_Settlement = 2510,
        [Description("  注单不存在(注单ID不存在)")]
        Bet_Slip_Does_Not_Exist = 2511
    }
}
