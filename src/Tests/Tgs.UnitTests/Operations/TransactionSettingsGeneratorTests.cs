using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Services.Operations.Validators;
using TGS.Tests.TestsCommon;

namespace TGS.UnitTests.Operations
{
    [TestFixture]
    [Category(Named.Unit)]
    [Parallelizable]
    public class TransactionSettingsGeneratorTests : NUnitTestsBase
    {
        private IAmountValidator _amountValidator;

        protected override void InitializeBeforeEachTest()
        {
            var amountValidatorMock = new Mock<IAmountValidator>();
            _amountValidator = amountValidatorMock.Object;
        }

        [TestCase(GameActionType.PlaceBet, 20, -20)]
        [TestCase(GameActionType.WinBet, 50, 50)]
        [TestCase(GameActionType.LoseBet, 20, 0)]
        [TestCase(GameActionType.FreeBet, 20, 20)]
        [TestCase(GameActionType.TieBet, 20, 20)]
        [TestCase(GameActionType.CancelTransaction, 20, -20)]
        [TestCase(GameActionType.WinJackpot, 50, 50)]
        public void Can_Get_Expected_Amount_By_GameActionType(GameActionType type, decimal inputAmount,
            decimal expectedAmount)
        {
            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(type);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }
    }
}