using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.BackOffice.Repositories
{
    internal class DepositRepository : IDepositRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public DepositRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask SignOffDepositFormAsync(Guid formId, ReviewStatus reviewStatus)
        {
            var formData = await _tgsRepository.DepositForms
                .FindAsync(formId)
                .ConfigureAwait(false);

            formData.Status = reviewStatus switch
            {
                ReviewStatus.Approve => 8000,
                ReviewStatus.Reject => 7000,
                _ => formData.Status
            };
            formData.UpdateOn = DateTimeOffset.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
