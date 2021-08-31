using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Enums
{
    public enum GameApiError
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("Success")]
        Success = 0,
        /// <summary>
        /// 未知的系统错误，请联系TCG客服
        /// </summary>
        [Description("Unknown system error, please contact TCG customer support")]
        UnknownSystemErrorPleaseContactTCGCustomerSupport = 1,
        /// <summary>
        /// 缺少必需的参数
        /// </summary>
        [Description("Missing required parameter")]
        MissingRequiredParameter = 2,
        /// <summary>
        /// 此产品类型不支持此方法
        /// </summary>
        [Description("Method not supported for the this product type")]
        MethodNotSupportedForTheThisProductType = 3,
        /// <summary>
        /// 商家不允许使用此产品类型
        /// </summary>
        [Description("Merchant is not allowed for this product type")]
        MerchantIsNotAllowedForThisProductType = 4,
        /// <summary>
        /// 找不到商家
        /// </summary>
        [Description("Merchant not found")]
        MerchantNotFound = 5,
        /// <summary>
        /// 参数无效，无法解密参数
        /// </summary>
        [Description("Invalid parameters, Failed to decrypt the parameters")]
        InvalidParametersFailedToDecryptTheParameters = 6,
        /// <summary>
        /// 签名无效
        /// </summary>
        [Description("Invalid signature")]
        InvalidSignature = 7,
        /// <summary>
        /// 不支持的货币
        /// </summary>
        [Description("Unsupported currency")]
        UnsupportedCurrency = 8,
        /// <summary>
        /// 帐户类型无效
        /// </summary>
        [Description("Invalid Account type")]
        InvalidAccountType = 9,
        /// <summary>
        /// 产品类型无效
        /// </summary>
        [Description("Invalid product type")]
        InvalidProductType = 10,
        /// <summary>
        /// 提现余额不足
        /// </summary>
        [Description("Insufficient balance to fund out / withdraw")]
        InsufficientBalanceToFundOutWithdraw = 11,
        /// <summary>
        /// 交易序号已經存在
        /// </summary>
        [Description("Transaction already exists")]
        TransactionAlreadyExists = 12,
        /// <summary>
        /// 游戏代码无效
        /// </summary>
        [Description("Invalid game code")]
        InvalidGameCode = 13,
        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("User Does Not Exists")]
        UserDoesNotExists = 15,
        /// <summary>
        /// 信用额度不足
        /// </summary>
        [Description("Insufficient merchant credit to fund in")]
        InsufficientMerchantCreditToFundIn = 16,
        /// <summary>
        /// 此游戏代码不支持试用模式
        /// </summary>
        [Description("Trial mode is not supported for this game code")]
        TrialModeIsNotSupportedForThisGameCode = 18,
        /// <summary>
        /// 批次理未准备好
        /// </summary>
        [Description("Batch not ready")]
        BatchNotReady = 19,
        /// <summary>
        /// 找不到方法
        /// </summary>
        [Description("Method not found")]
        MethodNotFound = 21,
        /// <summary>
        /// 参数验证失败
        /// </summary>
        [Description("Parameter Validation Failed")]
        ParameterValidationFailed = 22,
        /// <summary>
        /// API 繁忙
        /// </summary>
        [Description("API is busy")]
        APIIsBusy = 23,
        /// <summary>
        /// 未找到此交易
        /// </summary>
        [Description("Transaction not found")]
        TransactionNotFound = 24,
        /// <summary>
        /// 預約已經完成
        /// </summary>
        [Description("Reservation already process")]
        ReservationAlreadyProcess = 25,
        /// <summary>
        /// Decimal 格式位支持
        /// </summary>
        [Description("Decimal not supported")]
        DecimalNotSupported = 26,
        /// <summary>
        /// 接口正在维护中
        /// </summary>
        [Description("API is under maintenance")]
        APIIsUnderMaintenance = 27,
        /// <summary>
        /// 此商户禁止使用此接口
        /// </summary>
        [Description("API is prohibited for this merchant")]
        APIIsProhibitedForThisMerchant = 28,
        /// <summary>
        /// 产品正在维护中
        /// </summary>
        [Description("Product is under maintenance")]
        ProductIsUnderMaintenance = 30
    }
}
