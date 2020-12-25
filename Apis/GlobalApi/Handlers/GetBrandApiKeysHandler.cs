using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TGS.Apis.GlobalApi.Models.Data;
using TGS.Apis.GlobalApi.Models.Messages;
using TGS.Core.DataServices;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;

namespace TGS.Apis.GlobalApi.Handlers
{
    public sealed class GetBrandApiKeysHandler
        : IRequestHandler<GetBrandApiKeysRequest, PagedResponse<BrandApiKeyResult>>
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IWebHostEnvironment _env;
        private readonly ITgsRepository _repository;
        private readonly ISettingOperations _settingOperations;

        public GetBrandApiKeysHandler(
            ITgsRepository repository,
            ISettingOperations settingOperations,
            IRedisCacheManager cacheManager,
            IWebHostEnvironment env)
        {
            _repository = repository;
            _settingOperations = settingOperations;
            _cacheManager = cacheManager;
            _env = env;
        }

        async Task<PagedResponse<BrandApiKeyResult>>
            IRequestHandler<GetBrandApiKeysRequest, PagedResponse<BrandApiKeyResult>>
            .Handle(GetBrandApiKeysRequest request, CancellationToken cancellationToken)
        {
            var licenseeCodes = _env.IsProduction()
                ? await _settingOperations.GetValueFromJsonByKeyAsync<string[]>(SettingConsts.SuperLicenseeCodes)
                : Array.Empty<string>();

            var results = await _cacheManager.SafeGetAndCacheAsync(RedisCacheType.BrandApiKeys, null,
                TimeSpan.FromHours(12),
                () => _repository.Brands.AsNoTracking()
                    .Where(b => licenseeCodes.Length == 0 || licenseeCodes.Contains(b.Licensee.Code))
                    .Select(b => new BrandApiKeyResult(b.Code, b.ApiKey))
                    .ToArrayAsync(cancellationToken));

            if (request.ShowAll)
            {
                return new()
                {
                    TotalCount = results.Length,
                    Items = results
                };
            }

            var page = request.Page <= 0 ? 0 : request.Page - 1;
            var pageSize = request.Rows <= 0 ? 10 : request.Rows;

            return new()
            {
                TotalCount = results.Length,
                Items = results.Skip(page * pageSize).Take(pageSize).ToArray()
            };
        }
    }
}