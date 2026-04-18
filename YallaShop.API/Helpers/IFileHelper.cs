namespace YallaShop.API.Helpers
{
    public interface IFileHelper
    {
        Task<string?> UploadPhoto(IFormFile file);
    }
}
