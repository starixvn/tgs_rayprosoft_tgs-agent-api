using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Paging;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Withdraw
{
    internal class WithdrawFormQueryService : IQueryService<WithdrawFormQuery, WithdrawFormInfo>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsRepository _tgsRepository;

        public WithdrawFormQueryService(ITgsRepository tgsRepository, IJsonSerializationProvider jsonSerializationProvider)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        public IAsyncEnumerable<WithdrawFormInfo> QueryAsync(WithdrawFormQuery query, CancellationToken cancellationToken = default)
        {
            var dataQuery = MakeQuery(query);

            var dbPaging = new Paging<WithdrawForm>();

            if (query.Paging.ShowAll)
            {
                dbPaging.SortByPropertyName(query.Paging.SortBy, query.Paging.IsDesc);

                dataQuery = dataQuery.ApplyOrderBy(dbPaging);
            }
            else
            {
                dbPaging.Page = query.Paging.Page;
                dbPaging.RowCount = query.Paging.Rows;

                dbPaging.SortByPropertyName(query.Paging.SortBy, query.Paging.IsDesc);

                dataQuery = dataQuery.ApplyOrderBy(dbPaging).ApplyPaging(dbPaging);
            }

            return dataQuery
                .ToAsyncEnumerable()
                .Select(form => new WithdrawFormInfo(
                    form.Id,
                    form.PlayerCode,
                    form.Player.Username,
                    form.BrandId,
                    form.BrandPaymentId,
                    form.BrandPayment?.ChannelName,
                    form.BrandPayment?.PaymentMethodChannel.MethodId,
                    form.Amount,
                    form.Remark,
                    form.Status,
                    string.IsNullOrWhiteSpace(form.ApplyInfo)
                        ? null
                        : _jsonSerializationProvider.DeserializeFromString<JsonDocument>(form.ApplyInfo, null),
                    string.IsNullOrWhiteSpace(form.PaymentReturn)
                        ? null
                        : _jsonSerializationProvider.DeserializeFromString<JsonDocument>(form.PaymentReturn, null),
                    form.CreatedOn,
                    form.UpdatedOn))
                .AsAsyncEnumerable();
        }

        private IQueryable<WithdrawForm> MakeQuery(WithdrawFormQuery query)
        {
            var dataQuery = _tgsRepository.WithdrawForms
                .Include(f => f.Player)
                .Include(f => f.BrandPayment)
                .ThenInclude(bp => bp.PaymentMethodChannel)
                .Where(form => form.BrandId == query.BrandId);

            if (!string.IsNullOrWhiteSpace(query.UserName))
                dataQuery = dataQuery.Where(form => form.Player.Username == query.UserName);

            if (!string.IsNullOrWhiteSpace(query.ChannelName))
                dataQuery = dataQuery.Where(form => form.BrandPayment.ChannelName == query.ChannelName);

            if (query.PaymentMethodId != null)
                dataQuery = dataQuery.Where(form => form.BrandPayment.PaymentMethodChannel.MethodId == query.PaymentMethodId);

            if (query.Status != null)
            {
                var statusList = query.Status.ToArray();

                if (statusList.Length > 0)
                    dataQuery = dataQuery.Where(form => statusList.Contains(form.Status));
            }

            if (query.CreateOnRange != null)
                dataQuery = dataQuery.Where(form => form.CreatedOn >= query.CreateOnRange.Value.Begin
                    && form.CreatedOn < query.CreateOnRange.Value.End);

            if (query.UpdateOnRange != null)
                dataQuery = dataQuery.Where(form => form.UpdatedOn >= query.UpdateOnRange.Value.Begin
                    && form.UpdatedOn < query.UpdateOnRange.Value.End);

            return dataQuery;
        }
    }
}
