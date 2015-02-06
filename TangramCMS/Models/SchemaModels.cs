using System.Collections.Generic;
using MongoDB.Bson;

namespace TangramService.Models
{
    public class DocumentType : DocumentBase
    {
        public string DocumentTypeName { get; set; }
        public List<Property> PropertyList { get; set; }
        public string LastBuild { get; set; }
    }

    public class Property
    {
        public string PropertyName { get; set; }
        public string DataType { get; set; }
        public bool IsArray { get; set; }
        public bool IsSelection { get; set; }
        public List<PropertyAttribute> PropertyAttributes { get; set; } 
    }

    public class PropertyAttribute
    {
        public string Name { get; set; }
        public BsonDocument Value { get; set; }
    }

    public class DataType : DocumentBase
    {
        public string DataTypeName { get; set; }
        public List<DataTypeAttribute> DataTypeAttributes { get; set; }
    }

    public class DataTypeAttribute
    {
        public string Name { get; set; }
        public BsonDocument Value { get; set; }
    }

    public class DocumentReference
    {
        public string CollectionId { get; set; }
        public string DocumentId { get; set; }
    }
}