using System;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Enums;

namespace TGS.Apis.TGS.Apis.BackendOffice.DataService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public UserStatus Status { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public RoleType RoleType { get; set; }

        public Guid? ResellerId { get; set; }
        public Guid? LicenseeId { get; set; }
        public Guid? BrandId { get; set; }

        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string TimeZone { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }

        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}