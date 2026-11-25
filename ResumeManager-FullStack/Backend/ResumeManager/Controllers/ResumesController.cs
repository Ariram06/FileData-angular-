using Microsoft.AspNetCore.Mvc;
using ResumeManager.DTOs;
using ResumeManager.Services;

namespace ResumeManager.Controllers;

[ApiController]
[Route("api/resumes")]
public class ResumesController : ControllerBase
{
    private readonly IResumeService _resumeService;

    public ResumesController(IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ResumeDto>>> GetAll()
    {
        return Ok(await _resumeService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResumeDto>> GetById(int id)
    {
        var resume = await _resumeService.GetByIdAsync(id);
        return resume == null ? NotFound(new { message = "Resume not found." }) : Ok(resume);
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ResumeDto>> Upload([FromForm] ResumeUploadDto request)
    {
        try
        {
            if (request.File == null)
            {
                return BadRequest(new { message = "Please select a file." });
            }

            var result = await _resumeService.UploadAsync(request.File);
            return CreatedAtAction(nameof(GetById), new { id = result.ResumeId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ResumeDto>> Replace(int id, [FromForm] ResumeUploadDto request)
    {
        try
        {
            if (request.File == null)
            {
                return BadRequest(new { message = "Please select a file." });
            }

            var result = await _resumeService.ReplaceAsync(id, request.File);
            return result == null
                ? NotFound(new { message = "Resume not found." })
                : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _resumeService.GetDownloadAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Resume or stored file not found." });
        }

        return PhysicalFile(result.Value.Path, result.Value.ContentType, result.Value.FileName);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _resumeService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound(new { message = "Resume not found." });
    }
}
