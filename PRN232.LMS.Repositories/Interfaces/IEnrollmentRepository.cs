using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id);
    Task<IQueryable<Enrollment>> GetQueryableAsync();
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<Enrollment?> UpdateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
}
