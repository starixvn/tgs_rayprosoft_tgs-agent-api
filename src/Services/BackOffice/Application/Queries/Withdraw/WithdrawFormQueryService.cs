using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Withdraw
{
    internal class WithdrawFormQueryService : IGetService<WithdrawFormQuery, PagingResponse<WithdrawFormInfo>>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public WithdrawFormQueryService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public ValueTask<PagingResponse<WithdrawFormInfo>> GetAsync(WithdrawFormQuery query, CancellationToken cancellationToken)
            => _paymentServiceClient.QueryWithdrawFormsAsync(
                new WithdrawFormQueryConditions
                {
                    BrandId = query.BrandId,
                    ChannelName = query.ChannelName,
                    CreateOnRange = query.CreateOnRange,
                    PaymentMethodId = query.PaymentMethodId,
                    Status = query.Status,
                    ThirdPartyOrderNo = query.ThirdPartyOrderNo,
                    FormId = query.FormId,
                    UpdateOnRange = query.UpdateOnRange,
                    UserName = query.UserName
                },
                new Tgs.Payment.Clients.Paging
                {
                    IsDesc = query.Paging.IsDesc,
                    Page = query.Paging.Page,
                    Rows = query.Paging.Rows,
                    ShowAll = query.Paging.ShowAll,
                    SortBy = query.Paging.SortBy
                });
    }
}
