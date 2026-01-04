using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EntreLaunch.Interfaces.MediaIntf;

namespace EntreLaunch.Services.MediaSvc
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var azureSection = configuration.GetSection("AzureBlobStorage");
            if (azureSection == null)
                throw new ArgumentNullException(nameof(azureSection));

            var connectionString = azureSection["ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _containerName = azureSection["ContainerName"]!;
            if (string.IsNullOrWhiteSpace(_containerName))
                throw new ArgumentNullException(nameof(_containerName));

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName = "")
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = GenerateFileKey(file.FileName, folderName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            return blobClient.Uri.ToString();
        }

        public async Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string folderName = "")
        {
            var urls = new List<string>();

            foreach (var file in files)
            {
                var url = await UploadFileAsync(file, folderName);
                urls.Add(url);
            }

            return urls;
        }

        private string GenerateFileKey(string originalFileName, string folderName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            return string.IsNullOrWhiteSpace(folderName)
                ? uniqueFileName
                : $"{folderName.TrimEnd('/')}/{uniqueFileName}";
        }
    }
}
