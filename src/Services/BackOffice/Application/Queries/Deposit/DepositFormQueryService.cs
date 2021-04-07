using System;
using System.Collections.Generic;
using System.Threading;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    internal class DepositFormQueryService : IQueryService<DepositFormQuery, DepositFormInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public DepositFormQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IAsyncEnumerable<DepositFormInfo> QueryAsync(DepositFormQuery query, CancellationToken cancellationToken = default)
        {
            var dataQuery = from form in _tgsRepository.DepositForms
                                .Include(f => f.Player)
                                .Include(f => f.BrandPayment)
                            where form.Player.Username == query.UserName
                                && form.BrandId == query.BrandId
                                && form.BrandPayment.ChannelName == query.ChannelName
                                && form.BrandPayment.PaymentMethodId == query.PaymentMethodId
                            select new DepositFormInfo(
                                form.Id,
                                form.PlayerCode,
                                form.BrandId,
                                form.BrandPaymentId,
                                form.BankAccount,
                                form.Amount,
                                "");

            throw new NotImplementedException();
        }
    }
}
