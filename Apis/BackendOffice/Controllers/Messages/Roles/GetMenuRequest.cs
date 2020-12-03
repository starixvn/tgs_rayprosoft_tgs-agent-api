using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{
    public class GetMenuRequest
    {
        public Guid? ResellerId { get; set; }
        public Guid? LicenseeId { get; set; }
        public Guid? BrandId { get; set; }
    }
}