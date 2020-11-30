using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Reseller;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Models;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IResellerService
    {
        Task<Reseller> GetResellerAsync(Expression<Func<Reseller, bool>> predicate,
            Func<IQueryable<Reseller>, IQueryable<Reseller>> preQuery = null, bool tracking = false);

        IQueryable<Reseller> GetResellersAsync(bool tracking = false);
        Task CreateResellerAsync(CreateResellerRequest request, string username);
        Task UpdatePaymentAsync(UpdateResellerRequest request, string username);
    }

    public class ResellerService : IResellerService
    {
        private readonly ITgsRepository _repository;

        public ResellerService(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task IResellerService.CreateResellerAsync(CreateResellerRequest request,
            string username)
        {
            var reseller = new Reseller
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                OperationalEmail = request.OperationalEmail,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            reseller.Languages = GenerateResellerLanguages(reseller, request);
            reseller.Currencies = GenerateResellerCurrencies(reseller, request);


            _repository.Resellers.AddAsync(reseller);
            return _repository.SaveChangesAsync();
        }

        Task<Reseller> IResellerService.GetResellerAsync(Expression<Func<Reseller, bool>> predicate,
            Func<IQueryable<Reseller>, IQueryable<Reseller>> preQuery, bool tracking)
        {
            var reseller = tracking ? _repository.Resellers : _repository.Resellers.AsNoTracking();
            preQuery ??= c => c;

            return preQuery(reseller).FirstOrDefaultAsync(predicate);
        }

        Task IResellerService.UpdatePaymentAsync(UpdateResellerRequest request, string username)
        {
            var reseller = new Reseller
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                OperationalEmail = request.OperationalEmail,
                UpdatedByUsername = username,
                UpdatedOn = DateTimeOffset.Now
            };
            reseller.Languages = GenerateResellerLanguages(reseller, request);
            reseller.Currencies = GenerateResellerCurrencies(reseller, request);

            _repository.Entry(reseller).State = EntityState.Modified;

            return _repository.SaveChangesAsync();
        }

        IQueryable<Reseller> IResellerService.GetResellersAsync(bool tracking)
        {
            return tracking ? _repository.Resellers : _repository.Resellers.AsNoTracking();
        }

        private HashSet<ResellerLanguage> GenerateResellerLanguages(Reseller reseller, ResellerRequest request)
        {
            var resellerLanguages = _repository.Languages
                .Where(request.CultureCodes.Any(), x => request.CultureCodes.Contains(x.CultureCode)).Select(x =>
                    new ResellerLanguage
                    {
                        ResellerId = reseller.Id,
                        CultureCode = x.CultureCode,
                        Reseller = reseller,
                        Language = x
                    });
            return new HashSet<ResellerLanguage>(resellerLanguages);
        }

        private HashSet<ResellerCurrency> GenerateResellerCurrencies(Reseller reseller, ResellerRequest request)
        {
            var resellerCurrencies = _repository.Currencies
                .Where(request.CurrencyCodes.Any(), x => request.CurrencyCodes.Contains(x.Code)).Select(x =>
                    new ResellerCurrency
                    {
                        ResellerId = reseller.Id,
                        CurrencyCode = x.Code,
                        Reseller = reseller,
                        Currency = x
                    });
            return new HashSet<ResellerCurrency>(resellerCurrencies);
        }
    }
}