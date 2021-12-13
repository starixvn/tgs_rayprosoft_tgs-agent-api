using System;
using System.Collections.Generic;

namespace TGS.Services.BackOffice.Application.Models.BrandAllAgent
{
    public record BrandAllAgentModel(Guid BrandId, IEnumerable<BrandAllAgentGameTypeModel> BrandAllAgentGameTypes);
}