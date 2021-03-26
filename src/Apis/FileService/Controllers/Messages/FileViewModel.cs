using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.Apis.FileService.Controllers.Messages
{
    public class FileViewModel
    {
        public string Id { get; init; }

        public string FileName { get; init; }

        public string ContentType { get; init; }

        public decimal FileLength { get; init; }

        public DateTimeOffset CreatedOn { get; init; }

        public string ExternalUrl { get; init; }
    }
}
