using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Moq;
using NUnit.Framework;
using TGS.Apis.FileService.DataService;
using TGS.Apis.FileService.DataService.Model;
using TGS.Apis.FileService.DataService.Services;
using TGS.Core.Infrastructures.Aws;
using TGS.Tests.FileService.UnitTests.Controllers;

namespace TGS.Tests.FileService.UnitTests.Services
{
    public class UploadServiceTests : BaseTests
    {
        private readonly Mock<IAmazonS3> _amazonS3Mock = new();
        private readonly Mock<IRepository<File>> _repositoryMock = new();
        private readonly Mock<IRepository<StationLetterFile>> _stationLetterRepositoryMock = new();
        private AwsSettings _awsSettings;

        private UploadService _service;

        [SetUp]
        public void Setup()
        {
            _awsSettings = Customize<AwsSettings>(x => x)
                .Create<AwsSettings>();

            _service = new(
                _amazonS3Mock.Object, _awsSettings, _repositoryMock.Object,
                _stationLetterRepositoryMock.Object);
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

            _amazonS3Mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var url = await _service.UploadFileS3Async(file);

            _repositoryMock.Verify(
                c => c.InsertAsync(It.IsAny<File>()));

            Assert.IsTrue(url.IndexOf($"{_awsSettings.ServiceUrl}/{_awsSettings.S3.Bucket}", StringComparison.Ordinal) > -1);
        }

        private static Fixture Customize<T>(Func<ICustomizationComposer<T>, ISpecimenBuilder> composerTransformation)
            where T : new()
        {
            var fixture = new Fixture();
            fixture.Customize(composerTransformation);
            return fixture;
        }
    }
}