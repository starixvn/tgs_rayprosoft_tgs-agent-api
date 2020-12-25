using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.GameProvider;

namespace TGS.Apis.BackendOffice.Controllers.GameProvider
{
    public partial class GameProviderController
    {
        [HttpPut("{brandId:guid}/{status}")]
        [Description("change a Brand's status")]
        public Task<MessageResponse> UpdateBrandStatusAsync(Guid brandId, Status status) =>
            _mediator.Send(new UpdateGameProviderStatusRequest(brandId, status));
    }
}