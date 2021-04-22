using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Report.Application.Models.Promotion;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IPromotionBonusService
    {
        IReportRepository Repository { get; }

        IQueryable<PromotionBonus> GetPromotionBonusQueries(
            Func<IQueryable<PromotionBonus>, IQueryable<PromotionBonus>> preQuery = null,
            bool tracking = false);

        Task CreatePromotionBonusAsync(PromotionBonusModel[] model);
    }

    public class PromotionBonusService : BaseRepositoryOperations<IReportRepository>, IPromotionBonusService
    {
        private readonly IReportRepository _repository;

        public PromotionBonusService(IReportRepository repository, ITgsRepository tgsRepository) : base(repository)
        {
            _repository = repository;
        }

        IReportRepository IPromotionBonusService.Repository => _repository;

        Task IPromotionBonusService.CreatePromotionBonusAsync(PromotionBonusModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new PromotionBonus
            {
                Id = model.Id,
                PromotionId = model.PromotionId,
                From = model.From,
                To = model.To,
                PromotionType = model.PromotionType,
                TotalAmountLimit = model.TotalAmountLimit,
                GameType = model.GameType,
                Percentage = model.Percentage,
                BonusAmountLimit = model.BonusAmountLimit,
                CreatedOn = model.CreatedOn,
                Players = model.Players.Select(x => new PromotionPlayer
                {
                    PlayerCode = x.PlayerCode,
                    PromotionBonusId = model.Id,
                    IsSent = x.IsSent,
                    BonusAmount = x.BonusAmount
                }).ToHashSet()
            }).ToArray());

        IQueryable<PromotionBonus> IPromotionBonusService.GetPromotionBonusQueries(
            Func<IQueryable<PromotionBonus>, IQueryable<PromotionBonus>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}