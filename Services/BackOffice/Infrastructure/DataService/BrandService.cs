using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IBrandService
    {
        Task<Brand> GetBrandAsync(Expression<Func<Brand, bool>> predicate,
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null);

        IQueryable<Brand> GetBrandsAsync(Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null,
            bool tracking = false);

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

            return UpdateEntityAsync(brand);
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

            var (gameProviders, _) = await GenerateBrandGameProviders(brand, request);

            brand.Languages = await GenerateBrandLanguages(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.GameProviders = gameProviders;
            brand.BetLimits = await GenerateBetLimits(brand, request);
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
            brand.ApiKey = request.ApiKey;
            brand.ClientId = request.ClientId;
            brand.SecretKey = request.SecretKey;
            brand.UpdatedByUsername = request.Username;
            brand.UpdatedOn = DateTimeOffset.Now;

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            var (gameProviders, deleteBrandGameProviderIds) = await GenerateBrandGameProviders(brand, request);
            var (games, deleteGameIds) = await GenerateWithUpdateGamesAsync(brand, request);

            await DeleteEntitiesAsync(brand.BetLimits);
            await DeleteEntitiesAsync(brand.Languages);
            await DeleteEntitiesAsync(brand.Currencies);
            await DeleteEntitiesAsync(brand.GameProviders.Where(x => deleteBrandGameProviderIds.Contains(x.Id)));
            await DeleteEntitiesAsync(brand.GameProviders.Where(x => deleteBrandGameProviderIds.Contains(x.Id)).Select(
                x =>
                    x.GameProviderConfiguration));
            await DeleteEntitiesAsync(brand.BrandGroups.SelectMany(x => x.BrandGroupBetLimits)
                .SelectMany(x => x.GameProviderBetLimits));
            await DeleteEntitiesAsync(brand.BrandGroups.SelectMany(x => x.BrandGroupBetLimits));
            await DeleteEntitiesAsync(brand.BrandGroups.SelectMany(x => x.BrandGames));
            await DeleteEntitiesAsync(brand.BrandGroups);
            await DeleteEntitiesAsync(brand.Games.Where(x => deleteGameIds.Contains(x.GameId)));

            brand.Languages = await GenerateBrandLanguages(brand, request);
            brand.Currencies = GenerateBrandCurrencies(brand, request);
            brand.BetLimits = await GenerateBetLimits(brand, request);
            brand.BrandGroups = GenerateWithUpdateGroups(brand, request);

            await CreateOrUpdateEntitiesAsync(gameProviders.Select(x => x.GameProviderConfiguration).ToArray());
            await CreateOrUpdateEntitiesAsync(gameProviders.ToArray());
            await CreateOrUpdateEntitiesAsync(games.ToArray());
            await UpdateEntityAsync(brand);

            await tran.CommitAsync();
        }

        IQueryable<Brand> IBrandService.GetBrandsAsync(Func<IQueryable<Brand>, IQueryable<Brand>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

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

        private async Task<HashSet<BrandBetLimit>> GenerateBetLimits(Brand brand, BrandRequest request)
        {
            var brandBetLimits = await _repository.BetLimits
                .Where(x => request.BrandBetLimits.Select(b => b.Id).Contains(x.Id)).ToArrayAsync();
            var betLimits = brandBetLimits.Select(x =>
                new BrandBetLimit
                {
                    BrandId = brand.Id,
                    BetLimitId = x.Id,
                    BetLimit = x,
                    IsDefault = request.BrandBetLimits.First(b => b.Id == x.Id).IsDefault
                });
            return new HashSet<BrandBetLimit>(betLimits);
        }

        private HashSet<BrandGroup> GenerateWithCreateGroups(Brand brand, BrandRequest request)
        {
            var groups = (from BrandGroupType type in Enum.GetValues(typeof(BrandGroupType))
                let id = Guid.NewGuid()
                let code = type.ToString()
                let gameIds = type == BrandGroupType.Sports
                    ? brand.Games.Where(x => Consts.Sports.Contains(x.Game.Type)).Select(x => x.GameId).ToArray()
                    : brand.Games.Where(x => !Consts.Sports.Contains(x.Game.Type)).Select(x => x.GameId).ToArray()
                select new BrandGroup
                {
                    Id = id,
                    Code = code,
                    Name = code,
                    IsSports = type == BrandGroupType.Sports,
                    BrandId = brand.Id,
                    Description = code,
                    BrandGames = GenerateBrandGroupGames(brand, id, gameIds),
                    BrandGroupBetLimits =
                        GenerateBrandGroupBetLimits(brand, id, type == BrandGroupType.Sports),
                    CreatedByUsername = request.Username,
                    UpdatedByUsername = request.Username,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                }).ToList();

            return new HashSet<BrandGroup>(groups);
        }

        private async Task<HashSet<BrandGame>> GenerateWithCreateGamesAsync(Brand brand)
        {
            var gameProviderIds = brand.GameProviders.Select(x => x.GameProviderId).ToArray();

            var brandGames = await _repository.Games
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
            return new HashSet<BrandGame>(games);
        }

        private HashSet<BrandGroup> GenerateWithUpdateGroups(Brand brand, UpdateBrandRequest request)
        {
            var brandBetLimits = request.BrandGroups.Select(x =>
            {
                var brandGroup = new BrandGroup
                {
                    Code = x.Code,
                    Name = x.Name,
                    BrandId = brand.Id,
                    Description = x.Description,
                    IsSports = x.IsSports,
                    CreatedByUsername = request.Username,
                    UpdatedByUsername = request.Username,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                };
                brandGroup.BrandGames = GenerateWithUpdateBrandGroupGames(brand, brandGroup, x.BrandGameIds);
                brandGroup.BrandGroupBetLimits =
                    GenerateWithUpdateBrandGroupBetLimits(brand, brandGroup, x.GameProviderBetLimits);

                return brandGroup;
            });

            return new HashSet<BrandGroup>(brandBetLimits);
        }

        private async Task<(HashSet<BrandGame>, int[] deleteGameIds)> GenerateWithUpdateGamesAsync(Brand brand,
            UpdateBrandRequest request)
        {
            var deleteGameIds = brand.Games.Select(x => x.GameId).Except(request.BrandGames.Select(x => x.GameId))
                .ToArray();
            var gameIds = request.BrandGames.Select(x => x.GameId);

            var brandGames = await _repository.Games
                .Where(x => gameIds.Contains(x.Id)).ToArrayAsync();

            var games = brandGames.Select(x =>
            {
                var originBrandGame = brand.Games.FirstOrDefault(g => g.GameId == x.Id);

                var brandGame = new BrandGame
                {
                    BrandId = brand.Id,
                    GameId = x.Id,
                    Game = x
                };

                if (originBrandGame != null)
                {
                    brandGame.Id = originBrandGame.Id;
                }

                return brandGame;
            });


            return (new HashSet<BrandGame>(games), deleteGameIds);
        }

        private static HashSet<BrandGroupGame> GenerateWithUpdateBrandGroupGames(Brand brand, BrandGroup brandGroup,
            IEnumerable<int> brandGameIds)
        {
            var brandGroupGames = brandGameIds.Select(brandGameId => new BrandGroupGame
            {
                BrandGroup = brandGroup,
                //BrandGameId = brand.Games.First(g => g.GameId == brandGameId).Id,
                BrandGame = brand.Games.First(g => g.GameId == brandGameId)
            });
            return new HashSet<BrandGroupGame>(brandGroupGames);
        }

        private static HashSet<BrandGroupGame> GenerateBrandGroupGames(Brand brand, Guid brandGroupId,
            IEnumerable<int> brandGameIds)
        {
            var brandGroupGames = brandGameIds.Select(brandGameId => new BrandGroupGame
            {
                BrandGroupId = brandGroupId,
                BrandGame = brand.Games.First(g => g.GameId == brandGameId)
            });
            return new HashSet<BrandGroupGame>(brandGroupGames);
        }

        private async Task<(HashSet<BrandGameProvider>, Guid[] deleteBrandGameProviderIds)> GenerateBrandGameProviders(
            Brand brand, BrandRequest request)
        {
            var deleteBrandGameProviderIds =
                brand.GameProviders?.Select(x => x.Id).Except(request.GameProviders.Select(x => x.Id)).ToArray();

            var gameProviders = await _repository.GameProviders
                .Where(x => request.GameProviders.Select(g => g.GameProviderId).Contains(x.Id)).ToArrayAsync();
            var brandGameProviders = gameProviders.Select(x =>
                {
                    var brandGameProviderId = brand.GameProviders.FirstOrDefault(g => g.GameProviderId == x.Id)?.Id;
                    var requestBrandGameProvider = request.GameProviders.First(m => m.GameProviderId == x.Id);
                    var gameProviderConfiguration =
                        GenerateGameProviderConfiguration(x, requestBrandGameProvider.Configuration, request.Username);
                    var brandGameProvider = new BrandGameProvider
                    {
                        BrandId = brand.Id,
                        GameProviderId = x.Id,
                        GameProvider = x,
                        DefaultLanguageCode = brand.Languages.FirstOrDefault()?.CultureCode,
                        GameProviderConfiguration = gameProviderConfiguration,
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
            return (new HashSet<BrandGameProvider>(brandGameProviders), deleteBrandGameProviderIds);
        }

        private static GameProviderConfiguration GenerateGameProviderConfiguration(GameProvider gameProvider,
            Configuration request, string username) =>
            new()
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
                    GenerateWithUpdateGameProviderBetLimits(brandGroupBetLimit, x.BetLimitId, models);

                return brandGroupBetLimit;
            });
            return new HashSet<BrandGroupBetLimit>(brandGroupBetLimits);
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
            return new HashSet<BrandGroupBetLimit>(brandGroupBetLimits);
        }

        private static HashSet<BrandGroupGameProviderBetLimit> GenerateGameProviderBetLimits(Brand brand,
            Guid brandGroupBetLimitId, BetLimit betLimit, bool isSports)
        {
            var brandGroupGameProviderBetLimits = brand.GameProviders.Select(x =>
            {
                var gameProviderBetLimits = x.GameProvider.BetLimits
                    .Where(b =>
                        b.CurrencyCode == betLimit.CurrencyCode && betLimit.MinBet <= b.MinBet &&
                        betLimit.MaxBet >= b.MaxBet)
                    .Where(isSports, b => betLimit.MaxBetPerMatch >= b.MaxBetPerMatch)
                    .OrderBy(b => b.MinBet).ToArray();

                if (!gameProviderBetLimits.Any())
                {
                    throw new Exception($"{x.GameProvider.Code} bet limits not in bet limit range");
                }

                return new BrandGroupGameProviderBetLimit
                {
                    BrandGroupBetLimitId = brandGroupBetLimitId,
                    GameProviderBetLimitId = gameProviderBetLimits.First().Id
                };
            });

            return new HashSet<BrandGroupGameProviderBetLimit>(brandGroupGameProviderBetLimits);
        }

        private static HashSet<BrandGroupGameProviderBetLimit> GenerateWithUpdateGameProviderBetLimits(
            BrandGroupBetLimit brandGroupBetLimit, int betLimitId, BrandGroupGameProviderBetLimitModel[] models)
        {
            var brandGroupGameProviderBetLimits = models
                .Where(x => x.BrandBetLimitId == betLimitId).Select(x =>
                    new BrandGroupGameProviderBetLimit
                    {
                        BrandGroupBetLimit = brandGroupBetLimit,
                        GameProviderBetLimitId = x.GameProviderBetLimitId
                    });

            return new HashSet<BrandGroupGameProviderBetLimit>(brandGroupGameProviderBetLimits);
        }
    }
}