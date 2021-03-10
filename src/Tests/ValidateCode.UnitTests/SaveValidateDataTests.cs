using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.ValidateCode;

namespace TGS.Tests.ValidateCode.UnitTests
{
    [TestFixture]
    public class SaveValidateDataTests
    {
        [Test]
        public async Task Save_Validate_Data_Should_Success()
        {
            var fakeValidateCodeStore = Substitute.For<IValidateCodeStore>();

            var sut = new SaveValidateDataCommandService(fakeValidateCodeStore);

            var validateCode = "123456";
            var token = "98FBCC27-C3F1-4479-AE0D-D68493853623";
            var content = "jsonContent";
            var command = new SaveValidateDataCommand(validateCode, token, content);

            await sut.ExecuteAsync(command);

            _ = fakeValidateCodeStore
                .Received(1)
                .SaveDataAsync(Arg.Is(validateCode), Arg.Is(token), Arg.Is(content));
        }
    }
}
