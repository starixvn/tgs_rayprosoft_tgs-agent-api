using System;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Operator;
internal interface IBrandRegisterFlowDefinitionManager
{
    Task<RegisterFlowDefinition?> GetRegisterFlowDefinitionAsync(Guid brandId, CancellationToken cancellationToken = default);
}