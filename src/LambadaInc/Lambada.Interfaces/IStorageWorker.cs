using System.IO;
using System.Threading.Tasks;

namespace Lambada.Interfaces
{
    public interface IStorageWorker
    {
        string Container { get; set; }
        string ConnectionString { get; set; }
        Task<string> GetFileUrl(string name, bool validate);
        Task<bool> IsValidAsync(string name);
        Task<bool> UploadFileAsync(string name, Stream data);
        Task<bool> DeleteFileAsync(string name);
        Task<Stream> DownloadFileAsync(string name);
        Task<string> DownloadAsStringAsync(string name);
    }
}