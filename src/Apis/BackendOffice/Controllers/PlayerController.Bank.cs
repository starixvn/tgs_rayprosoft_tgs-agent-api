using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerBank;

namespace TGS.Apis.BackendOffice.Controllers
{
    public partial class PlayerController
    {
        [HttpPut("{playerCode}/bank/{id}")]
        [Description("Update player bank")]
        public Task<MessageResponse> UpdatePlayerBankAsync(string playerCode, Guid id,
            [FromBody] UpdatePlayerBankRequest request)
        {
            request.PlayerCode = playerCode;
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpPost("{playerCode}/bank")]
        [Description("Create a player bank")]
        public Task<MessageResponse> CreatePlayerBankAsync(string playerCode,
            [FromBody] CreatePlayerBankRequest request)
        {
            request.PlayerCode = playerCode;
            return _mediator.Send(request);
        }

        [Route("{playerCode}/bank/{id}")]
        [HttpDelete]
        [Description("Delete a player bank")]
        public Task<MessageResponse> DeletePlayerBankAsync(string playerCode, Guid id) =>
            _mediator.Send(new DeletePlayerBankRequest(playerCode, id));
    }
}