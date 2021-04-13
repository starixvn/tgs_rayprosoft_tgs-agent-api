using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using ExpectedObjects;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TGS.FileService;

namespace TGS.Tests.FileService.UnitTests.Services
{
    [TestFixture]
    public class FileManagerTests
    {
        [Test]
        public async Task Save_File_Should_Success()
        {
            var options = new AwsOptions
            {
                ServiceUrl = "https://aws",
                Bucket = "bucket"
            };
            var moqAmazonS3 = new Mock<IAmazonS3>();
            var moqFileMetaRepository = new Mock<IFileMetaRepository>();

            var sut = new FileManager(
                new OptionsWrapper<AwsOptions>(options),
                moqAmazonS3.Object,
                moqFileMetaRepository.Object);

            var filePath = "/test/path/file.ext";
            var contentType = "testContentType";
            var fileLength = 1024L;
            using var fileStream = new MemoryStream();

            var expected = new FileMeta
            {
                Id = "testFileId",
                FileName = "test/path/file.ext",
                ContentType = contentType,
                FileLength = fileLength,
                CreatedOn = DateTimeOffset.UtcNow,
                ExternalUrl = $"{options.ServiceUrl}/{options.Bucket}/test/path/file.ext"
            };

            moqAmazonS3
                .Setup(fake => fake.PutObjectAsync(
                    It.IsAny<PutObjectRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PutObjectResponse()
                {
                    HttpStatusCode = HttpStatusCode.OK
                }));
            moqFileMetaRepository
                .Setup(fake => fake.AddFileMetaAsync(
                    It.IsAny<FileMeta>()))
                .Returns(ValueTask.FromResult(expected.Id));

            var actual = await sut.SaveAsync(filePath, contentType, fileLength, fileStream);

            expected.ToExpectedObject(ctx =>
            {
                ctx.Ignore(obj => obj.CreatedOn);
            }).ShouldMatch(actual);
        }

    }
}
