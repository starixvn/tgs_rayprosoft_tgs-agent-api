using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.DataServices;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandGameProviderController : BaseContextController
    {
        private ITgsRepository _repository;

        public BrandGameProviderController(ITgsRepository repository)
        {
            _repository = repository;
        }

        public async Task<MessageResponse> GetBrandGameProvidersAsync()
        {
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut]
        public async Task<MessageResponse> SetScheduledMaintenanceTimeAsync()
        {
            return CreateValidResponse<MessageResponse>();
        }
    }
}