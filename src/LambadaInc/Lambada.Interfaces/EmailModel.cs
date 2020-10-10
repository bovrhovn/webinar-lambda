namespace Lambada.Interfaces
{
    public class EmailModel : INotification
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}