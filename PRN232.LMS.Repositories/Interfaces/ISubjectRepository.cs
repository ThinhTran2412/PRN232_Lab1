using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(int id);
    Task<IQueryable<Subject>> GetQueryableAsync();
    Task<Subject> CreateAsync(Subject subject);
    Task<Subject?> UpdateAsync(Subject subject);
    Task<bool> DeleteAsync(int id);
}
