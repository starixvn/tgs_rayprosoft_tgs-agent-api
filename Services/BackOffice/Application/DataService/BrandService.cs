using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Application.DataService
{
    public interface IBrandService
    {
        Task<Brand> GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null, bool tracking = false);

        IQueryable<Brand> GetBrandsAsync(bool tracking = false);
        Task CreateBrandAsync(CreateBrandRequest request);
        Task UpdateBrandAsync(Brand brand, UpdateBrandRequest request);
        Task ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request);
        Task<bool> ExistsAsync(Expression<Func<Brand, bool>> predicate);
    }

    public class BrandService : BaseService, IBrandService
    {
        private readonly ITgsRepository _repository;

        public BrandService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task IBrandService.ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request)
        {
            brand.Status = request.Status;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.UpdatedByUsername = request.Username;

            return ChangeStatusAsync(brand);
        }

        async Task IBrandService.CreateBrandAsync(CreateBrandRequest request)
        {
            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                LicenseeId = request.LicenseeId,
                Code = request.Code,
                Name = request.Name,
                Status = Status.Active,
                Description = request.Description,
                OperationalEmail = request.OperationalEmail,
                ApiKey = request.ApiKey,
                ClientId = request.ClientId,
                SecretKey = request.SecretKey,
                CreatedByUsername = request.Username,
                UpdatedByUsername = request.Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            brand.Languages = await GenerateBrandLanguages(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.GameProviders = await GenerateBrandGameProviders(brand, request);

            await CreateEntityAsync(brand);
        }

        Task<Brand> IBrandService.GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery, bool tracking)
        {
            return GetEntityFirstOrDefaultAsync(predicate, preQuery, tracking);
        }

        IQueryable<Brand> IBrandService.GetBrandsAsync(bool tracking)
        {
            return GetQueriesAsync<Brand>(tracking);
        }

        Task<bool> IBrandService.ExistsAsync(Expression<Func<Brand, bool>> predicate)
        {
            return ExistsAsync(predicate);
        }

        async Task IBrandService.UpdateBrandAsync(Brand brand, UpdateBrandRequest request)
        {
            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await DeleteEntitiesAsync(brand.Languages);
            await DeleteEntitiesAsync(brand.Currencies);
            await DeleteEntitiesAsync(brand.GameProviders);
            await DeleteEntitiesAsync(brand.GameProviders.Select(x =>
                x.GameProviderConfiguration));

            brand.Code = request.Code;
            brand.Name = request.Name;
            brand.Description = request.Description;
            brand.OperationalEmail = request.OperationalEmail;
            brand.ApiKey = request.ApiKey;
            brand.ClientId = request.ClientId;
            brand.SecretKey = request.SecretKey;
            brand.UpdatedByUsername = request.Username;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.Languages = await GenerateBrandLanguages(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.GameProviders = await GenerateBrandGameProviders(brand, request);

            await UpdateEntityAsync(brand);
            await tran.CommitAsync();
        }

        private async Task<HashSet<BrandLanguage>> GenerateBrandLanguages(Brand brand, BrandRequest request)
        {
            var cultureCodes = request.BrandCultures.Select(c => c.CultureCode);
            var languages = await _repository.Languages
                .Where(x => cultureCodes.Contains(x.CultureCode)).ToArrayAsync();
            var brandLanguages = languages.Select(x =>
                new BrandLanguage
                {
                    BrandId = brand.Id,
                    CultureCode = x.CultureCode,
                    BrandCultureCode = request.BrandCultures.FirstOrDefault(bc => bc.CultureCode == x.CultureCode)
                        ?.BrandCultureCode
                });
            return new HashSet<BrandLanguage>(brandLanguages);
        }

        private HashSet<BrandCurrency> GenerateBrandCurrencies(Brand brand, BrandRequest request)
        {
            var brandCurrencies = _repository.Currencies
                .Where(x => request.CurrencyCodes.Contains(x.Code)).Select(x =>
                    new BrandCurrency
                    {
                        BrandId = brand.Id,
                        CurrencyCode = x.Code
                    });
            return new HashSet<BrandCurrency>(brandCurrencies);
        }

        private async Task<HashSet<BrandGameProvider>> GenerateBrandGameProviders(Brand brand, BrandRequest request)
        {
            var gameProviders = await _repository.GameProviders
                .Where(x => request.GameProviders.Select(g => g.Id).Contains(x.Id)).ToArrayAsync();
            var brandGameProviders = gameProviders.Select(x =>
                {
                    var configuration = request.GameProviders.First(m => m.Id == x.Id).Configuration;
                    var gameProviderConfiguration =
                        GenerateGameProviderConfiguration(x, configuration, request.Username);
                    return new BrandGameProvider
                    {
                        BrandId = brand.Id,
                        GameProviderId = x.Id,
                        DefaultLanguageCode = brand.Languages.FirstOrDefault()?.CultureCode,
                        GameProviderConfiguration = gameProviderConfiguration
                    };
                }
            );
            return new HashSet<BrandGameProvider>(brandGameProviders);
        }

        private static GameProviderConfiguration GenerateGameProviderConfiguration(GameProvider gameProvider,
            Configuration request, string username)
        {
            return new()
            {
                Name = request.Name,
                Description = request.Description,
                ClientId = request.ClientId,
                SecretKey = request.SecretKey,
                Status = request.Status,
                GameProviderId = gameProvider.Id,
                GameProvider = gameProvider,
                DesktopEndpoint = request.DesktopEndpoint,
                MobileEndpoint = request.MobileEndpoint,
                ConfigurationData = request.ConfigurationData,
                IsDefault = request.IsDefault,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };
        }
    }
}