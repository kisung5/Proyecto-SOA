using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

namespace Sentiment_service.Services
{
    class CloudStorageService
    {
        private readonly string _AZURE_STORAGE_CONNECTION_STRING;
        private readonly string _CONTAINER_NAME;

        public CloudStorageService()
        {
            //string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\appsettings.json";
            string path = @"." + Path.DirectorySeparatorChar + "appsettings.json";

            StreamReader file = File.OpenText(path);
            JsonTextReader reader = new JsonTextReader(file);

            JObject settings = (JObject)JToken.ReadFrom(reader);

            JToken azureStorageConfig = settings.GetValue("AzureStorageConnection");
            _AZURE_STORAGE_CONNECTION_STRING = (string)azureStorageConfig["connection"];
            _CONTAINER_NAME = (string)azureStorageConfig["container"];
        }

        public async Task DownloadDocumentAsync(string fileName)
        {
            // Creates a local file in the ./Data/ directory for uploading and downloading
            //string localPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Data/";
            string localPath = @"." + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar;
            string localFilePath = Path.Combine(localPath, fileName);

            // Verifies if the file is already downloaded
            if (!File.Exists(localFilePath))
            {
                // Creates a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(_AZURE_STORAGE_CONNECTION_STRING);

                // Gets the container and return a container client object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_CONTAINER_NAME);

                // Gets a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                // Download the blob's contents and save it to a file
                BlobDownloadInfo download = await blobClient.DownloadAsync();

                using (FileStream downloadFileStream = System.IO.File.OpenWrite(localFilePath))
                {
                    await download.Content.CopyToAsync(downloadFileStream);
                    downloadFileStream.Close();
                }
            }
        }
    }
}
