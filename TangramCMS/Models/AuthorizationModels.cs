namespace TangramService.Models
{
    public class AuthorizationModel : DocumentBase
    {
        public string CollectionId { get; set; }
        public string RoleName { get; set; }
        public string Rights { get; set; }
    }
}