using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace TangramCMS.Models
{
    public class CmsResultModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class CmsDocumentBase
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string CreatedDate { get; set; }
        public string LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class CmsDocument : CmsDocumentBase
    {
        public string ParentId { get; set; }
        [BsonExtraElements]
        public BsonDocument ExtraElements { get; set; }
    }

    public class CmsDocumentReference
    {
        public string CollectionId { get; set; }
        public string DocumentId { get; set; }
    }

    public class CmsCollection : CmsDocumentBase
    {
        public string CollectionId { get; set; }
        public string DocumentType { get; set; }
    }
}