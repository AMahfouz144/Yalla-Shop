using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace YallaShop.API.Helpers
{
    public class FilesHelper(IConfiguration configuration) : IFileHelper
    {
        public async Task<string?> UploadPhoto(IFormFile file)
        {

            if (file == null)
            {
                return null;
            }
            var imgPath = Path.Combine("UploadedPhotos", DateTime.Now.ToString("ddMMyyyyhhmm") + file.FileName);
            // Define the file path where the photo will be saved
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imgPath);

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save the file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "https://yallashop-api.runasp.net/" + imgPath.Replace("\\", "/");
        }
    }
}
