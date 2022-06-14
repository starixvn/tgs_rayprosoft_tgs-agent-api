using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.BrandCertificate.Clients;

#nullable enable

namespace TGS.Services.Operator;

internal class BrandFlowDefinitionManager : IBrandRegisterFlowDefinitionManager
{
    private readonly IBrandServiceClient _brandServiceClient;
    private readonly IMemoryCache _memoryCache;
    private readonly RegisterFlowBuilder _registerFlowBuilder;

    public BrandFlowDefinitionManager(
        RegisterFlowBuilder registerFlowBuilder,
        IBrandServiceClient brandServiceClient,
        IMemoryCache memoryCache)
    {
        _registerFlowBuilder = registerFlowBuilder ?? throw new ArgumentNullException(nameof(registerFlowBuilder));
        _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public Task<RegisterFlowDefinition?> GetRegisterFlowDefinitionAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var key = $"Flow:{brandId}";

        return _memoryCache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30D);

            return InternalGetRegisterFlowDefinitionAsync(brandId, cancellationToken);
        });
    }

    private async Task<RegisterFlowDefinition?> InternalGetRegisterFlowDefinitionAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        var brandFrontendInfo = await _brandServiceClient
            .GetBrandFrontendInfoAsync(brandId)
            .ConfigureAwait(false);

        if (brandFrontendInfo.PlayerRegisterFlowSettings is null)
            return null;

        return _registerFlowBuilder.LoadSettings(brandFrontendInfo.PlayerRegisterFlowSettings);
    }
}
