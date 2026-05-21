using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IQueryable<Course>> GetQueryableAsync();
    Task<Course> CreateAsync(Course course);
    Task<Course?> UpdateAsync(Course course);
    Task<bool> DeleteAsync(int id);
}
