using Amazon.S3;
using Amazon.S3.Transfer;
using EntreLaunch.Interfaces.MediaIntf;

namespace EntreLaunch.Services.MediaSvc
{
    public class AwsS3Service : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsS3Service(IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AwsS3");

            var accessKey = awsSection["AccessKey"];
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new ArgumentNullException(nameof(accessKey));

            var secretKey = awsSection["SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentNullException(nameof(secretKey));

            var bucketName = awsSection["BucketName"];
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentNullException(nameof(bucketName));

            var regionName = awsSection["Region"];
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentNullException(nameof(regionName));

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            var region = Amazon.RegionEndpoint.GetBySystemName(regionName);

            _bucketName = bucketName;
            _s3Client = new AmazonS3Client(credentials, region);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName = "")
        {
            var key = GenerateFileKey(file.FileName, folderName);

            var request = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = key,
                BucketName = _bucketName,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead // أو Private لو تحب الروابط تكون مخفية
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request);

            return $"https://{_bucketName}.s3.amazonaws.com/{key}";
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

        #region PRIVATE METHODS
        private string GenerateFileKey(string originalFileName, string folderName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            return string.IsNullOrWhiteSpace(folderName)
                ? uniqueFileName
                : $"{folderName.TrimEnd('/')}/{uniqueFileName}";
        }
        #endregion
    }
}
