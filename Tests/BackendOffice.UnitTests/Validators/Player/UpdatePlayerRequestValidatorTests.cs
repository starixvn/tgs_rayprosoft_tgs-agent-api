using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.Controllers.Messages.Validators.Player;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Player
{
    [TestFixture]
    public class UpdatePlayerRequestValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new UpdatePlayerRequestValidator();
        }

        private UpdatePlayerRequestValidator _validator;

        [Test]
        public void Should_have_error_real_name_is_null()
        {
            var model = new UpdatePlayerRequest {RealName = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.RealName)
                .WithErrorMessage(Apis.BackendOffice.Resources.User.NameRequired);
        }

        [Test]
        public void Should_not_have_error_when_real_name_is_specified()
        {
            var model = new UpdatePlayerRequest {RealName = "RealName"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.RealName);
        }
    }
}