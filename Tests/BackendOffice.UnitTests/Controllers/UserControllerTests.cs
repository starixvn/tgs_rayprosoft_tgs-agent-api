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
using TGS.Apis.BackendOffice.Controllers.Messages.Users;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Security;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class UserControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new UserController(
                _passwordHandlerMock.Object,
                _userQueriesMock.Object,
                _departmentQueriesMock.Object,
                _roleQueriesMock.Object,
                _userCommandsMock.Object);
            _controller.ControllerContext.HttpContext = HttpContextMock.Object;
        }

        private UserController _controller;
        private readonly Mock<IPasswordHandler> _passwordHandlerMock = new();
        private readonly Mock<ITgsQueries<User>> _userQueriesMock = new();
        private readonly Mock<ITgsQueries<Department>> _departmentQueriesMock = new();
        private readonly Mock<ITgsQueries<Role>> _roleQueriesMock = new();
        private readonly Mock<ITgsCommands<User>> _userCommandsMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Users(bool showAll)
        {
            var request = new GetUsersRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "Username",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var users = CreateUserFixture()
                .CreateMany<User>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedUsers = users.OrderByDescending(p => p.Username);

            var expectedUsers = showAll ? orderedUsers.ToArray() : orderedUsers.Skip(skip).Take(take).ToArray();

            _userQueriesMock.Setup(x => x.GetQueries())
                .Returns(users);

            var response = await _controller.GetListAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Name.Should().Be(expectedUsers[i].Name);
                response.Items[i].Username.Should().Be(expectedUsers[i].Username);
                response.Items[i].Role.Should().Be(expectedUsers[i].Role.Name);
                response.Items[i].DepartmentName.Should().Be(expectedUsers[i].Department.Name);
                response.Items[i].Id.Should().Be(expectedUsers[i].Id);
                response.Items[i].LastLoginIp.Should().Be(expectedUsers[i].LastLoginIp);
                response.Items[i].LastLoginTime.Should().Be(expectedUsers[i].LastLoginTime);
                response.Items[i].Status.Should().Be(expectedUsers[i].Status.ToString());
                response.Items[i].CreatedOn.Should().Be(expectedUsers[i].CreatedOn);
            }

            _userQueriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Can_Create_User()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(false));

            _roleQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Role, bool>>>(),
                    It.IsAny<Func<IQueryable<Role>, IQueryable<Role>>>()))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult(new Department()));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();
            _userCommandsMock.Verify(s => s.CreateAsync(It.Is<User>(p =>
                p.Name == request.Name &&
                p.Username == request.Username &&
                p.RoleId == request.RoleId &&
                p.DepartmentId == request.DepartmentId)));
        }

        [Test]
        public async Task Cannot_Create_User_With_Name_Exists()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.User.UsernameExistedAlready);
        }

        [Test]
        public async Task Cannot_Create_User_With_Role_Not_Found()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(false));
            _roleQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Role, bool>>>(),
                    It.IsAny<Func<IQueryable<Role>, IQueryable<Role>>>()))
                .Returns(Task.FromResult<Role>(null));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Role.NotFound);
        }

        [Test]
        public async Task Cannot_Create_User_With_Department_Not_Found()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(false));

            _roleQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Role, bool>>>(),
                    It.IsAny<Func<IQueryable<Role>, IQueryable<Role>>>()))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult<Department>(null));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.NotFound);
        }

        [Test]
        public async Task Can_Update_User()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .Returns(Task.FromResult(user));

            _roleQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Role, bool>>>(),
                    It.IsAny<Func<IQueryable<Role>, IQueryable<Role>>>()))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult(new Department()));

            var response = await _controller.UpdateAsync(user.Id, request);

            response.Valid.Should().BeTrue();
            _userCommandsMock.Verify(p => p.UpdateAsync(It.Is<User>(u => u.Name == request.Name)));
        }

        [Test]
        public async Task Cannot_Update_User_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .Returns(Task.FromResult<User>(null));

            var response = await _controller.UpdateAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.User.NotFound);

            _userQueriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()));
            _userCommandsMock.Verify(p => p.UpdateAsync(user), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_User_With_Role_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .Returns(Task.FromResult(user));

            _roleQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Role, bool>>>(),
                    It.IsAny<Func<IQueryable<Role>, IQueryable<Role>>>()))
                .Returns(Task.FromResult<Role>(null));

            var response = await _controller.UpdateAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Role.NotFound);

            _userQueriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()));
            _userCommandsMock.Verify(p => p.UpdateAsync(user), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_User_With_Department_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .Returns(Task.FromResult(user));

            _roleQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Role, bool>>>(),
                    It.IsAny<Func<IQueryable<Role>, IQueryable<Role>>>()))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Department, bool>>>(),
                    It.IsAny<Func<IQueryable<Department>, IQueryable<Department>>>()))
                .Returns(Task.FromResult<Department>(null));

            var response = await _controller.UpdateAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Department.NotFound);

            _userQueriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()));
            _userCommandsMock.Verify(p => p.UpdateAsync(user), Times.Never);
        }

        [Test]
        public async Task Can_Update_User_Password()
        {
            var user = CreateUserFixture().Create<User>();
            var password = Generator.RandomString(6);
            var request = new UpdatePasswordRequest
            {
                Password = password,
                ConfirmPassword = password
            };

            _userQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .Returns(Task.FromResult(user));

            var response = await _controller.UpdatePasswordAsync(user.Id, request);

            response.Valid.Should().BeTrue();

            _userQueriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()));
            _userCommandsMock.Verify(p => p.UpdateAsync(user));
        }

        [Test]
        public async Task Cannot_Update_User_Password_With_User_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var password = Generator.RandomString(6);
            var request = new UpdatePasswordRequest
            {
                Password = password,
                ConfirmPassword = password
            };

            _userQueriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()))
                .Returns(Task.FromResult<User>(null));

            var response = await _controller.UpdatePasswordAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.User.NotFound);
            _userQueriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>()));
            _userCommandsMock.Verify(p => p.UpdateAsync(user), Times.Never);
        }

        private Fixture CreateUserFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<User>(u => u
                .With(s => s.Role, new Role {Name = Generator.RandomString(5)})
                .With(p => p.Department, new Department {Name = Generator.RandomString(5)}));
            return fixture;
        }
    }
}