using ResumeManager.Models;

namespace ResumeManager.Repositories;

public interface IResumeRepository
{
    Task<List<Resume>> GetAllAsync();
    Task<Resume?> GetByIdAsync(int id);
    Task AddAsync(Resume resume);
    Task SaveChangesAsync();
    Task DeleteAsync(Resume resume);
}
