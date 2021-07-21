using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Withdraw
{
    internal class GetWithdrawFormsQueryService : IGetService<GetWithdrawFormsQuery, PagingResponse<WithdrawFormInfo>>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IPaymentServiceClient _paymentServiceClient;

        public GetWithdrawFormsQueryService(
            IPlayerOperations playerOperations,
            IPaymentServiceClient paymentServiceClient)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask<PagingResponse<WithdrawFormInfo>> GetAsync(
            GetWithdrawFormsQuery query,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations.GetPlayerByCodeAsync(query.PlayerCode);

            return await _paymentServiceClient.QueryWithdrawFormsAsync(
                new WithdrawFormQueryConditions
                {
                    BrandId = query.BrandId,
                    UserName = player.Username,
                    Status = query.Status,
                    CreateOnRange = query.CreateOnRange,
                    UpdateOnRange = query.UpdateOnRange
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
}