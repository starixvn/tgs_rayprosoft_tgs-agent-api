using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class ArrayExtensionsTests
    {
        private readonly int[] _arrayForInts = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        [Test]
        public void Can_Get_Error_When_Number_Is_0()
        {
            Assert.Throws(typeof(ArgumentException), () => _arrayForInts.GetPartitions(0));
        }

        [Test]
        public void Can_Get_Error_When_index_Is_Smaller_Than_0()
        {
            var result = _arrayForInts.GetPartitions(2);

            foreach (var it in result)
            {
                Assert.Throws(typeof(InvalidOperationException), () => it.Current.Should().Be(_arrayForInts[0]));

                break;
            }
        }

        [TestCase(3)]
        public void Can_Execute_GetPartitions(int number)
        {
            var verifyCount = _arrayForInts.Count();
            var totalCount = 0;

            var result = _arrayForInts.GetPartitions(number);

            foreach (var it in result)
            {
                if (totalCount == 0)
                {
                    it.Reset();
                }

                while (it.MoveNext())
                {
                    it.Current.Should().BeLessThan(11);
                    totalCount++;
                }
            }

            totalCount.Should().Be(verifyCount);
        }

        [Test]
        public void Can_Get_Error_When_Array_Is_Null()
        {
            Assert.Throws(typeof(NullReferenceException), () => ((int[])null).GetPartitions(2));
        }

        [Test]
        public async Task Can_Execute_ForEachAsync_When_dop_is_1()
        {
            var listVerifyForEachAsync = new ConcurrentBag<int>();
            var token = CancellationToken.None;

            await _arrayForInts.ForEachAsync(1, token,
                (val, tokenparameter) =>
                {
                    listVerifyForEachAsync.Add(val);
                    return Task.FromResult(0);
                }
            );

            _arrayForInts.Should().BeEquivalentTo(listVerifyForEachAsync);
        }

        [TestCase(2)]
        public async Task Can_Execute_ForEachAsync(int dop)
        {
            var listVerifyForEachAsync = new ConcurrentBag<int>();
            var token = CancellationToken.None;

            await _arrayForInts.ForEachAsync(dop, token,
                (val, tokenparameter) =>
                {
                    listVerifyForEachAsync.Add(val);
                    return Task.FromResult(0);
                }
            );

            _arrayForInts.Should().BeEquivalentTo(listVerifyForEachAsync);
        }

        [TestCase(3)]
        public async Task Can_Execute_ForEachAsync_With_initShared(int dop)
        {
            var listVerifyForEachAsync = new ConcurrentBag<int>();
            var token = CancellationToken.None;
            var shared = 0;

            var returnFromForEachAsync = await _arrayForInts.ForEachAsync(dop, token,
                () => ++shared,
                (val, val2, tokenparameter) =>
                {
                    listVerifyForEachAsync.Add(val);
                    return Task.FromResult(val2);
                },
                Substitute.For<Action<int>>()
                );

            _arrayForInts.Should().BeEquivalentTo(listVerifyForEachAsync);
            shared.Should().Be(returnFromForEachAsync.Count());
        }
    }
}