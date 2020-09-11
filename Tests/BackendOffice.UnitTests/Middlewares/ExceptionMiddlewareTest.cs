using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Infrastructures.Log;
using TGS.Infrastructures.Log.Services;

namespace BackendOffice.UnitTests.Middlewares
{
    [TestFixture, Parallelizable]
    public class ExceptionMiddlewareTest
    {
        private ExceptionMiddleware middleware;
        private readonly Mock<IRequestAnalyzer> _requestAnalyzerMock = new Mock<IRequestAnalyzer>();
        private readonly Mock<ITgsLog> _tgsLogMock = new Mock<ITgsLog>();
        private readonly Mock<HttpContext> _httpContextMock = new Mock<HttpContext>();
        private readonly Mock<RequestDelegate> _requestDelegateMock = new Mock<RequestDelegate>();

        [SetUp]
        public void Setup()
        {
            middleware = new ExceptionMiddleware(_requestAnalyzerMock.Object, _tgsLogMock.Object);
        }

        public async Task Can_Invoke()
        {

            //var target = new ExceptionMiddleware((innerHttpContext) => null);

            //RequestDelegate next = (HttpContext hc) => Task.CompletedTask;

            //var httpContext = new DefaultHttpContext();
            //httpContext.Response.Body = new MemoryStream();
            //httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            //var expect = new InvalidOperationException("Missing header !!");

            ////act
            //Func<Task> action = () => target.InvokeAsync(httpContext, next);

            //await action.Should().NotThrowAsync();
        }
    }
}
