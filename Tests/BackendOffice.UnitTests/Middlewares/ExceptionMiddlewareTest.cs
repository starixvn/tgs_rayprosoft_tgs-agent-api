using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Infrastructures.Log;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Services;

namespace BackendOffice.UnitTests.Middlewares
{
    [TestFixture, Parallelizable]
    public class ExceptionMiddlewareTest
    {
        private readonly Mock<IRequestAnalyzer> _requestAnalyzerMock = new Mock<IRequestAnalyzer>();
        private readonly Mock<ITgsLog> _tgsLogMock = new Mock<ITgsLog>();
        private readonly string _requestString = "Hello World Request!!";
        private readonly string _responseString = "Hello World Response!!";
        private RequestDelegate _requestDelegate;
        private IMiddleware _target;

        [SetUp]
        public void Setup()
        {
            _requestDelegate = (innerContext) => Task.FromResult(0);
            _target = new ExceptionMiddleware(_requestAnalyzerMock.Object, _tgsLogMock.Object);
        }

        [Test]
        public async Task Can_Invoke()
        {
            var httpContextMock = SetupHttpContext();
            Func<Task> action = () => _target.InvokeAsync(httpContextMock.Object, _requestDelegate);

            await action.Should().NotThrowAsync();

            _tgsLogMock.Verify(p => p.LogAdmin(It.Is<AdminLogEvent>(l =>
                l.requestcontent == _requestString &&
                l.responsecontent == _responseString)));
        }

        [Test]
        public async Task Can_Invoke_With_Exception()
        {
            var httpContextMock = SetupHttpContext(false);
            Func<Task> action = () => _target.InvokeAsync(httpContextMock.Object, _requestDelegate);

            await action.Should().NotThrowAsync();

            _tgsLogMock.Verify(p => p.LogAdmin(It.Is<AdminLogEvent>(l =>
                l.exception != string.Empty)));
        }

        private Mock<HttpContext> SetupHttpContext(bool hasNoException = true)
        {
            //https://andrewcampey.com/2017/12/07/c-memory-stream-is-not-expandable/
            // This CANNOT be extended   for error : memory stream is not expandable
            var streamRequest = new MemoryStream(Encoding.UTF8.GetBytes(_requestString));
            var streamResponse = new MemoryStream(); // // This CAN be extended
            var responseStringByteArray = Encoding.UTF8.GetBytes(_responseString);
            streamResponse.Write(responseStringByteArray, 0, responseStringByteArray.Length);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(p => p.Connection)
                .Returns(new Mock<ConnectionInfo>().Object);

            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpResponse.Setup(p => p.Cookies)
                .Returns(new Mock<IResponseCookies>().Object);
            mockHttpResponse.Setup(p => p.Body)
                .Returns(streamResponse);

            mockHttpResponse
                .Setup(_ => _.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Verifiable();

            if (hasNoException)
            {
                mockHttpResponse.SetupGet(p => p.Headers)
                    .Returns(new HeaderDictionary());
            }

            mockHttpContext.Setup(p => p.Response)
                .Returns(mockHttpResponse.Object);

            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(p => p.Body).Returns(streamRequest);
            mockHttpRequest.SetupGet(p => p.Headers)
                .Returns(new HeaderDictionary());

            mockHttpContext.Setup(p => p.Request)
                .Returns(mockHttpRequest.Object);

            var fc = new FeatureCollection();
            var endPoint = new Endpoint(c => Task.CompletedTask, new EndpointMetadataCollection(), "for unit test");
            var ehf = new ExceptionHandlerFeature();
            fc.Set(endPoint);
            fc.Set(ehf);

            mockHttpContext.Setup(p => p.Features)
                .Returns(fc);

            return mockHttpContext;
        }
    }
}
