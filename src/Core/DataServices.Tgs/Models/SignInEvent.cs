using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class SignInEvent
    {
        public Guid BrandId { get; set; }

        public int Combo { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime Date { get; set; }

        public Guid Id { get; set; }

        public string PlayerCode { get; set; }
    }
}
