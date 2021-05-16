using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.Payment.LeePay;
using TGS.Services.Payment;

namespace TGS.Services.BackOffice.Domain
{
    class PrimePayProcessor : IWithdrawProcessor
    {
        public string Subject => "tgs.payment.leepay.withdraw";

        public string MakeWithdrawSettings(
            Guid formId,
            decimal amount,
            string bankInfo,
            BrandPaymentInfo brandPayment)
        {
            var withdrawBankInfo = _jsonSerializationProvider
                            .DeserializeFromString<WithdrawBankInfo<LeePayBankInfo>>(bankInfo, _jsonSerializerOptions);
            var destinationAddress = withdrawBankInfo.Detail.Address;

            var brandPaymentExtendedData = _jsonSerializationProvider
                .DeserializeFromString<JsonDocument>(brandPayment.ExtendedData);

            var settings = new LeePayWithdrawSettings(
                brandPaymentExtendedData.RootElement.GetProperty("systemId").GetString(),
                brandPaymentExtendedData.RootElement.GetProperty("systemKey").GetString(),
                formId,
                destinationAddress,
                brandPaymentExtendedData.RootElement.GetProperty("channelCurrency").GetString(),
                brandPaymentExtendedData.RootElement.GetProperty("channel").GetString());

            return _leePaySerializer.Serialize(settings);
        }
    }
}
