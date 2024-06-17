using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using Microsoft.AspNetCore.Components.Forms;
using ModelLib;
using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using System.IO;
using System.Net.NetworkInformation;

namespace RazorLib.Models
{
    public class CloudClient
    {
        public async Task UploadFile(IBrowserFile file, string blobPath, Action<long> onProgressUpdate)
        {
            var uriSplit = blobPath.Split("?");
            var uri = new Uri(uriSplit.First());
            var sasToken = uriSplit.Last();

            var client = new BlobClient(uri, credential: new AzureSasCredential(sasToken));
            await client.UploadAsync(file.OpenReadStream(MediaUtils.FORTY_MEGABYTES_IN_BYTES), new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType,
                    CacheControl = "no-cache",
                },
                TransferOptions = new StorageTransferOptions
                {
                    InitialTransferSize = 1024 * 1024,
                    MaximumConcurrency = 10,
                    MaximumTransferSize = MediaUtils.FORTY_MEGABYTES_IN_BYTES,
                },
                ProgressHandler = new Progress<long>(onProgressUpdate)
            });

        }
    }

}
