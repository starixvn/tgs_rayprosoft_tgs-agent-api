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
        public void Should_not_have_error_when_password_is_null()
        {
            var model = new UpdatePlayerRequest {Password = null};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.Password);
        }

        [Test]
        public void Should_have_error_password_length_is_not_greater_than_or_equal_to_eight()
        {
            var model = new UpdatePlayerRequest {Password = "Pswd", ConfirmedPassword = "Pswd"};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Password)
                .WithErrorMessage(Apis.BackendOffice.Resources.User.PasswordLengthAtLeast);
        }

        [Test]
        public void Should_not_have_error_when_password_have_not_number()
        {
            var model = new UpdatePlayerRequest {Password = "P@ssword", ConfirmedPassword = "P@ssword"};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Password)
                .WithErrorMessage(Apis.BackendOffice.Resources.User.PasswordNeedNumber);
        }

        [Test]
        public void Should_have_error_password_is_not_equal_to_confirmed_password()
        {
            var model = new UpdatePlayerRequest {Password = "Password", ConfirmedPassword = "ConfirmedPassword"};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Password)
                .WithErrorMessage(Apis.BackendOffice.Resources.User.PasswordNotMatched);
        }

        [Test]
        public void Should_not_have_error_when_password_is_equal_to_confirmed_password()
        {
            var model = new UpdatePlayerRequest {Password = "password1", ConfirmedPassword = "password1"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.Password);
        }
    }
}