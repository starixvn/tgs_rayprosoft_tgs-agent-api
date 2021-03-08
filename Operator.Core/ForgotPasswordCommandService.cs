using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valhalla.Services;

namespace TGS.Services.Operator
{
    class ForgotPasswordCommandService : ICommandService<ForgotPasswordCommand>
    {
        public ValueTask ExecuteAsync(ForgotPasswordCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
