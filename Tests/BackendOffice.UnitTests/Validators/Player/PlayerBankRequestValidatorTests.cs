using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.Controllers.Messages.Validators.Player;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Player
{
    [TestFixture]
    public class PlayerBankRequestValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new PlayerBankRequestValidator();
        }

        private PlayerBankRequestValidator _validator;

        [Test]
        public void Should_have_error_when_username_is_null()
        {
            var model = new PlayerBankRequest {Username = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.Username)
                .WithErrorMessage(Apis.BackendOffice.Resources.Player.BankUsernameRequired);
        }

        [Test]
        public void Should_not_have_error_when_username_is_specified()
        {
            var model = new PlayerBankRequest {Username = "Username"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.Username);
        }

        [Test]
        public void Should_have_error_when_bank_name_is_null()
        {
            var model = new PlayerBankRequest {BankName = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.BankName)
                .WithErrorMessage(Apis.BackendOffice.Resources.Player.BankNameRequired);
        }

        [Test]
        public void Should_not_have_error_when_bank_name_is_specified()
        {
            var model = new PlayerBankRequest {BankName = "BankName"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.BankName);
        }

        [Test]
        public void Should_have_error_when_bank_account_is_null()
        {
            var model = new PlayerBankRequest {BankAccount = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.BankAccount)
                .WithErrorMessage(Apis.BackendOffice.Resources.Player.BankAccountRequired);
        }

        [Test]
        public void Should_not_have_error_when_bank_account_is_specified()
        {
            var model = new PlayerBankRequest {BankAccount = "BankAccount"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.BankAccount);
        }
    }
}