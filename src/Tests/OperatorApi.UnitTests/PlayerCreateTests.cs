using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Validators;

namespace OperatorApi.UnitTests
{
    
    public class PlayerCreateTests
    {                
        [Test]
        public void Verify_Player_Account_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
            {
                Password = "123456",
                ConfirmPassword = "123456",
                Username = "user",
                AgentInvitationCode = "",
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);

            // assert            
            actual.ShouldHaveValidationErrorFor(request => request.Username);
        }

        [Test]
        public void Verify_Player_Account_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
            {
                Password = "123456",
                ConfirmPassword = "123456",
                Username = "123456789012345678901234567890123",
                AgentInvitationCode = "",
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert            
            actual.ShouldHaveValidationErrorFor(request => request.Username);
        }

        [Test]
        public void Verify_Player_Account_With_Special_Character()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
            {
                Password = "123456",
                ConfirmPassword = "123456",
                Username = "abc123_",
                AgentInvitationCode = "",
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert            
            actual.ShouldHaveValidationErrorFor(request => request.Username);
        }



        [Test]
        public void Verify_Player_Password_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
            {
                Password = "1234",
                ConfirmPassword = "1234",
                Username ="username",
                AgentInvitationCode="",
                CurrencyCode="CNY",
                IsTestPlayer = false
            };
            
            // act
            var actual = sut.TestValidate(request);
            // assert            
            actual.ShouldHaveValidationErrorFor(request => request.Password);                
        }

        [Test]
        public void Verify_Player_Password_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
            {
                Password = "123456789012345678901",
                ConfirmPassword = "123456789012345678901",
                Username = "username",
                AgentInvitationCode = "",
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert            
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }


    }
}
