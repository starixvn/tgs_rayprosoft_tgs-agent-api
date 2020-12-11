using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    public partial class BrandGameController
    {

        [HttpPut("{id}/{status}")]
        [Description("Change a brand game status")]
        public Task<MessageResponse> UpdateBrandStatusAsync(Guid id, GameStatus status)
        {
            return _mediator.Send(new UpdateBrandGameStatusRequest(id, status));
        }
    }
}
