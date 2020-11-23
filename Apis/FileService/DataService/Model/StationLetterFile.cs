using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TGS.Apis.FileService.DataService.Model
{
    public class StationLetterFile : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ICollection<string> Files { get; set; }

        [BsonIgnore] public ICollection<File> FileList { get; set; }
    }
}