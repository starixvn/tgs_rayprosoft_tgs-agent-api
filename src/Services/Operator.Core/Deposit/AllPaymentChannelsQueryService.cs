using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Deposit
{
    internal class AllPaymentChannelsQueryService : IGetService<AllPaymentChannelsQuery, IDictionary<Guid, PaymentChannelInfo>>
    {
        private readonly IWalletRepository _walletRepository;

        public AllPaymentChannelsQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<IDictionary<Guid, PaymentChannelInfo>> GetAsync(AllPaymentChannelsQuery query, CancellationToken cancellationToken = default)
        {
            var allPaymentChannels = await _walletRepository.GetAllPaymentChannelsAsync()
                .ToDictionaryAsync(method => method.Id)
                .ConfigureAwait(false);

            return new ReadOnlyDictionary<Guid, PaymentChannelInfo>(allPaymentChannels);
        }
    }
}
