using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class VipLevel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Player> Players { get; set; }
    }
}