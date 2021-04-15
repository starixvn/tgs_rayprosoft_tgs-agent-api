using System;
using System.Collections.Generic;
using System.Threading;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Deposit
{
    internal class BrandPaymentsQueryService : IQueryService<BrandPaymentsQuery, BrandPaymentInfo>
    {
        private readonly IWalletRepository _walletRepository;

        public BrandPaymentsQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public IAsyncEnumerable<BrandPaymentInfo> QueryAsync(
            BrandPaymentsQuery query,
            CancellationToken cancellationToken = default)
            => _walletRepository.GetBrandPaymentsAsync(query.BrandId, query.PlayerCode);
    }
}
