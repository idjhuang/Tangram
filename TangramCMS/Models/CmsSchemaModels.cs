using System.Collections.Generic;

namespace TangramCMS.Models
{
    public class CmsDocumentType : CmsDocumentBase
    {
        public string DocumentType { get; set; }
        public List<CmsField> FiledList { get; set; }
    }

    public class CmsField
    {
        public string FieldName { get; set; }
        public string FiledType { get; set; }
    }

    public class CmsSelection : CmsDocumentBase
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