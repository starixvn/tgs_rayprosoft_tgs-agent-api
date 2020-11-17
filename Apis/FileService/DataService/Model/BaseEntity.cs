using MongoDB.Bson.Serialization.Attributes;

namespace FileService.DataService.Model
{
    public abstract class BaseEntity
    {
        [BsonId] public string Id { get; set; }
    }
}