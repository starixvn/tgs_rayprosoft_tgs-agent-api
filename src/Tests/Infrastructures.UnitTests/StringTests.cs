using NUnit.Framework;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;

namespace Infrastructures.UnitTests
{
    [TestFixture]
    public class StringTests
    {
        [TestCase("https://test/com/pic_trulli.jpg")]
        [TestCase(@"<img src=""https://test/com/pic_trulli.jpg"" alt=""Italian Trulli"">")]
        public void Can_Get_Image_NameTests(string url)
        {
            var imageName = url.GetMatchStrings(Consts.UploadFile.ImagePattern);

            Assert.AreEqual(imageName[0], "pic_trulli.jpg");
        }
    }
}