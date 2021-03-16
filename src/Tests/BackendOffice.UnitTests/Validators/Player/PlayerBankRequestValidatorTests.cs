using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Models.PlayerBank;
using TGS.Services.BackOffice.Validation.Player;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Player
{
    [TestFixture]
    public class PlayerBankRequestValidatorTests
    {
        [SetUp]
        public void Setup() => _validator = new();

        private PlayerBankRequestValidator _validator;

        [Test]
        public void Should_have_error_when_username_is_null()
        {
            var model = new PlayerBankRequest {RealName = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.RealName)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Player.BankRealNameRequired);
        }

        [Test]
        public void Should_not_have_error_when_username_is_specified()
        {
            var model = new PlayerBankRequest {RealName = "Username"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.RealName);
        }

        [Test]
        public void Should_have_error_when_bank_name_is_null()
        {
            var model = new PlayerBankRequest {BankName = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.BankName)
                .WithErrorMessage(Services.BackOffice.Application.Resources.Player.BankNameRequired);
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
                .WithErrorMessage(Services.BackOffice.Application.Resources.Player.BankAccountRequired);
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