using System;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public interface ISystemPaymentRepository
    {
        ValueTask CreatePaymentMethodChannelAsync(Guid paymentId, Guid methodId, Guid channelId);

        ValueTask DeletePaymentMethodChannelAsync(Guid id);
    }
}
