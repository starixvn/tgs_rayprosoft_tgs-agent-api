using System;

namespace TGS.FileService.Repositories.Model
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public decimal FileLength { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
