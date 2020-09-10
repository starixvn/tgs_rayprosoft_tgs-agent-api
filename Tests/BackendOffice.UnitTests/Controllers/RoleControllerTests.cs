using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Services;
using TGS.Infrastructures.Json;
using Resources = TGS.Apis.BackendOffice.Resources;

namespace BackendOffice.UnitTests.Controllers
{
    [TestFixture, Parallelizable]
    public class RoleControllerTests : BaseTests
    {
        private RoleController _controller;
        private readonly Mock<IRoleQueries> _roleQueriesMock = new Mock<IRoleQueries>();
        private readonly Mock<ISettingQueries> _settingQueriesMock = new Mock<ISettingQueries>();
        private readonly Mock<IRoleCommands> _roleCommandsMock = new Mock<IRoleCommands>();
        private readonly Mock<IJsonSerializationProvider> _jsonSerializationProviderMock = new Mock<IJsonSerializationProvider>();
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new RoleController(
                _roleQueriesMock.Object,
                _settingQueriesMock.Object,
                _roleCommandsMock.Object,
                _jsonSerializationProviderMock.Object,
                _httpContextAccessorMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Roles(bool showAll)
        {
            var request = new GetRolesRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "Name",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var roles = CreateRoleFixture()
                .CreateMany<Role>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = roles.OrderByDescending(p => p.Name);

            var expectedRoles = showAll ?
                orderedRoles.ToArray() :
                orderedRoles.Skip(skip).Take(take).ToArray();

            _roleQueriesMock.Setup(x => x.GetRoleQueries())
                .Returns(roles);

            var response = await _controller.GetRolesAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Name.Should().Be(expectedRoles[i].Name);
                response.Items[i].DisplayName.Should().Be(expectedRoles[i].DisplayName);
                response.Items[i].Description.Should().Be(expectedRoles[i].Description);
                response.Items[i].CreatedOn.Should().Be(expectedRoles[i].CreatedOn);
                response.Items[i].UpdatedOn.Should().Be(expectedRoles[i].UpdatedOn);
                response.Items[i].CreatedByUsername.Should().Be(expectedRoles[i].CreatedByUsername);
                response.Items[i].UpdatedByUsername.Should().Be(expectedRoles[i].UpdatedByUsername);
            }

            _roleQueriesMock.Verify(p => p.GetRoleQueries());
        }

        [Test]
        public async Task Can_Get_Role()
        {
            var role = CreateRoleFixture().Create<Role>();
            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            _settingQueriesMock.Setup(p => p.GetSettingByKeyAsync("Menus"))
                .Returns(Task.FromResult(new Fixture().Create<Setting>()));

            var menuSettings = CreateMenuSettingFixture().CreateMany<MenuSetting>(10).ToArray();
            _jsonSerializationProviderMock.SetupSequence(p => p.DeserializeFromString<MenuSetting[]>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>()))
                .Returns(menuSettings)
                .Returns(menuSettings);

            var response = await _controller.GetRoleAsync(role.Id);
            response.Valid.Should().BeTrue();
            response.Role.Id.Should().Be(role.Id);
            response.Role.Name.Should().Be(role.Name);
            response.Role.DisplayName.Should().Be(role.DisplayName);
            response.Role.Description.Should().Be(role.Description);
            response.Role.CreatedByUsername.Should().Be(role.CreatedByUsername);
            response.Role.UpdatedByUsername.Should().Be(role.UpdatedByUsername);
            response.Role.CreatedOn.Should().Be(role.CreatedOn);
            response.Role.UpdatedOn.Should().Be(role.UpdatedOn);

            for (var i = 0; i < response.Role.Menus.Length; i++)
            {
                response.Role.Menus[i].Code.Should().Be(menuSettings[i].Code);
                response.Role.Menus[i].IsDisplay.Should().Be(menuSettings[i].IsDisplay);

                for (var j = 0; j < response.Role.Menus[i].SubMenus.Length; j++)
                {
                    var actualMenu = response.Role.Menus[i].SubMenus[j];
                    var expectedMenu = menuSettings[i].SubMenus[j];
                    actualMenu.Code.Should().Be(expectedMenu.Code);
                    actualMenu.IsDisplay.Should().Be(expectedMenu.IsDisplay);
                }
            }

        }


        [Test]
        public async Task Cannot_Get_Role()
        {
            var roleId = Guid.NewGuid();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == roleId)))
                .Returns(Task.FromResult<Role>(null));

            var response = await _controller.GetRoleAsync(roleId);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NotFound);
        }

        [Test]
        public async Task Can_Create_Role()
        {
            var request = CreateRoleRequestFixture().Create<CreateRoleRequest>();

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateRoleAsync(request);
            response.Valid.Should().BeTrue();
            _roleCommandsMock.Verify(s => s.CreateRoleAsync(It.Is<Role>(p =>
                p.Name == request.Name &&
                p.DisplayName == request.DisplayName &&
                p.Description == request.Description)));
        }

        [Test]
        public async Task Cannot_Create_User_With_Name_Exists()
        {
            var request = CreateRoleRequestFixture().Create<CreateRoleRequest>();

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateRoleAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NameExists);
        }

        [Test]
        public async Task Can_Update_Role()
        {
            var role = CreateRoleFixture().Create<Role>();
            var request = UpdateRoleRequestFixture().Create<UpdateRoleRequest>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateRoleAsync(role.Id, request);

            response.Valid.Should().BeTrue();
            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleCommandsMock.Verify(p => p.UpdateRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)));
        }

        [Test]
        public async Task Cannot_Update_Role_Not_Found()
        {
            var role = CreateRoleFixture().Create<Role>();
            var request = UpdateRoleRequestFixture().Create<UpdateRoleRequest>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult<Role>(null));

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(true));


            var response = await _controller.UpdateRoleAsync(role.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NotFound);

            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleQueriesMock.Verify(s => s.NameExistsAsync(It.Is<string>(p => p == role.Name)), Times.Never);
            _roleCommandsMock.Verify(p => p.UpdateRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Role_With_Name_Exists()
        {
            var role = CreateRoleFixture().Create<Role>();
            var request = UpdateRoleRequestFixture().Create<UpdateRoleRequest>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdateRoleAsync(role.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NameExists);

            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleQueriesMock.Verify(s => s.NameExistsAsync(It.Is<string>(p => p == role.Name)), Times.Never);
            _roleCommandsMock.Verify(p => p.UpdateRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)), Times.Never);
        }


        [Test]
        public async Task Can_Copy_Role()
        {
            var role = CreateRoleFixture().Create<Role>();
            var request = new Fixture().Create<CopyRoleRequest>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(false));

            var response = await _controller.CopyRoleAsync(role.Id, request);

            response.Valid.Should().BeTrue();
            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleCommandsMock.Verify(p => p.CreateRoleAsync(It.Is<Role>(r =>
                r.Name == request.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)));
        }

        [Test]
        public async Task Cannot_Copy_Role_Not_Found()
        {
            var role = CreateRoleFixture().Create<Role>();
            var request = new Fixture().Create<CopyRoleRequest>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult<Role>(null));

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(true));


            var response = await _controller.CopyRoleAsync(role.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NotFound);

            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleQueriesMock.Verify(s => s.NameExistsAsync(It.Is<string>(p => p == role.Name)), Times.Never);
            _roleCommandsMock.Verify(p => p.UpdateRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)), Times.Never);
        }

        [Test]
        public async Task Cannot_Copy_Role_With_Name_Exists()
        {
            var role = CreateRoleFixture().Create<Role>();
            var request = UpdateRoleRequestFixture().Create<CopyRoleRequest>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            _roleQueriesMock.Setup(s => s.NameExistsAsync(It.Is<string>(p => p == request.Name)))
                .Returns(Task.FromResult(true));

            var response = await _controller.CopyRoleAsync(role.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NameExists);

            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleQueriesMock.Verify(s => s.NameExistsAsync(It.Is<string>(p => p == role.Name)), Times.Never);
            _roleCommandsMock.Verify(p => p.UpdateRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)), Times.Never);
        }

        [Test]
        public async Task Can_Delete_Role()
        {
            var role = CreateRoleFixture().Create<Role>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            var response = await _controller.DeleteRoleAsync(role.Id);

            response.Valid.Should().BeTrue();
            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleCommandsMock.Verify(p => p.DeleteRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)));
        }

        [Test]
        public async Task Cannot_Delete_Role_Not_Found()
        {
            var role = CreateRoleFixture().Create<Role>();

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult<Role>(null));

            var response = await _controller.DeleteRoleAsync(role.Id);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.NotFound);

            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleCommandsMock.Verify(p => p.DeleteRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)), Times.Never);
        }

        [Test]
        public async Task Cannot_Delete_Role_With_User_Used()
        {
            var role = CreateRoleFixture().Create<Role>();

            role.Users = new List<User> { new User() };

            _roleQueriesMock.Setup(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)))
                .Returns(Task.FromResult(role));

            var response = await _controller.DeleteRoleAsync(role.Id);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Role.CannotDeleteRole);

            _roleQueriesMock.Verify(s => s.GetRoleByIdAsync(It.Is<Guid>(p => p == role.Id)));
            _roleQueriesMock.Verify(s => s.NameExistsAsync(It.Is<string>(p => p == role.Name)), Times.Never);
            _roleCommandsMock.Verify(p => p.DeleteRoleAsync(It.Is<Role>(r =>
                r.Name == role.Name &&
                r.DisplayName == role.DisplayName &&
                r.Description == role.Description)), Times.Never);
        }


        private Fixture CreateRoleFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<Role>(u => u
                .With(s => s.Users, new User[] { }));
            return fixture;
        }

        private Fixture CreateMenuSettingFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<MenuSetting>(u => u
                .With(s => s.SubMenus, new MenuSetting[] { }));


            fixture.Customize<MenuSetting>(u => u
                .With(s => s.SubMenus, fixture.CreateMany<MenuSetting>(5).ToArray()));
            return fixture;
        }

        private Fixture CreateRoleRequestFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateRoleRequest>(u => u
                .With(s => s.Menus, new MenuSetting[] { }));
            return fixture;
        }

        private Fixture UpdateRoleRequestFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<UpdateRoleRequest>(u => u
                .With(s => s.Menus, new MenuSetting[] { }));
            return fixture;
        }
    }
}
