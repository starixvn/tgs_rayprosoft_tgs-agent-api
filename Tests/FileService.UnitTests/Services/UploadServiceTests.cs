using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TGS.Apis.FileService;
using TGS.Apis.FileService.DataService;
using TGS.Apis.FileService.DataService.Model;
using TGS.Apis.FileService.DataService.Services;
using TGS.Tests.FileService.UnitTests.Controllers;

namespace TGS.Tests.FileService.UnitTests.Services
{
    public class UploadServiceTests : BaseTests
    {
        private readonly Mock<IAmazonS3> _amazonS3Mock = new Mock<IAmazonS3>();
        private readonly Mock<IOptions<AwsSettings>> _awsSettingsMock = new Mock<IOptions<AwsSettings>>();
        private readonly Mock<IOptions<FileSettings>> _fileSettingsMock = new Mock<IOptions<FileSettings>>();
        private readonly Mock<IRepository<File>> _repositoryMock = new Mock<IRepository<File>>();

        private UploadService _service;

        [SetUp]
        public void Setup()
        {
            _service = new UploadService(
                _amazonS3Mock.Object, _awsSettingsMock.Object, _fileSettingsMock.Object, _repositoryMock.Object);
        }

        [Test]
        public async Task Can_Upload()
        {
            CreateFile();
            var file = fileMock.Object;
            var response = new PutObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };
            var settings = Customize<FileSettings>(x => x).Create<FileSettings>();

            _fileSettingsMock.Setup(x => x.Value).Returns(settings);

            var url = await _service.UploadFileAsync(file);

            _repositoryMock.Verify(
                c => c.InsertAsync(It.IsAny<File>()), Times.Once);

            Assert.IsTrue(url.IndexOf($"{settings.Path}", StringComparison.Ordinal) > -1);
        }

        [Test]
        public async Task Can_Upload_S3()
        {
            CreateFile();
            var file = fileMock.Object;
            var response = new PutObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };
            var settings = Customize<AwsSettings>(x => x).Create<AwsSettings>();
            var request = new PutObjectRequest
            {
                Key = It.IsAny<string>(),
                InputStream = file.OpenReadStream()
            };

            _awsSettingsMock.Setup(x => x.Value).Returns(settings);
            _amazonS3Mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var url = await _service.UploadFileS3Async(file);

            _repositoryMock.Verify(
                c => c.InsertAsync(It.IsAny<File>()));

            Assert.IsTrue(url.IndexOf($"{settings.ServiceUrl}/{settings.S3.Bucket}", StringComparison.Ordinal) > -1);
        }

        private Fixture Customize<T>(Func<ICustomizationComposer<T>, ISpecimenBuilder> composerTransformation)
            where T : new()
        {
            var fixture = new Fixture();
            fixture.Customize(composerTransformation);
            return fixture;
        }
    }
}