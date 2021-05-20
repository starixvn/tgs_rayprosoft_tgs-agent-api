using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion.Models.PromotionBonus;
using TGS.Services.Promotion;

namespace TGS.Promotion.Repositories
{
    public class PromotionBonusRepository : BaseRepositoryOperations<ITgsRepository>, IPromotionBonusRepository
    {
        private readonly ITgsRepository _repository;

        public PromotionBonusRepository(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IPromotionBonusRepository.Repository => _repository;

        Task IPromotionBonusRepository.CreatePromotionBonusAsync(PromotionBonusModel[] models) =>
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
                TurnoverMultiple = model.TurnoverMultiple,
                CreatedOn = model.CreatedOn,
                PromotionPlayers = model.Players.Select(x => new PromotionPlayer
                {
                    PlayerCode = x.PlayerCode,
                    PromotionBonusId = model.Id,
                    BonusAmount = x.BonusAmount,
                    SentOn = model.SentOn
                }).ToHashSet()
            }).ToArray());

        IQueryable<PromotionPlayer> IPromotionBonusRepository.GetPromotionBonusPlayersQueries(
            Func<IQueryable<PromotionPlayer>, IQueryable<PromotionPlayer>> preQuery, bool tracking)
            =>
                GetQueriesAsync(preQuery, tracking);

        IQueryable<PromotionBonus> IPromotionBonusRepository.GetPromotionBonusQueries(
            Func<IQueryable<PromotionBonus>, IQueryable<PromotionBonus>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task IPromotionBonusRepository.UpdatePromotionBonusPlayersAsync(PromotionPlayer[] players) =>
            UpdateEntitiesAsync(players);
    }
}
