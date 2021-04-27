using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Infrastructure.Services;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IBrandService
    {
        Task<Brand> GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null);

        IQueryable<Brand> GetBrandQueries(Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null,
            bool tracking = false);

        Task CreateBrandAsync(CreateBrandRequest request);
        Task UpdateBrandAsync(Brand brand, UpdateBrandRequest request);
        Task ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request);
        Task<bool> ExistsAsync(Expression<Func<Brand, bool>> predicate);
    }

    public class BrandService : BaseRepositoryOperations<ITgsRepository>, IBrandService
    {
        private readonly ICredentialGenerator _credentialGenerator;
        private readonly ITgsRepository _repository;

        public BrandService(ITgsRepository tgsRepository, ICredentialGenerator credentialGenerator) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
            _credentialGenerator = credentialGenerator;
        }

        Task IBrandService.ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request)
        {
            brand.Status = request.Status;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.UpdatedByUsername = request.EditorUsername;

            return UpdateEntityAsync(brand);
        }

        async Task IBrandService.CreateBrandAsync(CreateBrandRequest request)
        {
            var (currentFromDate, currentToDate, nextFromDate, nextToDate) =
                BrandSettlementIntervalGenerator.GetInterval(request.SettlementIntervalType);
            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                LicenseeId = request.LicenseeId,
                Code = request.Code,
                Name = request.Name,
                CurrentSettlementFrom = currentFromDate,
                CurrentSettlementTo = currentToDate,
                NextSettlementFrom = nextFromDate,
                NextSettlementTo = nextToDate,
                Status = Status.Active,
                Description = request.Description,
                OperationalEmail = request.OperationalEmail,
                SettlementIntervalType = request.SettlementIntervalType,
                ApiKey = _credentialGenerator.GetApiKey(),
                ClientId = request.Code,
                SecretKey = _credentialGenerator.GetSecretKey(),
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            brand.Languages = await GenerateBrandLanguagesAsync(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.GameProviders = await GenerateBrandGameProvidersAsync(brand, request);
            brand.BetLimits = await GenerateBetLimitsAsync(brand, request);
            brand.Games = await GenerateWithCreateGamesAsync(brand);
            brand.BrandGroups = GenerateWithCreateGroups(brand, request);

            await CreateEntityAsync(brand);
        }

        Task<Brand> IBrandService.GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        Task<bool> IBrandService.ExistsAsync(Expression<Func<Brand, bool>> predicate) => ExistsAsync(predicate);

        async Task IBrandService.UpdateBrandAsync(Brand brand, UpdateBrandRequest request)
        {
            brand.Code = request.Code;
            brand.Name = request.Name;
            brand.Description = request.Description;
            brand.OperationalEmail = request.OperationalEmail;
            brand.UpdatedByUsername = request.EditorUsername;
            brand.UpdatedOn = DateTimeOffset.Now;

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await DeleteEntitiesAsync(brand.BetLimits);
            await DeleteEntitiesAsync(brand.Languages);
            await DeleteEntitiesAsync(brand.Currencies);
            await DeleteEntitiesAsync(brand.BrandGroups);

            brand.Languages = await GenerateBrandLanguagesAsync(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.BetLimits = await GenerateBetLimitsAsync(brand, request);
            brand.Games = await GenerateWithUpdateGamesAsync(brand, request);
            brand.GameProviders = await GenerateBrandGameProvidersAsync(brand, request);
            brand.BrandGroups = GenerateWithUpdateGroups(brand, request);

            await UpdateEntityAsync(brand);

            await tran.CommitAsync();
        }

        IQueryable<Brand> IBrandService.GetBrandQueries(Func<IQueryable<Brand>, IQueryable<Brand>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        private async Task<HashSet<BrandLanguage>> GenerateBrandLanguagesAsync(Brand brand, BrandRequest request)
        {
            var cultureCodes = request.Languages.Select(c => c.CultureCode);
            var languages = await _repository.Languages.AsQueryable()
                .Where(x => cultureCodes.Contains(x.CultureCode)).ToArrayAsync();
            var brandLanguages = languages.Select(x =>
                new BrandLanguage
                {
                    BrandId = brand.Id,
                    CultureCode = x.CultureCode,
                    BrandCultureCode = request.Languages.FirstOrDefault(bc => bc.CultureCode == x.CultureCode)
                        ?.BrandCultureCode
                });
            return new(brandLanguages);
        }

        private HashSet<BrandCurrency> GenerateBrandCurrencies(Brand brand, BrandRequest request)
        {
            var brandCurrencies = _repository.Currencies.AsQueryable()
                .Where(x => request.CurrencyCodes.Contains(x.Code)).Select(x =>
                    new BrandCurrency {BrandId = brand.Id, CurrencyCode = x.Code, BrandCurrencyCode = x.Code});
            return new(brandCurrencies);
        }

        private async Task<HashSet<BrandBetLimit>> GenerateBetLimitsAsync(Brand brand, BrandRequest request)
        {
            var betLimits = await _repository.BetLimits.AsQueryable()
                .Where(x => request.BetLimits.Select(b => b.Id).Contains(x.Id)).ToArrayAsync();
            var brandBetLimits = betLimits.Select(x =>
                new BrandBetLimit
                {
                    BrandId = brand.Id,
                    BetLimitId = x.Id,
                    BetLimit = x,
                    IsDefault = request.BetLimits.First(b => b.Id == x.Id).IsDefault
                });
            return new(brandBetLimits);
        }

        private HashSet<BrandGroup> GenerateWithCreateGroups(Brand brand, BrandRequest request)
        {
            var groups = (from BrandGroupType type in Enum.GetValues(typeof(BrandGroupType))
                          let id = Guid.NewGuid()
                          let code = type.ToString()
                          let gameIds = type == BrandGroupType.Sports
                              ? brand.Games.Where(x => Consts.Sports.Contains(x.Game.Type)).Select(x => x.GameId)
                                  .ToArray()
                              : brand.Games.Where(x => !Consts.Sports.Contains(x.Game.Type)).Select(x => x.GameId)
                                  .ToArray()
                          select new BrandGroup
                          {
                              Id = id,
                              Name = code,
                              IsSports = type == BrandGroupType.Sports,
                              BrandId = brand.Id,
                              Description = code,
                              BrandGames = GenerateBrandGroupGames(brand, id, gameIds),
                              BrandGroupBetLimits =
                                  GenerateBrandGroupBetLimits(brand, id, type == BrandGroupType.Sports),
                              CreatedByUsername = request.EditorUsername,
                              UpdatedByUsername = request.EditorUsername,
                              CreatedOn = DateTimeOffset.Now,
                              UpdatedOn = DateTimeOffset.Now
                          }).ToList();

            return new(groups);
        }

        private async Task<HashSet<BrandGame>> GenerateWithCreateGamesAsync(Brand brand)
        {
            var gameProviderIds = brand.GameProviders.Select(x => x.GameProviderId).ToArray();

            var brandGames = await _repository.Games.AsQueryable()
                .Where(x => gameProviderIds.Contains(x.GameProviderId)).ToArrayAsync();

            var games = brandGames.Select((x, count) =>
                new BrandGame
                {
                    Id = Guid.NewGuid(),
                    BrandId = brand.Id,
                    GameId = x.Id,
                    Game = x,
                    DisplayOrder = count,
                    Rebate = false,
                    EffectiveBettingPercentage = decimal.Zero
                });
            return new(games);
        }

        private HashSet<BrandGroup> GenerateWithUpdateGroups(Brand brand, UpdateBrandRequest request)
        {
            var brandGroups = request.BrandGroups.Select(x =>
            {
                var brandGroup = new BrandGroup
                {
                    Name = x.Name,
                    BrandId = brand.Id,
                    Description = x.Description,
                    IsSports = x.IsSports,
                    CreatedByUsername = request.EditorUsername,
                    UpdatedByUsername = request.EditorUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                };
                brandGroup.BrandGames =
                    GenerateWithUpdateBrandGroupGames(brand, brandGroup, x.BrandGames.Select(g => g.GameId));
                brandGroup.BrandGroupBetLimits =
                    GenerateWithUpdateBrandGroupBetLimits(brand, brandGroup, x.GameProviderBetLimits);

                return brandGroup;
            });

            return new(brandGroups);
        }

        private async Task<HashSet<BrandGame>> GenerateWithUpdateGamesAsync(Brand brand,
            UpdateBrandRequest request)
        {
            var gameProviderIds = request.GameProviders.Select(x => x.GameProviderId);
            var games = await _repository.Games.AsQueryable()
                .Where(x => gameProviderIds.Contains(x.GameProviderId)).ToArrayAsync();

            var brandGames = games.Select(x =>
            {
                var originBrandGame = brand.Games.FirstOrDefault(g => g.GameId == x.Id);

                var brandGame = new BrandGame {BrandId = brand.Id, GameId = x.Id, Game = x};

                if (originBrandGame == null)
                {
                    return brandGame;
                }

                brandGame.Id = originBrandGame.Id;
                brandGame.Status = originBrandGame.Status;
                brandGame.Tag = originBrandGame.Tag;
                brandGame.DisplayOrder = originBrandGame.DisplayOrder;
                brandGame.Rebate = originBrandGame.Rebate;
                brandGame.EffectiveBettingPercentage = originBrandGame.EffectiveBettingPercentage;

                return brandGame;
            });


            return new(brandGames);
        }

        private static HashSet<BrandGroupGame> GenerateWithUpdateBrandGroupGames(Brand brand, BrandGroup brandGroup,
            IEnumerable<int> brandGameIds)
        {
            var brandGroupGames = brandGameIds.Select(brandGameId => new BrandGroupGame
            {
                BrandGroup = brandGroup, BrandGameId = brand.Games.First(g => g.GameId == brandGameId).Id
            });
            return new(brandGroupGames);
        }

        private static HashSet<BrandGroupGame> GenerateBrandGroupGames(Brand brand, Guid brandGroupId,
            IEnumerable<int> brandGameIds)
        {
            var brandGroupGames = brandGameIds.Select(brandGameId => new BrandGroupGame
            {
                BrandGroupId = brandGroupId, BrandGame = brand.Games.First(g => g.GameId == brandGameId)
            });
            return new(brandGroupGames);
        }

        private async Task<HashSet<BrandGameProvider>> GenerateBrandGameProvidersAsync(
            Brand brand, BrandRequest request)
        {
            var gameProviders = await _repository.GameProviders.AsQueryable()
                .Where(x => request.GameProviders.Select(g => g.GameProviderId).Contains(x.Id)).ToArrayAsync();
            var brandGameProviders = gameProviders.Select(x =>
                {
                    var brandGameProviderId = brand.GameProviders?.FirstOrDefault(g => g.GameProviderId == x.Id)?.Id;
                    var requestBrandGameProvider = request.GameProviders.First(m => m.GameProviderId == x.Id);

                    var brandGameProvider = new BrandGameProvider
                    {
                        BrandId = brand.Id,
                        GameProviderId = x.Id,
                        GameProvider = x,
                        DefaultLanguageCode = brand.Languages.FirstOrDefault()?.CultureCode,
                        GameProviderConfigurationId = requestBrandGameProvider.ConfigurationId,
                        DesktopQueryString = requestBrandGameProvider.DesktopQueryString,
                        MobileQueryString = requestBrandGameProvider.MobileQueryString
                    };
                    if (brandGameProviderId.HasValue)
                    {
                        brandGameProvider.Id = brandGameProviderId.Value;
                    }

                    return brandGameProvider;
                }
            );
            return new(brandGameProviders);
        }

        private HashSet<BrandGroupBetLimit> GenerateWithUpdateBrandGroupBetLimits(Brand brand, BrandGroup brandGroup,
            BrandGroupGameProviderBetLimitModel[] models)
        {
            var brandGroupBetLimits = brand.BetLimits.Select(x =>
            {
                var brandGroupBetLimit = new BrandGroupBetLimit {BrandGroup = brandGroup, BetLimitId = x.BetLimitId};
                brandGroupBetLimit.GameProviderBetLimits =
                    GenerateWithUpdateGameProviderBetLimits(brandGroupBetLimit, models);

                return brandGroupBetLimit;
            });
            return new(brandGroupBetLimits);
        }

        private HashSet<BrandGroupBetLimit> GenerateBrandGroupBetLimits(Brand brand, Guid brandGroupId,
            bool isSports = false)
        {
            var brandGroupBetLimits = brand.BetLimits.Select(x =>
            {
                var id = Guid.NewGuid();
                return new BrandGroupBetLimit
                {
                    Id = id,
                    BrandGroupId = brandGroupId,
                    BetLimitId = x.BetLimitId,
                    GameProviderBetLimits = GenerateGameProviderBetLimits(brand, id, x.BetLimit, isSports)
                };
            });
            return new(brandGroupBetLimits);
        }

        private static HashSet<BrandGroupGameProviderBetLimit> GenerateGameProviderBetLimits(Brand brand,
            Guid brandGroupBetLimitId, BetLimit betLimit, bool isSports)
        {
            bool BetLimitRangeFunc(GameProviderBetLimit gb) =>
                gb.CurrencyCode == betLimit.CurrencyCode && betLimit.MinBet <= gb.MinBet &&
                betLimit.MaxBet >= gb.MaxBet;

            bool BetLimitIsSportsFunc(GameProviderBetLimit b) => betLimit.MaxBetPerMatch >= b.MaxBetPerMatch;

            var brandGroupGameProviderBetLimits = brand.GameProviders.Where(x => x.GameProvider.BetLimits
                    .Any(BetLimitRangeFunc))
                .Where(isSports, x => x.GameProvider.BetLimits
                    .Any(BetLimitIsSportsFunc))
                .Select(x =>
                {
                    var gameProviderBetLimits = x.GameProvider.BetLimits
                        .Where(BetLimitRangeFunc)
                        .Where(isSports, BetLimitIsSportsFunc)
                        .OrderBy(b => b.MinBet).ToArray();

                    return new BrandGroupGameProviderBetLimit
                    {
                        BrandGroupBetLimitId = brandGroupBetLimitId,
                        GameProviderBetLimitId = gameProviderBetLimits.First().Id
                    };
                });

            return new(brandGroupGameProviderBetLimits);
        }

        private static HashSet<BrandGroupGameProviderBetLimit> GenerateWithUpdateGameProviderBetLimits(
            BrandGroupBetLimit brandGroupBetLimit, BrandGroupGameProviderBetLimitModel[] models)
        {
            var brandGroupGameProviderBetLimits = models.Where(x => x.BrandBetLimitId == brandGroupBetLimit.BetLimitId)
                .Select(x =>
                    new BrandGroupGameProviderBetLimit
                    {
                        BrandGroupBetLimit = brandGroupBetLimit, GameProviderBetLimitId = x.GameProviderBetLimitId
                    });

            return new(brandGroupGameProviderBetLimits);
        }
    }
}