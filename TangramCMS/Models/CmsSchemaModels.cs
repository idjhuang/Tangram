using System.Collections.Generic;

namespace TangramCMS.Models
{
    public class CmsDocumentType : CmsDocumentBase
    {
        public string DocumentType { get; set; }
        public List<CmsProperty> PropertyList { get; set; }
        public string LastBuild { get; set; }
    }

    public class CmsProperty
    {
        public string PropertyName { get; set; }
        public string DataType { get; set; }
        public bool IsArray { get; set; }
    }

    public class CmsDocumentReference
    {
        public string CollectionId { get; set; }
        public string DocumentId { get; set; }
    }
}