using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var content = "jsonContent";
            var command = new SaveValidateDataCommand(validateCode, content);

            await sut.ExecuteAsync(command);

            _ = fakeValidateCodeStore
                .Received(1)
                .SaveDataAsync(Arg.Is(validateCode), Arg.Is(content));
        }
    }
}
