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
using TGS.Services.BackOfficeApplication.Models.Brand;

namespace TGS.Services.BackOfficeApplication.DataService.Services
{
    public interface IBrandService
    {
        Task<Brand> GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null, bool tracking = false);

        IQueryable<Brand> GetBrandsAsync(bool tracking = false);
        Task CreateBrandAsync(CreateBrandRequest request);
        Task UpdateBrandAsync(Brand brand, UpdateBrandRequest request);
        Task ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request);
    }

    public class BrandService : IBrandService
    {
        private readonly ITgsRepository _repository;

        public BrandService(ITgsRepository tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task IBrandService.ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request)
        {
            brand.Status = request.Status;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.UpdatedByUsername = request.Username;

            _repository.Entry(brand).State = EntityState.Modified;

            return _repository.SaveChangesAsync();
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

            await _repository.Brands.AddAsync(brand);
            await _repository.SaveChangesAsync();
        }

        Task<Brand> IBrandService.GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery, bool tracking)
        {
            var brand = tracking ? _repository.Brands : _repository.Brands.AsNoTracking();
            preQuery ??= c => c;

            return preQuery(brand).FirstOrDefaultAsync(predicate);
        }

        IQueryable<Brand> IBrandService.GetBrandsAsync(bool tracking)
        {
            return tracking ? _repository.Brands : _repository.Brands.AsNoTracking();
        }

        async Task IBrandService.UpdateBrandAsync(Brand brand, UpdateBrandRequest request)
        {
            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            _repository.BrandLanguages.RemoveRange(brand.Languages);
            _repository.BrandCurrencies.RemoveRange(brand.Currencies);
            _repository.BrandGameProviders.RemoveRange(brand.GameProviders);
            _repository.GameProviderConfigurations.RemoveRange(brand.GameProviders.Select(x =>
                x.GameProviderConfiguration));

            await _repository.SaveChangesAsync();

            var editBrand = await _repository.Brands.FirstOrDefaultAsync(x => x.Id == brand.Id);
            editBrand.Code = request.Code;
            editBrand.Name = request.Name;
            editBrand.Description = request.Description;
            editBrand.OperationalEmail = request.OperationalEmail;
            editBrand.ApiKey = request.ApiKey;
            editBrand.ClientId = request.ClientId;
            editBrand.SecretKey = request.SecretKey;
            editBrand.UpdatedByUsername = request.Username;
            editBrand.UpdatedOn = DateTimeOffset.Now;
            editBrand.Languages = await GenerateBrandLanguages(editBrand, request);
            editBrand.Currencies = GenerateBrandCurrencies(editBrand, request);
            editBrand.GameProviders = await GenerateBrandGameProviders(editBrand, request);

            _repository.Entry(editBrand).State = EntityState.Modified;

            await _repository.SaveChangesAsync();
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
                    Brand = brand,
                    Language = x,
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
                        CurrencyCode = x.Code,
                        Brand = brand,
                        Currency = x
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
                        Brand = brand,
                        GameProvider = x,
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
            return new GameProviderConfiguration
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