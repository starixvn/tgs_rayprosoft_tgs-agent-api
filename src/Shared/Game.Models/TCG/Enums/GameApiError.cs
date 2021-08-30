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
        [Description("Success(成功)")]
        Success = 0,
        [Description("Unknown system error, please contact TCG customer support(未知的系统错误，请联系TCG客服)")]
        UnknownSystemErrorPleaseContactTCGCustomerSupport = 1,
        [Description("Missing required parameter(缺少必需的参数)")]
        MissingRequiredParameter = 2,
        [Description("Method not supported for the this product type (此产品类型不支持此方法)")]
        MethodNotSupportedForTheThisProductType = 3,
        [Description("Merchant is not allowed for this product type (商家不允许使用此产品类型)")]
        MerchantIsNotAllowedForThisProductType = 4,
        [Description("Merchant not found (找不到商家)")]
        MerchantNotFound = 5,
        [Description("Invalid parameters, Failed to decrypt the parameters (参数无效，无法解密参数)")]
        InvalidParametersFailedToDecryptTheParameters = 6,
        [Description("Invalid signature (签名无效)")]
        InvalidSignature = 7,
        [Description("Unsupported currency (不支持的货币)")]
        UnsupportedCurrency = 8,
        [Description("Invalid Account type (帐户类型无效)")]
        InvalidAccountType = 9,
        [Description("Invalid product type (产品类型无效)")]
        InvalidProductType = 10,
        [Description("Insufficient balance to fund out / withdraw (提现余额不足)")]
        InsufficientBalanceToFundOutWithdraw = 11,
        [Description("Transaction already exists (交易序号已經存在)")]
        TransactionAlreadyExists = 12,
        [Description("Invalid game code (游戏代码无效)")]
        InvalidGameCode = 13,
        [Description("User Does Not Exists (用户不存在)")]
        UserDoesNotExists = 15,
        [Description("Insufficient merchant credit to fund in (信用额度不足)")]
        InsufficientMerchantCreditToFundIn = 16,
        [Description("Trial mode is not supported for this game code (此游戏代码不支持试用模式)")]
        TrialModeIsNotSupportedForThisGameCode = 18,
        [Description("Batch not ready (批次理未准备好)")]
        BatchNotReady = 19,
        [Description("Method not found (找不到方法)")]
        MethodNotFound = 21,
        [Description("Parameter Validation Failed (参数验证失败)")]
        ParameterValidationFailed = 22,
        [Description("API is busy (API 繁忙)")]
        APIIsBusy = 23,
        [Description("Transaction not found (未找到此交易)")]
        TransactionNotFound = 24,
        [Description("Reservation already process (預約已經完成)")]
        ReservationAlreadyProcess = 25,
        [Description("Decimal not supported (Decimal 格式位支持)")]
        DecimalNotSupported = 26,
        [Description("API is under maintenance (接口正在维护中)")]
        APIIsUnderMaintenance = 27,
        [Description("API is prohibited for this merchant (此商户禁止使用此接口)")]
        APIIsProhibitedForThisMerchant = 28,
        [Description("Product is under maintenance (产品正在维护中)")]
        ProductIsUnderMaintenance = 30
    }
}
