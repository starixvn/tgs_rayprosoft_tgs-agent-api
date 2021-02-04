using MongoDB.Bson.Serialization.Attributes;

namespace TGS.Apis.FileService.DataService.Model
{
    public abstract class BaseEntity
    {
        [BsonId] public string Id { get; set; }
    }
}