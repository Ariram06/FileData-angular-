namespace ResumeManager.Helpers;

public class FileStorageHelper
{
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] AllowedExtensions = [".pdf", ".docx"];

    public FileStorageHelper(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    private string UploadFolder
    {
        get
        {
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            return Path.Combine(webRoot, "uploads", "resumes");
        }
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only PDF and DOCX files are allowed.");
        }

        Directory.CreateDirectory(UploadFolder);

        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(UploadFolder, storedFileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew);
        await file.CopyToAsync(stream);

        return storedFileName;
    }

    public string GetFullPath(string storedFileName)
    {
        return Path.Combine(UploadFolder, storedFileName);
    }

    public void Delete(string storedFileName)
    {
        var fullPath = GetFullPath(storedFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
