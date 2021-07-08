using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
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

        [TestCase(20, -20)]
        [TestCase(10, -10)]
        public void Can_Get_Expected_Amount_Of_PlaceBet(decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData {GameActionType = GameActionType.PlaceBet};

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }

        [TestCase(50, 50)]
        [TestCase(100, 100)]
        public void Can_Get_Expected_Amount_Of_WinBet(decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData {GameActionType = GameActionType.WinBet};

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }

        [TestCase(true, -50, -50)]
        [TestCase(true, 0, 0)]
        [TestCase(false, 0, 0)]
        [TestCase(false, -50, 0)]
        public void Can_Get_Expected_Amount_Of_LoseBet(bool allowNegativeAmount,
            decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData
            {
                GameActionType = GameActionType.LoseBet,
                AllowLoseBetNegativeAmount = allowNegativeAmount
            };

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }

        [TestCase(20, 20)]
        [TestCase(50, 50)]
        public void Can_Get_Expected_Amount_Of_TieBet(decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData {GameActionType = GameActionType.TieBet};

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }

        [TestCase(20, 20)]
        [TestCase(0, 0)]
        public void Can_Get_Expected_Amount_Of_FreeBet(decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData {GameActionType = GameActionType.FreeBet};

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }

        [TestCase(20, -20)]
        [TestCase(10, -10)]
        public void Can_Get_Expected_Amount_Of_CancelTransaction(decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData {GameActionType = GameActionType.CancelTransaction};

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }

        [TestCase(20, 20)]
        [TestCase(50, 50)]
        public void Can_Get_Expected_Amount_Of_WinJackpot(decimal inputAmount, decimal expectedAmount)
        {
            var transaction = new TransactionData {GameActionType = GameActionType.WinJackpot};

            var generator = (ITransactionSettingsGenerator)new TransactionSettingsGenerator(_amountValidator);
            var ts = generator.GetTransactionSettings(transaction);
            var convertedAmount = ts.GetTransactionAmount(inputAmount);

            convertedAmount.Should().Be(expectedAmount);
        }
    }
}