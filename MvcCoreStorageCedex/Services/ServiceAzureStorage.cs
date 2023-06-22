using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcCoreStorageCedex.Models;

namespace MvcCoreStorageCedex.Services
{
    public class ServiceAzureStorage
    {
        private BlobServiceClient client;

        public ServiceAzureStorage(BlobServiceClient client)
        {
            this.client = client;
        }

        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem item in this.client.GetBlobContainersAsync())
            {
                containers.Add(item.Name);
            }
            return containers;
        }

        public async Task CreateContainerAsync(string containerName)
        {
            containerName = containerName.ToLower();
            await this.client.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            //RECUPERAMOS EL CLIENTE DE CONTAINERS
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                //SI QUEREMOS LAS PROPIEDADES DEL BLOBITEM, NECESITAMOS UN 
                //CLIENTE DE BLOBS
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.ContainerName = containerName;
                blob.Url = blobClient.Uri.AbsoluteUri;
                models.Add(blob);
            }
            return models;
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }

        public async Task UploadBlobAsync
            (string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
