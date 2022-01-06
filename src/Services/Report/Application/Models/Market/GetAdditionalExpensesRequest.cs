using System;
using MediatR;

namespace TGS.Services.Report.Application.Models.Market
{
    public class GetAdditionalExpensesRequest : IRequest<GetAdditionalExpensesResponse>
    {
        public Guid BrandId { get; set; }

        public DateTimeOffset ReportDate { get; set; }
    }
}