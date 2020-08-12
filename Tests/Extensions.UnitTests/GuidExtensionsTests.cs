using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class GuidExtensionsTests
    {
        [TestCase(false)]
        public void Can_Get_String_Without_Hyphens(bool expected)
        {
            Guid.NewGuid().ToStringWithoutHyphens().Contains("-").Should().Be(expected);
        }

        [TestCase(false)]
        public void Can_Execute_ToUpperString(bool expected)
        {
            Guid.NewGuid().ToUpperString().Any(char.IsLower).Should().Be(expected);
        }

        [TestCase(1000)]
        public void Can_Create_SequentialGuid(int count)
        {
            var guids = Enumerable.Range(0, count).Select(_ => GuidExtensions.NewSequentialGuid()).ToHashSet();
            guids.Count.Should().Be(count);
        }
    }
}