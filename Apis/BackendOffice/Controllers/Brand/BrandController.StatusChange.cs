using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    public partial class BrandController
    {
        [HttpPut("{brandId:guid}/{status}")]
        [Description("change a Brand's status")]
        public async Task<MessageResponse> UpdateBrandStatusAsync(Guid brandId, Status status)
        {
            var brand = await _service.GetBrandAsync(q => q.Id == brandId);

            if (brand == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Brand.NotFound);
            }

            await _service.ChangeStatusAsync(brand, status, Username);

            return CreateValidResponse<MessageResponse>();
        }
    }
}