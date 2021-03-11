using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Transactions;
using TGS.Services.BackOffice.Application.Models.VipLevel;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IVipLevelService
    {
        Task<VipLevel> GetVipLevelAsync(Expression<Func<VipLevel, bool>> predicate,
            Func<IQueryable<VipLevel>, IQueryable<VipLevel>> preQuery = null);

        IQueryable<VipLevel> GetVipLevelQueries(
            Func<IQueryable<VipLevel>, IQueryable<VipLevel>> preQuery = null,
            bool tracking = false);

        Task CreateVipLevelAsync(CreateVipLevelRequest request);
        Task UpdateVipLevelAsync(VipLevel language, UpdateVipLevelRequest request);
        Task<bool> ExistsAsync(Expression<Func<VipLevel, bool>> predicate);
    }

    public class VipLevelService : BaseRepositoryOperations<ITgsRepository>, IVipLevelService
    {
        private readonly ITgsRepository _repository;
        private readonly ITransactionManager _transactionManager;

        public VipLevelService(ITgsRepository tgsRepository, ITransactionManager transactionManager) : base(
            tgsRepository)
        {
            _transactionManager = transactionManager;
            _repository = tgsRepository;
        }

        Task<VipLevel> IVipLevelService.GetVipLevelAsync(Expression<Func<VipLevel, bool>> predicate,
            Func<IQueryable<VipLevel>, IQueryable<VipLevel>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<VipLevel> IVipLevelService.GetVipLevelQueries(
            Func<IQueryable<VipLevel>, IQueryable<VipLevel>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IVipLevelService.UpdateVipLevelAsync(
            VipLevel vipLevel, UpdateVipLevelRequest request)
        {
            vipLevel.Name = request.Name;
            vipLevel.LevelUpAccumulateBetAmount = request.LevelUpAccumulateBetAmount;
            vipLevel.LevelUpAccumulateDeposit = request.LevelUpAccumulateDeposit;
            vipLevel.LevelKeepAccumulateBetAmount = request.LevelKeepAccumulateBetAmount;
            vipLevel.LevelKeepAccumulateDeposit = request.LevelKeepAccumulateDeposit;
            vipLevel.LevelKeepCondition = request.LevelKeepCondition;
            vipLevel.BirthReward = request.BirthBenefit;
            vipLevel.PromotionReward = request.PromotionBenefit;
            vipLevel.QuarterlyReward = request.QuarterlyBenefit;
            vipLevel.LiveDealerRebate = request.LiveDealerRebate;
            vipLevel.SlotRebate = request.SlotRebate;
            vipLevel.CardGameRebate = request.CardGameRebate;
            vipLevel.SportsRebate = request.SportsRebate;
            vipLevel.VirtualSportsRebate = request.VirtualSportsRebate;
            vipLevel.UpdatedOn = DateTimeOffset.Now;

            await UpdateEntityAsync(vipLevel);
        }

        Task IVipLevelService.CreateVipLevelAsync(CreateVipLevelRequest request)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            return _transactionManager.ExecuteTransactionAsync(() => _repository,
                IsolationLevel.ReadCommitted, id,
                async _ =>
                {
                    var max = await _repository.VipLevels.MaxAsync(x => x.Level);

                    var vipLevel = new VipLevel
                    {
                        Name = request.Name,
                        Level = max + 1,
                        BrandId = request.BrandId,
                        LevelUpAccumulateBetAmount = request.LevelUpAccumulateBetAmount,
                        LevelUpAccumulateDeposit = request.LevelUpAccumulateDeposit,
                        LevelKeepAccumulateBetAmount = request.LevelKeepAccumulateBetAmount,
                        LevelKeepAccumulateDeposit = request.LevelKeepAccumulateDeposit,
                        LevelKeepCondition = request.LevelKeepCondition,
                        BirthReward = request.BirthBenefit,
                        PromotionReward = request.PromotionBenefit,
                        QuarterlyReward = request.QuarterlyBenefit,
                        LiveDealerRebate = request.LiveDealerRebate,
                        SlotRebate = request.SlotRebate,
                        CardGameRebate = request.CardGameRebate,
                        SportsRebate = request.SportsRebate,
                        VirtualSportsRebate = request.VirtualSportsRebate,
                        CreatedByUsername = request.Username,
                        UpdatedByUsername = request.Username,
                        CreatedOn = DateTimeOffset.Now,
                        UpdatedOn = DateTimeOffset.Now
                    };

                    return CreateEntityAsync(vipLevel);
                });
        }

        Task<bool> IVipLevelService.ExistsAsync(Expression<Func<VipLevel, bool>> predicate) =>
            ExistsAsync(predicate);
    }
}