using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class GetDepositFormsQueryService : IGetService<GetDepositFormsQuery, PagingResponse<DepositFormInfo>>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IPaymentServiceClient _paymentServiceClient;

        public GetDepositFormsQueryService(
            IPlayerOperations playerOperations,
            IPaymentServiceClient paymentServiceClient)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask<PagingResponse<DepositFormInfo>> GetAsync(
            GetDepositFormsQuery query,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations.GetPlayerByCodeAsync(query.PlayerCode).ConfigureAwait(false);

            return await _paymentServiceClient.QueryDepositFormsAsync(
               new DepositFormQueryConditions
               {
                   BrandId = query.BrandId,
                   UserName = player.Username,
                   CreateOnRange = query.CreateOnRange,
                   UpdateOnRange = query.UpdateOnRange,
                   Status = query.Status
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