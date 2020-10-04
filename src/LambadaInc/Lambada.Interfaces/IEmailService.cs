using System.Threading.Tasks;

namespace Lambada.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string from, string to,string subject, string body);
    }
}