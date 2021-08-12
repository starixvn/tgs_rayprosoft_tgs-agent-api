using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsLoginRequest
    {
        public string Token { get; init; }
        public string Lang { get; init; }
    }
}
