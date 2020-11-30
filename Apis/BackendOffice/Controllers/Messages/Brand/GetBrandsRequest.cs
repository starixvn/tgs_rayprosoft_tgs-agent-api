using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Brand
{
    public class GetBrandsRequest : PageableRequest
    {
        public string Keyword { get; set; }
    }
}