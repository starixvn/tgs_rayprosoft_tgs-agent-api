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
        private readonly IDepositRepository _depositRepository;

        public AllPaymentMethodsQueryService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public async ValueTask<IDictionary<Guid, PaymentMethod>> GetAsync(AllPaymentMethodsQuery query)
        {
            var allPaymentMethods = await _depositRepository.GetAllPaymentMethodsAsync()
                .ToDictionaryAsync(method => method.Id)
                .ConfigureAwait(false);

            return new ReadOnlyDictionary<Guid, PaymentMethod>(allPaymentMethods);
        }
    }
}
