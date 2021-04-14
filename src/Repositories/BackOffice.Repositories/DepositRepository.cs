using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public ValueTask<DepositFormDetail> GetDepositFormDetailAsync(Guid formId)
        {
            var query = from df in _tgsRepository.DepositForms
                            .Include(f => f.Player)
                            .Include(f => f.BrandPayment)
                            .ThenInclude(bp => bp.PaymentMethodChannel)
                            .AsAsyncEnumerable()
                        where df.Id == formId
                        select new DepositFormDetail(
                            df.Id,
                            df.PlayerCode,
                            df.Player.Username,
                            df.BrandId,
                            df.BrandPaymentId,
                            df.BrandPayment.ChannelName,
                            df.BrandPayment.PaymentMethodChannel.Id,
                            df.BankAccount,
                            df.Amount,
                            df.Remark,
                            df.CreatedOn,
                            df.UpdateOn,
                            df.BrandPayment.ExtendedData,
                            df.ManualReceiptFileId);

            return query.SingleAsync();
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
