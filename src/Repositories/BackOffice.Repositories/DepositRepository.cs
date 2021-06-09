using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.Payment;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories
{
    internal class DepositRepository : IDepositRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public DepositRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public ValueTask<DepositFormInfo> GetDepositFormDetailAsync(Guid formId)
        {
            var query = from df in _tgsRepository.DepositForms
                            .Include(f => f.Player)
                            .Include(f => f.BrandPayment)
                            .ThenInclude(bp => bp.PaymentMethodChannel)
                            .AsAsyncEnumerable()
                        where df.Id == formId
                        select new DepositFormInfo(
                            df.Id,
                            df.PlayerCode,
                            df.Player.Username,
                            df.BrandId,
                            df.BrandPaymentId,
                            df.BrandPayment.ChannelName,
                            df.BrandPayment.PaymentMethodChannel.Id,
                            df.Amount,
                            df.Remark,
                            df.Status,
                            df.ApplyInfo,
                            df.PaymentReturn,
                            df.PromotionId,
                            df.CreatedOn,
                            df.UpdatedOn);

            return query.SingleAsync();
        }
    }
}
