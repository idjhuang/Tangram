using System.Collections.Generic;
using MongoDB.Bson;

namespace TangramCMS.Models
{
    public class CmsSelection : CmsDocumentBase
    {
        public string SelectionId { get; set; }
        public string DataType { get; set; }
        public List<CmsSelectionItem> ItemList { get; set; }
    }

    public class CmsSelectionItem
    {
        public string Name { get; set; }
        public BsonDocument Value { get; set; }
    }
}