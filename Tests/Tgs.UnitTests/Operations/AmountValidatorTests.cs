using System;
using FluentAssertions;
using NUnit.Framework;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations.Validators;
using TGS.Tests.TestsCommon;

namespace Tgs.UnitTests.Operations
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public sealed class AmountValidatorTests : NUnitTestsBase
    {
        private IAmountValidator _validator;
        private const string Name = "amount";

        protected override void InitializeBeforeEachTest()
        {
            _validator = new AmountValidator();
        }

        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(10)]
        [TestCase(100)]
        public void ThrowIfAmountIsNotPositive_No_Throws_When_Amount_Is_Positive(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNotPositive(Name, amount);
            action.Should().NotThrow();
        }

        [TestCase(0)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsNotPositive_Throws_InvalidAmountException_When_Amount_Is_Not_Positive(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNotPositive(Name, amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The {Name} must be positive but is {amount}");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(10)]
        [TestCase(100)]
        public void ThrowIfAmountIsNegative_No_Throws_When_Amount_Is_Not_Negative(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNegative(Name, amount);
            action.Should().NotThrow();
        }

        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsNegative_Throws_InvalidAmountException_When_Amount_Is_Not_Negative(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNegative(Name, amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The {Name} must be zero or positive but is {amount}");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void IgnoreAmount_No_Throws(decimal amount)
        {           
            Action action = () => _validator.IgnoreAmount(Name, amount);
            action.Should().NotThrow();
        }
    }
}
