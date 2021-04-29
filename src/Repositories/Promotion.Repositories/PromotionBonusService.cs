using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.PromotionCore;
using TGS.Services.PromotionCore.Models.PromotionBonus;

namespace Promotion.Repositories
{
    public class PromotionBonusService : BaseRepositoryOperations<ITgsRepository>, IPromotionBonusService
    {
        private readonly ITgsRepository _repository;

        public PromotionBonusService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IPromotionBonusService.Repository => _repository;

        Task IPromotionBonusService.CreatePromotionBonusAsync(PromotionBonusModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PromotionBonus
            {
                Id = model.Id,
                BrandId = model.BrandId,
                PromotionId = model.PromotionId,
                From = model.From,
                To = model.To,
                PromotionType = model.PromotionType,
                TotalAmountLimit = model.TotalAmountLimit,
                GameType = model.GameType,
                Percentage = model.Percentage,
                BonusAmountLimit = model.BonusAmountLimit,
                CreatedOn = model.CreatedOn,
                PromotionPlayers = model.Players.Select(x => new PromotionPlayer
                {
                    PlayerCode = x.PlayerCode, PromotionBonusId = model.Id, BonusAmount = x.BonusAmount
                }).ToHashSet()
            }).ToArray());

        IQueryable<PromotionPlayer> IPromotionBonusService.GetPromotionBonusPlayersQueries(
            Func<IQueryable<PromotionPlayer>, IQueryable<PromotionPlayer>> preQuery, bool tracking)
            =>
                GetQueriesAsync(preQuery, tracking);

        IQueryable<PromotionBonus> IPromotionBonusService.GetPromotionBonusQueries(
            Func<IQueryable<PromotionBonus>, IQueryable<PromotionBonus>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}