using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Brand
{
    public class GetBrandResponse : MessageResponse
    {
        public BrandModel Brand { get; set; }
    }
}