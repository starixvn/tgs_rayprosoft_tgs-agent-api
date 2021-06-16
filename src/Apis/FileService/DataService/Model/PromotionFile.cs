using System;
using System.Collections.Generic;
using TGS.FileService.Repositories.Model;

namespace TGS.Apis.FileService.DataService.Model
{
    public class PromotionFile : BaseEntity
    {
        public string PromotionId { get; set; }

        public ICollection<string> Files { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}