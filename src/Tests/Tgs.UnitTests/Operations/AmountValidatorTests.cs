using System;
using FluentAssertions;
using NUnit.Framework;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations.Validators;
using TGS.Tests.TestsCommon;

namespace TGS.UnitTests.Operations
{
    [TestFixture]
    [Category(Named.Unit)]
    [Parallelizable]
    public sealed class AmountValidatorTests : NUnitTestsBase
    {
        private IAmountValidator _validator;
        private const string _Name = "amount";

        protected override void InitializeBeforeEachTest() => _validator = new AmountValidator();

        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(10)]
        [TestCase(100)]
        public void ThrowIfAmountIsNotPositive_No_Throws_When_Amount_Is_Positive(decimal amount)
        {
            Action action = () => _validator.ThrowIfAmountIsNotPositive(_Name, amount);
            action.Should().NotThrow();
        }

        [TestCase(0)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsNotPositive_Throws_InvalidAmountException_When_Amount_Is_Not_Positive(decimal amount)
        {
            Action action = () => _validator.ThrowIfAmountIsNotPositive(_Name, amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The {_Name} must be positive but is {amount}");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(10)]
        [TestCase(100)]
        public void ThrowIfAmountIsNegative_No_Throws_When_Amount_Is_Not_Negative(decimal amount)
        {
            Action action = () => _validator.ThrowIfAmountIsNegative(_Name, amount);
            action.Should().NotThrow();
        }

        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsNegative_Throws_InvalidAmountException_When_Amount_Is_Not_Negative(decimal amount)
        {
            Action action = () => _validator.ThrowIfAmountIsNegative(_Name, amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The {_Name} must be zero or positive but is {amount}");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void IgnoreAmount_No_Throws(decimal amount)
        {
            Action action = () => _validator.IgnoreAmount(_Name, amount);
            action.Should().NotThrow();
        }

        [TestCase(0)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsPositive_No_Throws_When_Amount_Is_Not_Positive(decimal amount)
        {
            Action action = () => _validator.ThrowIfAmountIsPositive(_Name, amount);
            action.Should().NotThrow();
        }

        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(10)]
        [TestCase(100)]
        public void ThrowIfAmountIsPositive_Throws_InvalidAmountException_When_Amount_Is_Positive(decimal amount)
        {
            Action action = () => _validator.ThrowIfAmountIsPositive(_Name, amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The {_Name} must be zero or negative but is {amount}");
        }
    }
}