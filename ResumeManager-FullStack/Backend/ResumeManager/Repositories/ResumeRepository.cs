using Microsoft.EntityFrameworkCore;
using ResumeManager.Data;
using ResumeManager.Models;

namespace ResumeManager.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly AppDbContext _context;

    public ResumeRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Resume>> GetAllAsync()
    {
        return _context.Resumes
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync();
    }

    public Task<Resume?> GetByIdAsync(int id)
    {
        return _context.Resumes.FirstOrDefaultAsync(x => x.ResumeId == id);
    }

    public async Task AddAsync(Resume resume)
    {
        await _context.Resumes.AddAsync(resume);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Resume resume)
    {
        _context.Resumes.Remove(resume);
        return Task.CompletedTask;
    }
}
