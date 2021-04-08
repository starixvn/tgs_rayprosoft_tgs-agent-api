using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Paging;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    internal class DepositFormQueryService : IQueryService<DepositFormQuery, DepositFormInfo>, IGetService<DepositFormQuery, int>
    {
        private readonly ITgsRepository _tgsRepository;

        public DepositFormQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<int> GetAsync(DepositFormQuery query, CancellationToken cancellationToken = default)
        {
            return await MakeQuery(query).CountAsync().ConfigureAwait(false);
        }

        public IAsyncEnumerable<DepositFormInfo> QueryAsync(DepositFormQuery query, CancellationToken cancellationToken = default)
        {
            var dataQuery = MakeQuery(query);

            if (!query.Paging.ShowAll)
            {
                var dbPaging = new Paging<DepositForm>();
                dbPaging.Page = query.Paging.Page;
                dbPaging.RowCount = query.Paging.Rows;

                dbPaging.SortByPropertyName(query.Paging.SortBy, query.Paging.IsDesc);

                dataQuery = dataQuery.ApplyOrderBy(dbPaging).ApplyPaging(dbPaging);
            }

            return dataQuery
                .Select(form => new DepositFormInfo(
                    form.Id,
                    form.PlayerCode,
                    form.Player.Username,
                    form.BrandId,
                    form.BrandPaymentId,
                    form.BankAccount,
                    form.Amount,
                    form.Remark,
                    form.CreatedOn,
                    form.UpdateOn))
                .AsAsyncEnumerable();
        }

        private IQueryable<DepositForm> MakeQuery(DepositFormQuery query)
        {
            var dataQuery = _tgsRepository.DepositForms
                            .Include(f => f.Player)
                            .Include(f => f.BrandPayment)
                            .Where(form => form.BrandId == query.BrandId);

            if (!string.IsNullOrWhiteSpace(query.UserName))
                dataQuery = dataQuery.Where(form => form.Player.Username == query.UserName);

            if (!string.IsNullOrWhiteSpace(query.ChannelName))
                dataQuery = dataQuery.Where(form => form.BrandPayment.ChannelName == query.ChannelName);

            if (query.PaymentMethodId != null)
                dataQuery = dataQuery.Where(form => form.BrandPayment.PaymentMethodId == query.PaymentMethodId);

            if (query.CreateOnRange != null)
                dataQuery = dataQuery.Where(form => form.CreatedOn >= query.CreateOnRange.Value.Begin
                    && form.CreatedOn < query.CreateOnRange.Value.End);

            if (query.UpdateOnRange != null)
                dataQuery = dataQuery.Where(form => form.UpdateOn >= query.UpdateOnRange.Value.Begin
                    && form.UpdateOn < query.UpdateOnRange.Value.End);

            return dataQuery;
        }
    }
}
