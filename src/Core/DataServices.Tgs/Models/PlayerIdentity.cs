using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerIdentity
    {
        public virtual Player Player { get; set; }

        public Guid PlayerAuthenticationFormId { get; set; }

        public string PlayerCode { get; set; }
    }
}
