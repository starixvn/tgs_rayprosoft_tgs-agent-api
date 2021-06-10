using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using Tgs.Payment.LeePay;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class GetLeePayExchangeRateQueryService : IGetService<GetLeePayExchangeRateQuery, LeePayExchangeRateInfo>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly LeePaySerializer _leePaySerializer;

        public GetLeePayExchangeRateQueryService(IPaymentClient paymentClient, LeePaySerializer leePaySerializer)
        {
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _leePaySerializer = leePaySerializer ?? throw new ArgumentNullException(nameof(leePaySerializer));
        }

        public async ValueTask<LeePayExchangeRateInfo> GetAsync(GetLeePayExchangeRateQuery query, CancellationToken cancellationToken = default)
        {
            var response = await _paymentClient.AskAsync(query.Subject, query.LeePayQueryExchangeRateSettings);
            var applyInfo = _leePaySerializer.Deserialize<LeePayQueryExchangeRateApplyInfo>(response);

            if (applyInfo.Response.Code == "00")
            {
                return new LeePayExchangeRateInfo()
                {
                    BuyPrice = applyInfo.Response.BuyPrice,
                    SellPrice = applyInfo.Response.SellPrice,
                    BizTime = applyInfo.Response.BizTime
                };
            }
            else
            {
                throw new LeePayApiException();
            }
        }
    }
}