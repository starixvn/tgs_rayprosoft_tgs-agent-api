using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Data
{
    internal record RoundData
    {
        internal Guid RoundId;
        internal string ExternalRoundId;
        internal RoundStatus Status;
    }
}
