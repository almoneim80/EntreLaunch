namespace EntreLaunch.Interfaces.MediaIntf
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName = "");
        Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string folderName = "");
    }
}
