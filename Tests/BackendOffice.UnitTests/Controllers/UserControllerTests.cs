using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Data.SqlClient;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Security;
using Resources = TGS.Apis.BackendOffice.Resources;

namespace BackendOffice.UnitTests.Controllers
{
    [TestFixture, Parallelizable]
    public class UserControllerTests : BaseTests
    {
        private UserController _controller;
        private readonly Mock<IHashProvider> _hashProviderMock = new Mock<IHashProvider>();
        private readonly Mock<ITgsHashConfig> _tgsHashConfigMock = new Mock<ITgsHashConfig>();
        private readonly Mock<IAntiforgery> _antiForgeryMock = new Mock<IAntiforgery>();
        private readonly Mock<IUserQueries> _userQueriesMock = new Mock<IUserQueries>();
        private readonly Mock<IDepartmentQueries> _departmentQueriesMock = new Mock<IDepartmentQueries>();
        private readonly Mock<IRoleQueries> _roleQueriesMock = new Mock<IRoleQueries>();
        private readonly Mock<IUserCommands> _userCommandsMock = new Mock<IUserCommands>();
        private readonly Mock<IJsonSerializationProvider> _jsonSerializationProviderMock = new Mock<IJsonSerializationProvider>();

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new UserController(
                _hashProviderMock.Object,
                _tgsHashConfigMock.Object,
                _antiForgeryMock.Object,
                _userQueriesMock.Object,
                _departmentQueriesMock.Object,
                _roleQueriesMock.Object,
                _userCommandsMock.Object,
                _jsonSerializationProviderMock.Object,
                _httpContextAccessorMock.Object);
        }

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

            var expectedUsers = showAll ?
                orderedUsers.ToArray() :
                orderedUsers.Skip(skip).Take(take).ToArray();

            _userQueriesMock.Setup(x => x.GetUserQueries())
                .Returns(users);

            var response = await _controller.GetUsersAsync(request);
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

            _userQueriesMock.Verify(p => p.GetUserQueries());
        }

        [Test]
        public async Task Can_Create_User()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.UsernameExistsAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult(false));

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == request.RoleId)))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetDepartmentByIdAsync(It.Is<Guid>(p => p == request.DepartmentId)))
                .Returns(Task.FromResult(new Department()));

            var response = await _controller.CreateUserAsync(request);
            response.Valid.Should().BeTrue();
            _userCommandsMock.Verify(s => s.CreateUserAsync(It.Is<User>(p =>
                p.Name == request.Name &&
                p.Username == request.Username &&
                p.RoleId == request.RoleId &&
                p.DepartmentId == request.DepartmentId)));
        }

        [Test]
        public async Task Cannot_Create_User_With_Name_Exists()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.UsernameExistsAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateUserAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.User.UsernameExistedAlready);
        }

        [Test]
        public async Task Cannot_Create_User_With_Role_Not_Found()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.UsernameExistsAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult(false));
            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == request.RoleId)))
                .Returns(Task.FromResult<Role>(null));

            var response = await _controller.CreateUserAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NotFound);
        }

        [Test]
        public async Task Cannot_Create_User_With_Department_Not_Found()
        {
            var request = new Fixture().Create<CreateUserRequest>();

            _userQueriesMock.Setup(s => s.UsernameExistsAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult(false));

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == request.RoleId)))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetDepartmentByIdAsync(It.Is<Guid>(p => p == request.DepartmentId)))
                .Returns(Task.FromResult<Department>(null));

            var response = await _controller.CreateUserAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Department.NotFound);
        }

        [Test]
        public async Task Can_Update_User()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)))
                .Returns(Task.FromResult(user));

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == request.RoleId)))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetDepartmentByIdAsync(It.Is<Guid>(p => p == request.DepartmentId)))
                .Returns(Task.FromResult(new Department()));

            var response = await _controller.UpdateUserAsync(user.Id, request);

            response.Valid.Should().BeTrue();
            _userQueriesMock.Verify(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)));
            _userCommandsMock.Verify(p => p.UpdateUserAsync(user));
        }

        [Test]
        public async Task Cannot_Update_User_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)))
                .Returns(Task.FromResult<User>(null));

            var response = await _controller.UpdateUserAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.User.NotFound);

            _userQueriesMock.Verify(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)));
            _userCommandsMock.Verify(p => p.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_User_With_Role_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)))
                .Returns(Task.FromResult(user));

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == request.RoleId)))
                .Returns(Task.FromResult<Role>(null));

            var response = await _controller.UpdateUserAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NotFound);

            _userQueriesMock.Verify(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)));
            _userCommandsMock.Verify(p => p.UpdateUserAsync(user), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_User_With_Department_Not_Found()
        {
            var user = CreateUserFixture().Create<User>();
            var request = new Fixture().Create<UpdateUserRequest>();

            _userQueriesMock.Setup(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)))
                .Returns(Task.FromResult(user));

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == request.RoleId)))
                .Returns(Task.FromResult(new Role()));

            _departmentQueriesMock.Setup(s => s.GetDepartmentByIdAsync(It.Is<Guid>(p => p == request.DepartmentId)))
                .Returns(Task.FromResult<Department>(null));

            var response = await _controller.UpdateUserAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Department.NotFound);

            _userQueriesMock.Verify(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)));
            _userCommandsMock.Verify(p => p.UpdateUserAsync(user), Times.Never);
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

            _userQueriesMock.Setup(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)))
                .Returns(Task.FromResult(user));

            var response = await _controller.UpdateUserPasswordAsync(user.Id, request);

            response.Valid.Should().BeTrue();

            _userQueriesMock.Verify(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)));
            _userCommandsMock.Verify(p => p.UpdateUserAsync(user));
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

            _userQueriesMock.Setup(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)))
                .Returns(Task.FromResult<User>(null));

            var response = await _controller.UpdateUserPasswordAsync(user.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.User.NotFound);
            _userQueriesMock.Verify(s => s.GetUserByIdAsync(It.Is<Guid>(p => p == user.Id)));
            _userCommandsMock.Verify(p => p.UpdateUserAsync(user), Times.Never);
        }

        private Fixture CreateUserFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<User>(u => u
                .With(s => s.Role, new Role { Name = Generator.RandomString(5) })
                .With(p => p.Department, new Department { Name = Generator.RandomString(5) }));
            return fixture;
        }
    }
}
