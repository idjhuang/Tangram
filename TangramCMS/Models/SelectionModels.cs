using System.Collections.Generic;
using MongoDB.Bson;

namespace TangramService.Models
{
    public class Selection : DocumentBase
    {
        public string SelectionId { get; set; }
        public string DataType { get; set; }
        public List<SelectionItem> ItemList { get; set; }
    }

    public class SelectionItem
    {
        public string Name { get; set; }
        public BsonDocument Value { get; set; }
    }
}