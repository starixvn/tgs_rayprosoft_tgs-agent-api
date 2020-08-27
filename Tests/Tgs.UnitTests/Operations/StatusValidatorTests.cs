using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Tests.TestsCommon;

namespace Tgs.UnitTests.Operations
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public sealed class StatusValidatorTests : NUnitTestsBase
    {
        private IStatusValidator _statusValidator;
        private IBrandOperations _brandOperations;

        protected override void InitializeBeforeEachTest()
        {
            _brandOperations = Substitute.For<IBrandOperations>();
            _statusValidator = new StatusValidator(_brandOperations);
        }

        [Test]
        public void ThrowIfBrandStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Brand_Status_Is_Null()
        {
            var brandId = Guid.NewGuid();
            _brandOperations.GetBrandStatusByBrandIdAsync(brandId)
                .Returns(Task.FromResult((Status?)null));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                brandId,
                Status.Active);

            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfBrandStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Brand_Status_Is_Not_One_Of()
        {
            var brandId = Guid.NewGuid();
            _brandOperations.GetBrandStatusByBrandIdAsync(brandId)
                .Returns(Task.FromResult((Status?)Status.Inactive));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                brandId,
                Status.Active);

            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfBrandStatusIsNotOneOfAsync_Throws_BrandUnderMaintenanceException()
        {
            var brandId = Guid.NewGuid();
            _brandOperations.GetBrandStatusByBrandIdAsync(brandId)
                .Returns(Task.FromResult((Status?)Status.Maintenance));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                brandId,
                Status.Active);

            action.Should().Throw<BrandUnderMaintenanceException>();
        }

        [Test]
        public void ThrowIfBrandStatusIsNotOneOfAsync_Not_Throws_When_Ok()
        {
            var brandId = Guid.NewGuid();
            _brandOperations.GetBrandStatusByBrandIdAsync(brandId)
                .Returns(Task.FromResult((Status?)Status.Active));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                brandId,
                Status.Active);

            action.Should().NotThrow();
        }
    }
}