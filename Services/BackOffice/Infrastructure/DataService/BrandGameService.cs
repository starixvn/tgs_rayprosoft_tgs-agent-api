using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Services.BackOffice.Application.Models.BrandGame;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IBrandGameService
    {
        Task<BrandGame> GetBrandGameAsync(
            Expression<Func<BrandGame, bool>> predicate,
            Func<IQueryable<BrandGame>, IQueryable<BrandGame>> preQuery =
                null);

        IQueryable<BrandGame> GetBrandGamesAsync(
            Func<IQueryable<BrandGame>, IQueryable<BrandGame>> preQuery = null,
            bool tracking = false);

        Task UpdateBrandGameAsync(BrandGame game);

        Task UpdateBrandGamesAsync(IEnumerable<BrandGame> brandGames);
        Task UpdateBrandGameTagAsync(UpdateBrandGameTagRequest request);
    }

    public class BrandGameService : BaseService, IBrandGameService
    {
        public BrandGameService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
        }

        async Task IBrandGameService.UpdateBrandGamesAsync(IEnumerable<BrandGame> brandGames)
        {
            await UpdateEntitiesAsync(brandGames);
        }

        Task<BrandGame> IBrandGameService.GetBrandGameAsync(Expression<Func<BrandGame, bool>> predicate,
            Func<IQueryable<BrandGame>, IQueryable<BrandGame>> preQuery)
        {
            return GetEntityFirstOrDefaultAsync(predicate, preQuery);
        }

        IQueryable<BrandGame> IBrandGameService.GetBrandGamesAsync(
            Func<IQueryable<BrandGame>, IQueryable<BrandGame>> preQuery, bool tracking)
        {
            return GetQueriesAsync(preQuery, tracking);
        }

        Task IBrandGameService.UpdateBrandGameAsync(BrandGame game)
        {
            return UpdateEntityAsync(game);
        }

        Task IBrandGameService.UpdateBrandGameTagAsync(UpdateBrandGameTagRequest request)
        {
            throw new NotImplementedException();
        }
    }
}