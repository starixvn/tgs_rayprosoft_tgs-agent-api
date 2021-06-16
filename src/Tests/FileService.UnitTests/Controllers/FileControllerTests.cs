using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using TGS.Apis.FileService.Controllers;
using TGS.Apis.FileService.DataService.Services;
using TGS.FileService;

namespace TGS.Tests.FileService.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class FileControllerTests : BaseTests
    {
        [SetUp]
        public void Setup() =>
            _controller = new(
                _uploadServiceMock.Object,
                _stationLetterServiceMock.Object,
                _promotionServiceMock.Object);

        private FileController _controller;
        private readonly Mock<IUploadService> _uploadServiceMock = new();
        private readonly Mock<IStationLetterService> _stationLetterServiceMock = new();
        private readonly Mock<IPromotionService> _promotionServiceMock = new();

        [Test]
        public async Task Can_Upload_S3()
        {
            CreateFile();
            var file = fileMock.Object;

            const string prefix = "prefix";
            var fixture = new Fixture();
            var fileMeta = fixture.Create<FileMeta>();

            _uploadServiceMock.Setup(p => p.UploadFileS3Async(prefix, It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(fileMeta));

            var response = await _controller.UploadFileAsync(prefix, file);

            Assert.AreEqual(response.ExternalUrl, fileMeta.ExternalUrl);
        }
    }
}