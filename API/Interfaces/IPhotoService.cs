using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
  public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsnyc(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}