using System.Threading.Tasks;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Commands.BatchAdjust;

namespace TGS.Tests.BackendOffice.UnitTests;

public class BatchAdjustTests
{
    [Test]
    public async Task Export_Form_Template()
    {
        var sut = new ExportBatchAdjustFormTemplateCommandService();

        var command = new ExportBatchAdjustFormTemplateCommand();

        var actual = await sut.GetAsync(command);

        Assert.IsNotNull(actual);
    }
}
