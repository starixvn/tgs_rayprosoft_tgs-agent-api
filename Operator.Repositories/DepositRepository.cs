using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operator;
using BrandPayment = TGS.Services.Operator.BrandPayment;
using PaymentChannel = TGS.Services.Operator.PaymentChannel;
using PaymentMethod = TGS.Services.Operator.PaymentMethod;

namespace TGS.Operator.Repositories
{
    internal class DepositRepository : IDepositRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public DepositRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask CreateDepositFormAsync(BankDepositForm form)
        {
            _tgsRepository.DepositForms.Add(new DepositForm
            {
                Id = Guid.NewGuid(),
                PlayerCode = form.PlayerCode,
                BrandId = form.BrandId,
                BrandPaymentId = form.BrandPaymentId,
                BankAccount = form.BankAccount,
                Amount = form.Amount
            });

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public IAsyncEnumerable<PaymentChannel> GetAllPaymentChannelsAsync()
        {
            return _tgsRepository.PaymentChannels.AsAsyncEnumerable()
                .Select(row => new PaymentChannel(row.Id, row.Name));
        }

        public IAsyncEnumerable<PaymentMethod> GetAllPaymentMethodsAsync()
        {
            return _tgsRepository.PaymentMethods.AsAsyncEnumerable()
                .Select(row => new PaymentMethod(row.Id, row.Name));
        }

        public async ValueTask<BrandPayment> GetBrandPaymentByIdAsync(Guid id)
        {
            var row = await _tgsRepository.BrandPayments
                .FindAsync(id)
                .ConfigureAwait(false);

            return new BrandPayment(
                row.Id,
                row.PaymentMethodId,
                row.PaymentChannelId,
                row.ChannelName,
                row.ExtendedData);
        }

        public async ValueTask<BankDepositForm> GetDepositFormByIdAsync(Guid id)
        {
            var data = await _tgsRepository.DepositForms.FindAsync(id).ConfigureAwait(false);

            return new BankDepositForm(data.PlayerCode, data.BrandId, data.BrandPaymentId, data.BankAccount, data.Amount);
        }
    }
}
