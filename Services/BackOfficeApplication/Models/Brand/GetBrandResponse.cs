using TGS.Core.Models.Messages;

namespace TGS.Services.BackOfficeApplication.Models.Brand
{
    public class GetBrandResponse : MessageResponse
    {
        public BrandModel Brand { get; set; }
    }
}