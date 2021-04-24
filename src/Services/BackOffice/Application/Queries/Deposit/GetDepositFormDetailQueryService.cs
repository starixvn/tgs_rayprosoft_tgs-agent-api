using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    internal class GetDepositFormDetailQueryService : IGetService<GetDepositFormDetailQuery, DepositFormDetail>
    {
        private readonly IDepositRepository _depositRepository;

        public GetDepositFormDetailQueryService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public ValueTask<DepositFormDetail> GetAsync(GetDepositFormDetailQuery query, CancellationToken cancellationToken = default)
        {
            return _depositRepository.GetDepositFormDetailAsync(query.FormId);
        }
    }
}
