using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Payment : BaseEntity<Guid>
    {
        public string Description { get; set; }

        public string ExtendData { get; set; }

        public ActivateStatus Status { get; set; }
    }
}