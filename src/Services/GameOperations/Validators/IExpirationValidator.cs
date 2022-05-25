using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Validators;

public interface IExpirationValidator
{
    Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform = null);
}
