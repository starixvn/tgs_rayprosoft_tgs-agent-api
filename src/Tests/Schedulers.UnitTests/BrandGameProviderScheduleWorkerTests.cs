using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Workers.Schedulers.Jobs;

namespace TGS.Tests.Schedulers.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class BrandGameProviderScheduleWorkerTests
    {
        [SetUp]
        public void Setup() =>
            _service = new(
                _redisCacheManager.Object,
                _serviceMock.Object,
                _logMock.Object);

        private BrandGameProviderScheduleWorker _service;
        private readonly Mock<IBrandGameProviderService> _serviceMock = new();
        private readonly Mock<ITgsLog> _logMock = new();
        private readonly Mock<IRedisCacheManager> _redisCacheManager = new();

        [TestCase(Status.Active, Status.Maintenance, -1, 10)]
        [TestCase(Status.Maintenance, Status.Active, 1, 10)]
        public async Task Can_Change_Status(Status sourceStatus, Status targetStatus, int addFromMinutes,
            int addToMinutes)
        {
            var brandGameProviders = CreateBrandGameProviderFixture()
                .CreateMany<BrandGameProvider>(2)
                .AsQueryable()
                .BuildMock().Object;

            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));

            foreach (var brandGameProvider in brandGameProviders)
            {
                brandGameProvider.Status = sourceStatus;
                brandGameProvider.IsManualChangeStatus = false;
                brandGameProvider.Schedules = new[]
                {
                    new BrandGameProviderSchedule
                    {
                        From = now.AddMinutes(addFromMinutes),
                        To = now.AddMinutes(addToMinutes),
                        DayOfWeek = now.DayOfWeek,
                        IsDefault = true
                    }
                };
            }

            _serviceMock.Setup(s => s.GetBrandGameProviderQueries(null, true))
                .Returns(brandGameProviders);

            await _service.ExecuteAsync(It.IsAny<object>());

            _serviceMock.Verify(p =>
                p.UpdateBrandGameProvidersAsync(It.Is<BrandGameProvider[]>(b =>
                    b.Length > 0 && b.All(x => x.Status == targetStatus))));
        }

        private Fixture CreateBrandGameProviderFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<BrandGameProvider>(u => u
                .With(s => s.Brand, new Brand())
                .With(s => s.GameProvider, new GameProvider())
                .With(s => s.GameProviderConfiguration, new GameProviderConfiguration())
                .With(s => s.DefaultLanguage, new Language())
                .With(s => s.Schedules, new BrandGameProviderSchedule[]
                {
                }));
            return fixture;
        }
    }
}