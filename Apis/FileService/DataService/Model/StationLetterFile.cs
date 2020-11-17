using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FileService.DataService.Model
{
    public class StationLetterFile : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual ICollection<string> Files { get; set; }

        [BsonIgnore] public ICollection<File> FileList { get; set; }
    }
}