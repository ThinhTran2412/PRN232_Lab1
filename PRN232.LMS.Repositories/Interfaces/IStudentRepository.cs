using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<IQueryable<Student>> GetQueryableAsync();
    Task<Student> CreateAsync(Student student);
    Task<Student?> UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
}
