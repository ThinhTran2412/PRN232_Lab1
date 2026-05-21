using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<StudentBM?> GetByIdAsync(int id);
    Task<PagedResult<object>> GetAllAsync(StudentQueryParams query);
    Task<StudentBM> CreateAsync(StudentBM bm);
    Task<StudentBM?> UpdateAsync(int id, StudentBM bm);
    Task<bool> DeleteAsync(int id);
}
