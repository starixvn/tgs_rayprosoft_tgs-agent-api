using MongoDB.Bson.Serialization.Attributes;

namespace TGS.FileService.Repositories.Model
{
    public abstract class BaseEntity
    {
        [BsonId] public string Id { get; set; }
    }
}
