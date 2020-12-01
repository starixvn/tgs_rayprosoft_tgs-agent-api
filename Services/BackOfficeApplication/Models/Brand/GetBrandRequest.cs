using System;
using MediatR;

namespace TGS.Services.BackOfficeApplication.Models.Brand
{
    public class GetBrandRequest : IRequest<GetBrandResponse>
    {
        public GetBrandRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}