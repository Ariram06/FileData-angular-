using Microsoft.AspNetCore.Http;

namespace ResumeManager.DTOs;

public class ResumeUploadDto
{
    public IFormFile? File { get; set; }
}
