using ResumeManager.DTOs;

namespace ResumeManager.Services;

public interface IResumeService
{
    Task<List<ResumeDto>> GetAllAsync();
    Task<ResumeDto?> GetByIdAsync(int id);
    Task<ResumeDto> UploadAsync(IFormFile file);
    Task<ResumeDto?> ReplaceAsync(int id, IFormFile file);
    Task<bool> DeleteAsync(int id);
    Task<(string Path, string FileName, string ContentType)?> GetDownloadAsync(int id);
}
