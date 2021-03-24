using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class AllPaymentChannelsQueryService : IGetService<AllPaymentChannelsQuery, IDictionary<Guid, PaymentChannel>>
    {
        private readonly IDepositRepository _depositRepository;

        public AllPaymentChannelsQueryService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public async ValueTask<IDictionary<Guid, PaymentChannel>> GetAsync(AllPaymentChannelsQuery query)
        {
            var allPaymentChannels = await _depositRepository.GetAllPaymentChannelsAsync()
                .ToDictionaryAsync(method => method.Id)
                .ConfigureAwait(false);

            return new ReadOnlyDictionary<Guid, PaymentChannel>(allPaymentChannels);
        }
    }
}
