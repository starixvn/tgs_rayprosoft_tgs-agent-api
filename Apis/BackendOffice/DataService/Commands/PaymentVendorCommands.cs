using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;

namespace TGS.Apis.BackendOffice.DataService.Commands
{
    public interface IPaymentVendorCommands
    {
        Task UpdateAsync(PaymentVendor paymentVendor, PaymentChannel[] paymentChannels);
    }

    public class PaymentVendorCommands : IPaymentVendorCommands
    {
        private readonly IBackendOfficeRepository _repository;

        public PaymentVendorCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task IPaymentVendorCommands.UpdateAsync(PaymentVendor paymentVendor, PaymentChannel[] paymentChannels)
        {
            _repository.Entry(paymentVendor).State = EntityState.Modified;
            _repository.PaymentMethods.RemoveRange(paymentVendor.PaymentMethods);
            _repository.PaymentChannels.AddRangeAsync(paymentChannels);
            return _repository.SaveChangesAsync();
        }
    }
}