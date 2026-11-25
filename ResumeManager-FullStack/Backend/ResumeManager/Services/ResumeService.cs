using ResumeManager.DTOs;
using ResumeManager.Helpers;
using ResumeManager.Models;
using ResumeManager.Repositories;

namespace ResumeManager.Services;

public class ResumeService : IResumeService
{
    private const long MaxFileSize = 10 * 1024 * 1024;
    private readonly IResumeRepository _repository;
    private readonly FileStorageHelper _fileStorage;

    public ResumeService(IResumeRepository repository, FileStorageHelper fileStorage)
    {
        _repository = repository;
        _fileStorage = fileStorage;
    }

    public async Task<List<ResumeDto>> GetAllAsync()
    {
        var resumes = await _repository.GetAllAsync();
        return resumes.Select(ToDto).ToList();
    }

    public async Task<ResumeDto?> GetByIdAsync(int id)
    {
        var resume = await _repository.GetByIdAsync(id);
        return resume == null ? null : ToDto(resume);
    }

    public async Task<ResumeDto> UploadAsync(IFormFile file)
    {
        ValidateFile(file);

        var storedFileName = await _fileStorage.SaveAsync(file);

        var resume = new Resume
        {
            FileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(resume);
        await _repository.SaveChangesAsync();

        return ToDto(resume);
    }

    public async Task<ResumeDto?> ReplaceAsync(int id, IFormFile file)
    {
        ValidateFile(file);

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            return null;
        }

        var newStoredFileName = await _fileStorage.SaveAsync(file);
        var oldStoredFileName = existing.StoredFileName;

        existing.FileName = Path.GetFileName(file.FileName);
        existing.StoredFileName = newStoredFileName;
        existing.ContentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;
        existing.FileSize = file.Length;
        existing.UploadedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        _fileStorage.Delete(oldStoredFileName);

        return ToDto(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var resume = await _repository.GetByIdAsync(id);
        if (resume == null)
        {
            return false;
        }

        await _repository.DeleteAsync(resume);
        await _repository.SaveChangesAsync();
        _fileStorage.Delete(resume.StoredFileName);

        return true;
    }

    public async Task<(string Path, string FileName, string ContentType)?> GetDownloadAsync(int id)
    {
        var resume = await _repository.GetByIdAsync(id);
        if (resume == null)
        {
            return null;
        }

        var path = _fileStorage.GetFullPath(resume.StoredFileName);
        if (!File.Exists(path))
        {
            return null;
        }

        return (path, resume.FileName, resume.ContentType);
    }

    private static void ValidateFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("Please select a file.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException("File size must not exceed 10 MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".pdf" && extension != ".docx")
        {
            throw new InvalidOperationException("Only PDF and DOCX files are allowed.");
        }
    }

    private static ResumeDto ToDto(Resume resume)
    {
        return new ResumeDto
        {
            ResumeId = resume.ResumeId,
            FileName = resume.FileName,
            ContentType = resume.ContentType,
            FileSize = resume.FileSize,
            UploadedAt = resume.UploadedAt
        };
    }
}
