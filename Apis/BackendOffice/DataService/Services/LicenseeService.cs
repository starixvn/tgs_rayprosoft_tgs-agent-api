using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Licensee;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface ILicenseeService
    {
        Task<Licensee> GetLicenseeAsync(Expression<Func<Licensee, bool>> predicate,
            Func<IQueryable<Licensee>, IQueryable<Licensee>> preQuery = null, bool tracking = false);

        IQueryable<Licensee> GetLicenseesAsync(bool tracking = false);
        Task CreateLicenseeAsync(CreateLicenseeRequest request, string username);
        Task UpdateLicenseeAsync(Licensee licensee, UpdateLicenseeRequest request, string username);
        Task ChangeStatusAsync(Licensee licensee, Status status, string username);
    }

    public class LicenseeService : ILicenseeService
    {
        private readonly ITgsRepository _repository;

        public LicenseeService(ITgsRepository tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task ILicenseeService.ChangeStatusAsync(Licensee licensee, Status status, string username)
        {
            licensee.Status = status;
            licensee.UpdatedOn = DateTimeOffset.Now;
            licensee.UpdatedByUsername = username;

            _repository.Entry(licensee).State = EntityState.Modified;

            return _repository.SaveChangesAsync();
        }

        Task ILicenseeService.CreateLicenseeAsync(CreateLicenseeRequest request, string username)
        {
            var licensee = new Licensee
            {
                Id = Guid.NewGuid(),
                ResellerId = request.ResellerId,
                Code = request.Code,
                Name = request.Name,
                Status = Status.Active,
                Description = request.Description,
                OperationalEmail = request.OperationalEmail,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            licensee.Languages = GenerateLicenseeLanguages(licensee, request);
            licensee.Currencies = GenerateLicenseeCurrencies(licensee, request);
            licensee.GameProviders = GenerateLicenseeGameProviders(licensee, request);

            _repository.Licensees.AddAsync(licensee);
            return _repository.SaveChangesAsync();
        }

        Task<Licensee> ILicenseeService.GetLicenseeAsync(Expression<Func<Licensee, bool>> predicate,
            Func<IQueryable<Licensee>, IQueryable<Licensee>> preQuery, bool tracking)
        {
            var licensee = tracking ? _repository.Licensees : _repository.Licensees.AsNoTracking();
            preQuery ??= c => c;

            return preQuery(licensee).FirstOrDefaultAsync(predicate);
        }

        IQueryable<Licensee> ILicenseeService.GetLicenseesAsync(bool tracking)
        {
            return tracking ? _repository.Licensees : _repository.Licensees.AsNoTracking();
        }

        Task ILicenseeService.UpdateLicenseeAsync(Licensee licensee, UpdateLicenseeRequest request, string username)
        {
            _repository.LicenseeLanguages.RemoveRange(licensee.Languages);
            _repository.LicenseeCurrencies.RemoveRange(licensee.Currencies);
            _repository.LicenseeGameProviders.RemoveRange(licensee.GameProviders);

            licensee.Code = request.Code;
            licensee.Name = request.Name;
            licensee.Description = request.Description;
            licensee.OperationalEmail = request.OperationalEmail;
            licensee.UpdatedByUsername = username;
            licensee.UpdatedOn = DateTimeOffset.Now;
            licensee.Languages = GenerateLicenseeLanguages(licensee, request);
            licensee.Currencies = GenerateLicenseeCurrencies(licensee, request);
            licensee.GameProviders = GenerateLicenseeGameProviders(licensee, request);

            _repository.Entry(licensee).State = EntityState.Modified;

            return _repository.SaveChangesAsync();
        }

        private HashSet<LicenseeLanguage> GenerateLicenseeLanguages(Licensee licensee, LicenseeRequest request)
        {
            var licenseeLanguages = _repository.Languages
                .Where(x => request.CultureCodes.Contains(x.CultureCode)).Select(x =>
                    new LicenseeLanguage
                    {
                        LicenseeId = licensee.Id,
                        CultureCode = x.CultureCode
                    });
            return new HashSet<LicenseeLanguage>(licenseeLanguages);
        }

        private HashSet<LicenseeCurrency> GenerateLicenseeCurrencies(Licensee licensee, LicenseeRequest request)
        {
            var licenseeCurrencies = _repository.Currencies
                .Where(x => request.CurrencyCodes.Contains(x.Code)).Select(x =>
                    new LicenseeCurrency
                    {
                        LicenseeId = licensee.Id,
                        CurrencyCode = x.Code
                    });
            return new HashSet<LicenseeCurrency>(licenseeCurrencies);
        }

        private HashSet<LicenseeGameProvider> GenerateLicenseeGameProviders(Licensee licensee, LicenseeRequest request)
        {
            var licenseeGameProviders = _repository.GameProviders
                .Where(x => request.GameProviderIds.Contains(x.Id)).Select(x =>
                    new LicenseeGameProvider
                    {
                        LicenseeId = licensee.Id,
                        GameProviderId = x.Id
                    });
            return new HashSet<LicenseeGameProvider>(licenseeGameProviders);
        }
    }
}