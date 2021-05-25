using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories
{
    internal class SystemPaymentRepository : ISystemPaymentRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public SystemPaymentRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask CreatePaymentMethodChannelAsync(Guid paymentId, Guid methodId, Guid channelId)
        {
            var pmc = new PaymentMethodChannel
            {
                Id = Guid.NewGuid(),
                PaymentId = paymentId,
                MethodId = methodId,
                ChannelId = channelId
            };

            await _tgsRepository.PaymentMethodChannels.AddAsync(pmc).ConfigureAwait(false);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask DeletePaymentMethodChannelAsync(Guid id)
        {
            var pmc = await _tgsRepository.PaymentMethodChannels.FindAsync(id).ConfigureAwait(false);

            _tgsRepository.PaymentMethodChannels.Remove(pmc);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
