using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Department
{
    public class DepartmentModel
    {
        public Guid Id { get; set; }
        public virtual Guid? ResellerId { get; set; }
        public virtual Guid? LicenseeId { get; set; }
        public virtual Guid? BrandId { get; set; }
        public string Name { get; set; }
    }
}