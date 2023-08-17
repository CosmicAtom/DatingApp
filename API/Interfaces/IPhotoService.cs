using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {   
        Task<ImageUploadResult> AddPhotoAsync(IFormFile photoFile);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
