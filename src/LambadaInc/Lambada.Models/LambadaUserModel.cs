using Microsoft.WindowsAzure.Storage.Table;

namespace Lambada.Models
{
    public class LambadaUserModel : TableEntity
    {
        public string UserId
        {
            get => RowKey;
            set => RowKey = value;
        }
    
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}