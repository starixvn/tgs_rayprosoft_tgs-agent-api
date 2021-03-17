using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator.PlayerPhone
{
    public record CommitSetPlayerPhoneCommand(string PlayerCode, string SecurityKey, string ValidateCode);
}
