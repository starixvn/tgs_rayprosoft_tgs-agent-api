using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    public class UserPaymentAccessControl
    {
        public Guid Id { get; set; }
        public ListType ListType { get; set; }
        public Guid PlayerId { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}