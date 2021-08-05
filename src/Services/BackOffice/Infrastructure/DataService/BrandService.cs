using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Infrastructure.RedisServices;
using TGS.Services.BackOffice.Infrastructure.Services;
using IsolationLevel = System.Data.IsolationLevel;

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
        Task SyncBrandGamesAsync(Guid brandId);
        Task<bool> ExistsAsync(Expression<Func<Brand, bool>> predicate);
    }

    public class BrandService : BaseRepositoryOperations<ITgsRepository>, IBrandService
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly ICredentialGenerator _credentialGenerator;
        private readonly IMaintenanceRedisService _maintenanceRedisService;
        private readonly ITgsRepository _repository;

        public BrandService(
            ITgsRepository tgsRepository,
            ICredentialGenerator credentialGenerator,
            IRedisCacheManager cacheManager,
            IMaintenanceRedisService maintenanceRedisService) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
            _credentialGenerator = credentialGenerator;
            _cacheManager = cacheManager;
            _maintenanceRedisService = maintenanceRedisService ??
                throw new ArgumentNullException(nameof(maintenanceRedisService));
        }

        async Task IBrandService.ChangeStatusAsync(Brand brand, UpdateBrandStatusRequest request)
        {
            brand.Status = request.Status;
            brand.UpdatedOn = DateTimeOffset.Now;
            brand.UpdatedByUsername = request.EditorUsername;

            await UpdateEntityAsync(brand);
            await _cacheManager.ClearCacheAsync(RedisCacheType.Brand, brand.Id);
            if (request.Status is Status.Active or Status.Maintenance)
            {
                var licensee = _repository.Licensees.AsQueryable().First(x => x.Id == brand.LicenseeId);
                var reseller = _repository.Resellers.AsQueryable().First(x => x.Id == licensee.ResellerId);
                var isActive = reseller.Status is Status.Active && licensee.Status is Status.Active &&
                    request.Status is Status.Active;
                if (isActive)
                {
                    await _maintenanceRedisService.CleartBrandMaintenanceAsync(brand.Id).ConfigureAwait(false);
                }
                else
                {
                    await _maintenanceRedisService.AddBrandMaintenanceAsync(brand.Id).ConfigureAwait(false);
                }
            }
        }

        async Task IBrandService.CreateBrandAsync(CreateBrandRequest request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

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
                AutoSentAgentBonus = true,
                AutoSentPlayerBonus = true,
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
            brand.GameProviders = await GenerateAddBrandGameProvidersAsync(brand, request);
            brand.BetLimits = await GenerateBetLimitsAsync(brand, request);
            brand.Games = await GenerateWithCreateGamesAsync(brand);
            brand.BrandGroups = GenerateWithCreateGroups(brand, request);
            brand.Domains = GenerateBrandDomains(brand, request);

            await CreateEntityAsync(brand);

            var platformCredit = new PlatformCredit
            {
                BrandId = brand.Id,
                Credit = 0,
                Balance = 0,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await CreateEntityAsync(platformCredit);

            scope.Complete();
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
            await DeleteEntitiesAsync(brand.Domains);

            brand.Languages = await GenerateBrandLanguagesAsync(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.BetLimits = await GenerateBetLimitsAsync(brand, request);
            brand.Games = await GenerateWithUpdateGamesAsync(brand, request);
            brand.GameProviders = GenerateBrandGameProviders(brand, request);
            brand.BrandGroups = GenerateWithUpdateGroups(brand, request);
            brand.Domains = GenerateBrandDomains(brand, request);

            await UpdateEntityAsync(brand);
            await CreateEntitiesAsync(await GenerateAddBrandGameProvidersAsync(brand, request));

            await tran.CommitAsync();

            var cacheKeys = new[]
            {
                new CacheKey
                {
                    CacheType = RedisCacheType.Brand,
                    Key = brand.Id
                },
                new CacheKey
                {
                    CacheType = RedisCacheType.Currencies,
                    Key = brand.Id
                },
                new CacheKey
                {
                    CacheType = RedisCacheType.Languages,
                    Key = brand.Id
                },
                new CacheKey
                {
                    CacheType = RedisCacheType.BrandGameProviderConfiguration,
                    Key = brand.Code
                },
                new CacheKey
                {
                    CacheType = RedisCacheType.BrandGameProviders,
                    Key = brand.Id
                },
                new CacheKey
                {
                    CacheType = RedisCacheType.BrandGames,
                    Key = brand.Id
                },
                new CacheKey
                {
                    CacheType = RedisCacheType.BrandDefaultBetLimit,
                    Key = brand.Id
                }
            };

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys);
        }

        IQueryable<Brand> IBrandService.GetBrandQueries(Func<IQueryable<Brand>, IQueryable<Brand>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IBrandService.SyncBrandGamesAsync(Guid brandId)
        {
            var brand = await _repository.Brands.AsQueryable().FirstOrDefaultAsync(x => x.Id == brandId);

            var brandCurrencies = brand.Currencies.Select(bc => bc.CurrencyCode);

            var baseGames = await _repository.Games.AsQueryable()
                .Where(x => x.AvailableToBrand)
                .Where(x => brand.GameProviders.Select(g => g.GameProviderId).Contains(x.GameProviderId))
                .Where(x => x.GameProvider.Currencies.Any(c => brandCurrencies.Contains(c.CurrencyCode)))
                .ToArrayAsync();

            baseGames = baseGames.Where(x
                => x.AvailableCurrencies.IsNullOrEmpty() ||
                x.AvailableCurrencies.Split(",").Any(c => brandCurrencies.Contains(c))).ToArray();

            var baseGameIds = baseGames.Select(x => x.Id).ToArray();
            var brandGameIds = brand.Games.Select(x => x.GameId).ToArray();

            var addList = baseGameIds.Except(brand.Games.Select(x => x.GameId)).ToArray();
            var delList = brandGameIds.Except(baseGameIds).ToArray();

            var games = addList.Select(id => new BrandGame
            {
                BrandId = brand.Id,
                GameId = id,
                DisplayOrder = 99,
                Rebate = true,
                EffectiveBettingPercentage = 100M
            });

            var delGroupGames =
                brand.BrandGroups.SelectMany(x => x.BrandGames.Where(g => delList.Contains(g.BrandGame.GameId)));
            var delBrandGames = brand.Games.Where(x => delList.Contains(x.GameId));

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await CreateEntitiesAsync(games);
            await DeleteEntitiesAsync(delGroupGames);
            await DeleteEntitiesAsync(delBrandGames);
            scope.Complete();

            await _cacheManager.ClearCacheAsync(RedisCacheType.BrandGames, brand.Id, true);
        }

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
                    new BrandCurrency
                    {
                        BrandId = brand.Id,
                        CurrencyCode = x.Code,
                        BrandCurrencyCode = x.Code
                    });
            return new(brandCurrencies);
        }

        private HashSet<BrandDomain> GenerateBrandDomains(Brand brand, BrandRequest request)
        {
            var brandDomains = request.Domains.Select(x => new BrandDomain
            {
                Url = x,
                BrandId = brand.Id
            });
            return new(brandDomains);
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
                .Where(x => x.AvailableToBrand)
                .Where(x => gameProviderIds.Contains(x.GameProviderId)).ToArrayAsync();

            var games = brandGames.Select(x =>
                new BrandGame
                {
                    Id = Guid.NewGuid(),
                    BrandId = brand.Id,
                    GameId = x.Id,
                    Game = x,
                    DisplayOrder = 99,
                    Rebate = true,
                    EffectiveBettingPercentage = 100M
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
                .Where(x => x.AvailableToBrand)
                .Where(x => gameProviderIds.Contains(x.GameProviderId)).ToArrayAsync();

            var brandGames = games.Select(x =>
            {
                var originBrandGame = brand.Games.FirstOrDefault(g => g.GameId == x.Id);

                var brandGame = new BrandGame
                {
                    BrandId = brand.Id,
                    GameId = x.Id,
                    Game = x
                };

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
            var brandGamePoolIds = brand.Games.Select(x => x.GameId).ToArray();
            var brandGroupGames = brandGameIds
                .Where(brandGameId => brandGamePoolIds.Contains(brandGameId)).Select(brandGameId =>
                    new BrandGroupGame
                    {
                        BrandGroup = brandGroup,
                        BrandGameId = brand.Games.First(g => g.GameId == brandGameId).Id
                    });
            return new(brandGroupGames);
        }

        private static HashSet<BrandGroupGame> GenerateBrandGroupGames(Brand brand, Guid brandGroupId,
            IEnumerable<int> brandGameIds)
        {
            var brandGroupGames = brandGameIds.Select(brandGameId => new BrandGroupGame
            {
                BrandGroupId = brandGroupId,
                BrandGame = brand.Games.First(g => g.GameId == brandGameId)
            });
            return new(brandGroupGames);
        }

        private async Task<HashSet<BrandGameProvider>> GenerateAddBrandGameProvidersAsync(
            Brand brand, BrandRequest request)
        {
            var gameProviderIds =
                request.GameProviders.Where(x => x.Id == null).Select(x => x.GameProviderId).ToArray();

            var gameProviders = await _repository.GameProviders.AsQueryable()
                .Where(x => gameProviderIds.Contains(x.Id)).ToArrayAsync();
            var brandGameProviders = gameProviders.Select(x =>
                {
                    var requestBrandGameProvider = request.GameProviders.First(m => m.GameProviderId == x.Id);
                    var brandGameProviderId = GuidExtensions.NewSequentialGuid();

                    var newBrandGameProvider = new BrandGameProvider
                    {
                        Id = brandGameProviderId,
                        BrandId = brand.Id,
                        GameProviderId = x.Id,
                        GameProvider = x,
                        DefaultLanguageCode = brand.Languages.FirstOrDefault()?.CultureCode,
                        GameProviderConfigurationId = requestBrandGameProvider.ConfigurationId,
                        DesktopQueryString = requestBrandGameProvider.DesktopQueryString,
                        MobileQueryString = requestBrandGameProvider.MobileQueryString,
                        Schedules = new BrandGameProviderSchedule[]
                        {
                            new()
                            {
                                BrandGameProviderId = brandGameProviderId,
                                From = DateTimeOffset.Now,
                                To = DateTimeOffset.Now.AddDays(7),
                                DayOfWeek = DayOfWeek.Monday,
                                IsDefault = true
                            }
                        }.ToHashSet()
                    };

                    return newBrandGameProvider;
                }
            );
            return new(brandGameProviders);
        }

        private static HashSet<BrandGameProvider> GenerateBrandGameProviders(
            Brand brand, BrandRequest request)
        {
            var gameProviderIds = request.GameProviders.Select(x => x.GameProviderId).ToArray();
            var brandGameProviders = brand.GameProviders.Where(x => gameProviderIds.Contains(x.GameProviderId)).Select(
                x =>
                {
                    var requestBrandGameProvider =
                        request.GameProviders.First(m => m.GameProviderId == x.GameProviderId);

                    x.DefaultLanguageCode = brand.Languages.FirstOrDefault()?.CultureCode;
                    x.GameProviderConfigurationId = requestBrandGameProvider.ConfigurationId;
                    x.DesktopQueryString = requestBrandGameProvider.DesktopQueryString;
                    x.MobileQueryString = requestBrandGameProvider.MobileQueryString;

                    return x;
                }
            );

            return new(brandGameProviders);
        }

        private HashSet<BrandGroupBetLimit> GenerateWithUpdateBrandGroupBetLimits(Brand brand, BrandGroup brandGroup,
            BrandGroupGameProviderBetLimitModel[] models)
        {
            var brandGroupBetLimits = brand.BetLimits.Select(x =>
            {
                var brandGroupBetLimit = new BrandGroupBetLimit
                {
                    BrandGroup = brandGroup,
                    BetLimitId = x.BetLimitId
                };
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
                        BrandGroupBetLimit = brandGroupBetLimit,
                        GameProviderBetLimitId = x.GameProviderBetLimitId
                    });

            return new(brandGroupGameProviderBetLimits);
        }
    }
}