using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class GetGameBalanceInfosQueryService : IQueryService<GetGameBalanceInfosQuery, GameBalanceRecord>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetGameBalanceInfosQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async IAsyncEnumerable<GameBalanceRecord> QueryAsync(GetGameBalanceInfosQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var transactions = _tgsRepository.Transactions
               .AsEnumerable()
               .Where(x => x.BrandId == query.BrandId
                   && x.PlayerCode == query.PlayerCode
                   && (x.Type == TransactionType.GameProviderFundOut
                       || x.Type == TransactionType.GameProviderFundIn));

            if (query.RecordOnRange != null)
                transactions = transactions.Where(x => x.RecordedOn <= query.RecordOnRange.Value.Begin
                   && x.RecordedOn < query.RecordOnRange.Value.End);

            transactions = transactions.ToList();

            var brand = await _tgsRepository.Brands.FirstAsync(x => x.Id == query.BrandId);
            var brandName = brand.Name;

            var transactionIds = transactions.Select(x => x.Id).ToArray();

            var gameProviderFundTransferTrans = _tgsRepository.GameProviderFundTransferTransactions
                .AsAsyncEnumerable()
                .Where(x => transactionIds.Contains(x.TransactionId.Value));

            var gameProviderIds = await gameProviderFundTransferTrans
                .GroupBy(x => x.GameProviderId)
                .Select(x => x.Key)
                .ToListAsync();

            var gameProviders = await _tgsRepository.GameProviders
                .AsAsyncEnumerable()
                .Where(x => gameProviderIds.Contains(x.Id))
                .ToListAsync();

            foreach (var transaction in transactions)
            {
                var gameProviderFundTransferTransaction = await gameProviderFundTransferTrans
                    .FirstAsync(x => x.TransactionId == transaction.Id);
                var from = string.Empty;
                var to = string.Empty;
                var gameProvider = gameProviders
                    .First(x => x.Id == gameProviderFundTransferTransaction.GameProviderId);

                if (transaction.Type == TransactionType.GameProviderFundIn)
                {
                    from = gameProvider.Name;
                    to = brandName;
                }
                else
                {
                    from = brandName;
                    to = gameProvider.Name;
                }

                yield return new GameBalanceRecord()
                {
                    From = from,
                    To = to,
                    Amount = Math.Abs(transaction.Amount),
                    BalanceTime = transaction.RecordedOn
                };
            }
        }
    }
}