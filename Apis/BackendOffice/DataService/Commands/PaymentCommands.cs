using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    namespace TGS.Apis.BackendOffice.DataService.Commands
    {
        public interface IPaymentCommands
        {
            Task CreateAsync(Payment payment, PaymentUserAccessControl[] paymentUserAccessControls,
                PaymentVipLevel[] vipLevels);

            Task UpdateAsync(Payment payment, PaymentUserAccessControl[] paymentUserAccessControls,
                PaymentVipLevel[] vipLevels);
        }

        public class PaymentCommands : IPaymentCommands
        {
            private readonly ITgsRepository _repository;

            public PaymentCommands(ITgsRepository repository)
            {
                _repository = repository;
            }

            Task IPaymentCommands.CreateAsync(Payment payment,
                PaymentUserAccessControl[] paymentUserAccessControls,
                PaymentVipLevel[] vipLevels)
            {
                _repository.Payments.AddAsync(payment);
                _repository.BulkInsertAsync(vipLevels);
                _repository.BulkInsertAsync(paymentUserAccessControls);
                return _repository.SaveChangesAsync();

                //await tran.CommitAsync();
            }

            async Task IPaymentCommands.UpdateAsync(Payment payment,
                PaymentUserAccessControl[] paymentUserAccessControls,
                PaymentVipLevel[] vipLevels)
            {
                await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                _repository.Entry(payment).State = EntityState.Modified;
                _repository.PaymentUserAccessControls.RemoveRange(payment.PaymentUserAccessControls);
                _repository.PaymentVipLevels.RemoveRange(payment.VipLevels);
                await _repository.SaveChangesAsync();

                await _repository.BulkInsertAsync(paymentUserAccessControls);
                await _repository.BulkInsertAsync(vipLevels);
                await _repository.SaveChangesAsync();

                await tran.CommitAsync();
            }
        }
    }
}