using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGame
{
    public class UpdateBrandGameTagRequest : IRequest<MessageResponse>
    {
        public Guid Id { get; set; }
        public GameTag GameTag { get; set; }
        public bool Enable { get; set; }
    }
}