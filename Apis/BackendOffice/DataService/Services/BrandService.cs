using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Brand;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IBrandService
    {
        Task<Brand> GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null, bool tracking = false);

        IQueryable<Brand> GetBrandsAsync(bool tracking = false);
        Task CreateBrandAsync(CreateBrandRequest request, string username);
        Task UpdateBrandAsync(Brand brand, UpdateBrandRequest request, string username);
        Task ChangeStatusAsync(Brand brand, Status status, string username);
    }

    public class BrandService : IBrandService
    {
        private readonly ITgsRepository _repository;

        public BrandService(ITgsRepository tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task IBrandService.ChangeStatusAsync(Brand brand, Status status, string username)
        {
            brand.Status = status;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.UpdatedByUsername = username;

            _repository.Entry(brand).State = EntityState.Modified;

            return _repository.SaveChangesAsync();
        }

        async Task IBrandService.CreateBrandAsync(CreateBrandRequest request, string username)
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
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            brand.Languages = await GenerateBrandLanguages(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.GameProviders = await GenerateBrandGameProviders(brand, request, username);

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

        async Task IBrandService.UpdateBrandAsync(Brand brand, UpdateBrandRequest request, string username)
        {
            _repository.BrandLanguages.RemoveRange(brand.Languages);
            _repository.BrandCurrencies.RemoveRange(brand.Currencies);
            _repository.BrandGameProviders.RemoveRange(brand.GameProviders);

            brand.Code = request.Code;
            brand.Name = request.Name;
            brand.Description = request.Description;
            brand.OperationalEmail = request.OperationalEmail;
            brand.ApiKey = request.ApiKey;
            brand.ClientId = request.ClientId;
            brand.SecretKey = request.SecretKey;
            brand.UpdatedByUsername = username;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.Languages = await GenerateBrandLanguages(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.GameProviders = await GenerateBrandGameProviders(brand, request, username);

            _repository.Entry(brand).State = EntityState.Modified;

            await _repository.SaveChangesAsync();
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

        private async Task<HashSet<BrandGameProvider>> GenerateBrandGameProviders(Brand brand, BrandRequest request,
            string username)
        {
            var gameProviders = await _repository.GameProviders
                .Where(x => request.GameProviders.Select(g => g.Id).Contains(x.Id)).ToArrayAsync();
            var brandGameProviders = gameProviders.Select(x =>
                {
                    var configuration = request.GameProviders.First(m => m.Id == x.Id).Configuration;
                    return new BrandGameProvider
                    {
                        BrandId = brand.Id,
                        GameProviderId = x.Id,
                        Brand = brand,
                        GameProvider = x,
                        DefaultLanguageCode = brand.Languages.FirstOrDefault()?.CultureCode,
                        GameProviderConfiguration = GenerateGameProviderConfiguration(x, configuration, username)
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
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ClientId = request.ClientId,
                SecretKey = request.SecretKey,
                Status = request.Status,
                GameProviderId = gameProvider.Id,
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