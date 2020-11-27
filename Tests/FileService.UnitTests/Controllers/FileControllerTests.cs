using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using TGS.Apis.FileService.Controllers;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.Apis.FileService.DataService.Services;

namespace TGS.Tests.FileService.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class FileControllerTests : BaseTests
    {
        [SetUp]
        public void Setup()
        {
            _controller = new FileController(
                _uploadServiceMock.Object);
        }

        private FileController _controller;
        private readonly Mock<IUploadService> _uploadServiceMock = new Mock<IUploadService>();

        [Test]
        public async Task Can_Upload()
        {
            CreateFile();
            var file = fileMock.Object;
            var request = new UploadRequest
            {
                File = file
            };
            var url = "http://uploadfile.tgs.com";

            _uploadServiceMock.Setup(p => p.UploadFileAsync(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(url));

            var response = await _controller.UploadFileAsync(request);

            Assert.IsTrue(response.Valid);
            Assert.AreEqual(response.Location, url);
        }

        [Test]
        public async Task Can_Upload_S3()
        {
            CreateFile();
            var file = fileMock.Object;
            var request = new UploadRequest
            {
                File = file
            };
            var url = "http://uploadfile.tgs.com";

            _uploadServiceMock.Setup(p => p.UploadFileS3Async(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(url));

            var response = await _controller.UploadFileS3Async(request);

            Assert.IsTrue(response.Valid);
            Assert.AreEqual(response.Location, url);
        }
    }
}