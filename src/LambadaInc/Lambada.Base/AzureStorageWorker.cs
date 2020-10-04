using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Microsoft.WindowsAzure.Storage;

namespace Lambada.Base
{
    public class AzureStorageWorker : IStorageWorker
    {
        readonly CloudStorageAccount storageAccount;

        public AzureStorageWorker(string connectionString,string container)
        {
            Container = container;
            ConnectionString = connectionString;
            storageAccount = CloudStorageAccount.Parse(ConnectionString);
        }

        public string Container { get; set; }

        public string ConnectionString { get; set; }

        public async Task<string> GetFileUrl(string name, bool validate)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            if (!validate) return $"{storageAccount.BlobEndpoint.AbsoluteUri}{Container}/{name}";
            
            if (await IsValidAsync(name))
                return $"{storageAccount.BlobEndpoint.AbsoluteUri}{Container}/{name}";
            
            return string.Empty;
        }

        public async Task<bool> IsValidAsync(string name)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(Container);

            var blockBlob = container.GetBlockBlobReference(name);
            if (blockBlob == null) return false;

            return await blockBlob.ExistsAsync().ConfigureAwait(false);
        }

        public async Task<bool> UploadFileAsync(string name, Stream data)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(Container);

                var blockBlob = container.GetBlockBlobReference(name);

                if (data == null) return false;
                
                //data.Position = 0; // set the pointer to start
                await blockBlob.UploadFromStreamAsync(data).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteFileAsync(string name)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(Container);

                var blockBlob = container.GetBlockBlobReference(name);

                if (blockBlob == null) return false;

                await blockBlob.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<Stream> DownloadFileAsync(string name)
        {
            try
            {
                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(Container);

                var blockBlob = container.GetBlockBlobReference(name);

                var ms = new MemoryStream();
                await blockBlob.DownloadToStreamAsync(ms).ConfigureAwait(false);

                ms.Position = 0;
                return ms;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<string> DownloadAsStringAsync(string name)
        {
            try
            {
                await using var stream = await DownloadFileAsync(name).ConfigureAwait(false);
                var content = ((MemoryStream) stream).ToArray();
                return Encoding.UTF8.GetString(content);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
}