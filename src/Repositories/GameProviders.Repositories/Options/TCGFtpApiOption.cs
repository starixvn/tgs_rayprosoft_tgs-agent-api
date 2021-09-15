using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Repositories.Options
{
    public record TCGFtpApiOption
    {
        public Uri Url { get; init; }
    }
}
