using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerAuthenticationForm
    {
        public Guid BrandId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string Data { get; set; }

        public Guid Id { get; set; }

        public string PlayerCode { get; set; }

        public DateTimeOffset? ReviewedOn { get; set; }

        public string ReviewedUserId { get; set; }

        public string ReviewedUserName { get; set; }

        public int Status { get; set; }
    }
}
