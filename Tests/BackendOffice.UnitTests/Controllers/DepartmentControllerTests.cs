using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages.Department;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class DepartmentControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new DepartmentController(
                _queriesMock.Object,
                _commandsMock.Object);
            _controller.ControllerContext.HttpContext = HttpContextMock.Object;
        }

        private DepartmentController _controller;
        private readonly Mock<ITgsQueries<Department>> _queriesMock = new();
        private readonly Mock<ITgsCommands<Department>> _commandsMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Departments(bool showAll)
        {
            var request = new GetDepartmentsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "Name",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreateDepartmentFixture()
                .CreateMany<Department>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.Name);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetListAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Name.Should().Be(expectedRoles[i].Name);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Create_Department_With_Name_Exists()
        {
            var request = new Fixture().Create<DepartmentRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.NameExists);
        }

        [Test]
        public async Task Can_Create_Department()
        {
            var request = new Fixture().Create<DepartmentRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();

            _queriesMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()));
            _commandsMock.Verify(p => p.CreateAsync(It.Is<Department>(d => d.Name == request.Name)));
        }

        [Test]
        public async Task Can_Update_Department()
        {
            var department = CreateDepartmentFixture().Create<Department>();
            var request = new Fixture().Create<DepartmentRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult(department));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(department.Id, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Department>(r =>
                r.Name == department.Name)));
        }

        [Test]
        public async Task Can_Update_Department_Not_Found()
        {
            var department = CreateDepartmentFixture().Create<Department>();
            var request = new Fixture().Create<DepartmentRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult<Department>(null));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(department.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Department>(r =>
                r.Name == department.Name)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Department_With_Name_Exists()
        {
            var department = CreateDepartmentFixture().Create<Department>();
            var request = new Fixture().Create<DepartmentRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult(department));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdateAsync(department.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.NameExists);
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Department>(r =>
                r.Name == department.Name)), Times.Never);
        }

        [Test]
        public async Task Can_Delete_Department()
        {
            var department = CreateDepartmentFixture().Create<Department>();
            var departmentId = Guid.NewGuid();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult(department));

            var response = await _controller.DeleteAsync(departmentId);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()));
            _commandsMock.Verify(p => p.DeleteAsync(It.Is<Department>(r =>
                r.Name == department.Name)));
        }


        [Test]
        public async Task Cannot_Delete_Department_Not_Found()
        {
            var department = CreateDepartmentFixture().Create<Department>();
            var departmentId = Guid.NewGuid();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult<Department>(null));

            var response = await _controller.DeleteAsync(departmentId);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()));
            _commandsMock.Verify(p => p.DeleteAsync(It.Is<Department>(r =>
                r.Name == department.Name)), Times.Never);
        }

        [Test]
        public async Task Can_Delete_Department_With_Users_Used()
        {
            var department = CreateDepartmentFixture().Create<Department>();
            var departmentId = Guid.NewGuid();
            department.Users = new[] {new User()};

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult(department));

            var response = await _controller.DeleteAsync(departmentId);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.CannotBeDeleted);
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()));
            _commandsMock.Verify(p => p.DeleteAsync(It.Is<Department>(r =>
                r.Name == department.Name)), Times.Never);
        }

        private Fixture CreateDepartmentFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<Department>(u => u
                .With(s => s.Users, new User[] { }));
            return fixture;
        }
    }
}