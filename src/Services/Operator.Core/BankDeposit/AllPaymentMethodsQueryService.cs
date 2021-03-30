using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class AllPaymentMethodsQueryService : IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethod>>
    {
        private readonly IWalletRepository _walletRepository;

        public AllPaymentMethodsQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask<IDictionary<Guid, PaymentMethod>> GetAsync(AllPaymentMethodsQuery query)
        {
            var allPaymentMethods = await _walletRepository.GetAllPaymentMethodsAsync()
                .ToDictionaryAsync(method => method.Id)
                .ConfigureAwait(false);

            return new ReadOnlyDictionary<Guid, PaymentMethod>(allPaymentMethods);
        }
    }
}
