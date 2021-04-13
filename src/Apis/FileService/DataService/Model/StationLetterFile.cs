using System;
using System.Collections.Generic;
using TGS.FileService.Repositories.Model;

namespace TGS.Apis.FileService.DataService.Model
{
    public class StationLetterFile : BaseEntity
    {
        public string StationLetterId { get; set; }

        public ICollection<string> Files { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}
