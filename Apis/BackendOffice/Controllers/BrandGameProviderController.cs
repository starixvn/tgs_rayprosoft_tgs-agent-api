using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.DataServices;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandGameProviderController : BaseContextController
    {
        private ITgsRepository _repository;

        public BrandGameProviderController(ITgsRepository repository, IHttpContextAccessor httpContextAccessor) : base(
            httpContextAccessor)
        {
            _repository = repository;
        }

        public MessageResponse GetBrandGameProvidersAsync()
        {
            return CreateValidResponse<MessageResponse>();
        }
    }
}