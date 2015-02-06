using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace TangramService.Models
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class DocumentBase
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string CreatedDate { get; set; }
        public string LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class Document : DocumentBase
    {
        public string ParentId { get; set; }
        [BsonExtraElements]
        public BsonDocument ExtraElements { get; set; }
    }

    public class Collection : DocumentBase
    {
        public string CollectionId { get; set; }
        public string DocumentType { get; set; }
        public bool IsTree { get; set; }
    }
}