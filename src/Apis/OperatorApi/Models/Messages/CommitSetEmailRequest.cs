using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record CommitSetEmailRequest
    {
        public string Token { get; init; }
        public string ValidateCode { get; init; }
        public string Email { get; init; }
    }
}
