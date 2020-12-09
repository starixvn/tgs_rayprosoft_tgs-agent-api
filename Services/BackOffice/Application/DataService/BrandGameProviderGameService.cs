using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;

namespace TGS.Services.BackOffice.Application.DataService
{
    public interface IBrandGameProviderGameService
    {
        Task<BrandGameProviderSchedule> GetBrandGameProviderGameAsync(
            Expression<Func<BrandGameProviderSchedule, bool>> predicate,
            Func<IQueryable<BrandGameProviderSchedule>, IQueryable<BrandGameProviderSchedule>> preQuery =
                null, bool tracking = false);

        IQueryable<BrandGameProviderSchedule> GetBrandGameProviderSchedulesAsync(bool tracking = false);

        Task UpdateBrandGameProviderScheduleAsync(BrandGameProviderSchedule schedule,
            UpdateScheduleRequest request);

        Task UpdateBrandGameProviderDefaultTimesAsync(BrandGameProviderSchedule[] schedules,
            UpdateDefaultTimesRequest request);

        Task ChangeStatusAsync(BrandGameProviderSchedule schedule, UpdateScheduleStatusRequest request);
    }

    public class BrandGameProviderGameService : BaseService, IBrandGameProviderGameService
    {
        private readonly ITgsRepository _repository;

        public BrandGameProviderGameService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
            _repository = tgsRepository;
        }

        IQueryable<BrandGameProviderSchedule> IBrandGameProviderGameService.GetBrandGameProviderSchedulesAsync(
            bool tracking)
        {
            return GetQueriesAsync<BrandGameProviderSchedule>(tracking);
        }

        Task IBrandGameProviderGameService.ChangeStatusAsync(BrandGameProviderSchedule brandGameProvider,
            UpdateScheduleStatusRequest request)
        {
            brandGameProvider.Status = request.Status;

            return ChangeStatusAsync(brandGameProvider);
        }

        Task IBrandGameProviderGameService.UpdateBrandGameProviderScheduleAsync(
            BrandGameProviderSchedule schedule,
            UpdateScheduleRequest request)
        {
            schedule.From = request.From;
            schedule.To = request.To;
            schedule.DayOfWeek = request.DayOfWeek;

            return UpdateEntityAsync(schedule);
        }

        async Task IBrandGameProviderGameService.UpdateBrandGameProviderDefaultTimesAsync(
            BrandGameProviderSchedule[] schedules, UpdateDefaultTimesRequest request)
        {
            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await DeleteEntitiesAsync(schedules);

            var addSchedules = request.DefaultTimes.Select(x => new BrandGameProviderSchedule
            {
                Id = Guid.NewGuid(),
                BrandGameProviderId = request.BrandGameProviderId,
                From = x.From,
                To = x.To,
                Status = ScheduleStatus.Enabled,
                IsDefault = false
            }).ToArray();

            await CreateOrUpdateEntitiesAsync(addSchedules);
            await tran.CommitAsync();
        }

        Task<BrandGameProviderSchedule> IBrandGameProviderGameService.GetBrandGameProviderGameAsync(
            Expression<Func<BrandGameProviderSchedule, bool>> predicate,
            Func<IQueryable<BrandGameProviderSchedule>, IQueryable<BrandGameProviderSchedule>> preQuery, bool tracking)
        {
            return GetEntityFirstOrDefaultAsync(predicate, preQuery, tracking);
        }
    }
}