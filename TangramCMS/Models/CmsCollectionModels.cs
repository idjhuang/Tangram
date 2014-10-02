using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

    public class CmsDocument
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string CreatedDate { get; set; }
        public string LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        [BsonExtraElements]
        public BsonDocument ExtraElements { get; set; }
    }

    public class CmsDocumentReference
    {
        public string CollectionId { get; set; }
        public string DocumentId { get; set; }
    }

    public class CmsSelection
    {
        public string SelectionId { get; set; }
        public List<CmsSelectionItem> SelectionItemList { get; set; } 
    }

    public class CmsSelectionItem
    {
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
    }
}