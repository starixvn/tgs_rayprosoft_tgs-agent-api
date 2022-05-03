using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Validators
{
    public interface IExpirationValidator
    {
        Task ThrowIfAgentLoginExpiredAsync(Guid loginRecordId);

        Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform = null);
    }
}
