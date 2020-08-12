using System.Collections.Generic;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class DictionaryExtensionsTests
    {
        private readonly Dictionary<int, int> _nullDic = null;

        [Test]
        public void Can_Execute_GetOr_When_Dic_Is_NULL()
        {
            _nullDic.GetOr(3, () => 2).Should().Be(2);
        }

        [Test]
        public void Can_Execute_GetOrDefault_When_Dic_Is_NULL()
        {
            _nullDic.GetOrDefault(3).Should().Be(0);
        }

        [Test]
        public void Can_Execute_GetOrCreate_When_Dic_Is_NULL()
        {
            _nullDic.GetOrCreate(3, () => 2).Should().Be(2);
        }

        private readonly Dictionary<int, int> _dic = new Dictionary<int, int>()
        {
            { 1, 101 },
            { 2, 203 }
        };

        [Test]
        public void Can_Get_From_GetOr_By_Key()
        {
            _dic.GetOr(2, () => 2).Should().Be(203);
        }

        [Test]
        public void Can_Get_From_GetOr_When_Can_Not_Find_Key()
        {
            _dic.GetOr(5, () => 9).Should().Be(9);
        }

        [Test]
        public void Can_Get_From_GetOrDefault_By_Key()
        {
            _dic.GetOrDefault(2).Should().Be(203);
        }

        [Test]
        public void Can_Get_From_GetOrDefault_When_Can_Not_Find_Key()
        {
            _dic.GetOrDefault(6).Should().Be(0);
        }

        [Test]
        public void Can_Get_From_GetOrCreate_By_Key()
        {
            _dic.GetOrCreate(1, () => 5).Should().Be(101);
        }

        [Test]
        public void Can_Get_From_GetOrCreate_When_Can_Not_Find_Key()
        {
            _dic.GetOrCreate(5, () => 9).Should().Be(9);
        }
    }
}