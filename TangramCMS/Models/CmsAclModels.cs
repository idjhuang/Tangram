namespace TangramCMS.Models
{
    public class CmsAclModel : CmsDocumentBase
    {
        public string CollectionId { get; set; }
        public string RoleName { get; set; }
        public string Rights { get; set; }
    }
}