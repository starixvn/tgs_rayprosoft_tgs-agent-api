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
            var brand = await _tgsRepository.Brands.FirstAsync(x => x.Id == query.BrandId);
            var brandName = brand.Name;

            var records = from trans in _tgsRepository.Transactions
                        .AsQueryable()
                        .Where(x => x.BrandId == query.BrandId
                            && x.PlayerCode == query.PlayerCode
                            && (x.Type >= TransactionType.GameProviderFundOut
                                && x.Type <= TransactionType.GameProviderFundIn))
                          join gpftt in _tgsRepository.GameProviderFundTransferTransactions on trans.Id equals gpftt.TransactionId
                          join gp in _tgsRepository.GameProviders on gpftt.GameProviderId equals gp.Id
                          select new GameBalanceRecord()
                          {
                              Amount = trans.Amount,
                              From = trans.Type == TransactionType.GameProviderFundIn ? gp.Name : brandName,
                              To = trans.Type == TransactionType.GameProviderFundIn ? brandName : gp.Name,
                              BalanceTime = trans.RecordedOn
                          };

            foreach (var item in records)
            {
                yield return item;
            }
        }
    }
}