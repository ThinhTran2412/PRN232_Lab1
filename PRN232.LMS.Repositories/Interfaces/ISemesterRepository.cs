using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository
{
    Task<Semester?> GetByIdAsync(int id);
    Task<IQueryable<Semester>> GetQueryableAsync();
    Task<Semester> CreateAsync(Semester semester);
    Task<Semester?> UpdateAsync(Semester semester);
    Task<bool> DeleteAsync(int id);
}
