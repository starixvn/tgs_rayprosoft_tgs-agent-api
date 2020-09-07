using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Action action = () => _validator.ThrowIfAmountIsNotPositive(amount);
            action.Should().NotThrow();
        }

        [TestCase(0)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsNotPositive_Throws_InvalidAmountException_When_Amount_Is_Not_Positive(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNotPositive(amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The amount {amount} must be positive");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(10)]
        [TestCase(100)]
        public void ThrowIfAmountIsNegative_No_Throws_When_Amount_Is_Not_Negative(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNegative(amount);
            action.Should().NotThrow();
        }

        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void ThrowIfAmountIsNegative_Throws_InvalidAmountException_When_Amount_Is_Not_Negative(decimal amount)
        {           
            Action action = () => _validator.ThrowIfAmountIsNegative(amount);
            action.Should().Throw<InvalidAmountException>()
                .WithMessage($"Invalid Amount: The amount {amount} must be zero or positive");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.5)]
        [TestCase(-0.3)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void IgnoreAmount_No_Throws(decimal amount)
        {           
            Action action = () => _validator.IgnoreAmount(amount);
            action.Should().NotThrow();
        }
    }
}
