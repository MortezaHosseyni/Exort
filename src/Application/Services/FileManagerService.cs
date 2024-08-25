using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IFileManagerService
    {
        string SaveFileAndReturnName(IFormFile file, string savePath);
    }
    public class FileManagerService : IFileManagerService
    {
        public string SaveFileAndReturnName(IFormFile file, string savePath)
        {
            if (file == null)
                throw new Exception("File cannot be null!");

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), savePath);

            if (!Equals(!Directory.Exists(folderPath)))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }
    }
}
