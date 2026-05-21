using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentBM?> GetByIdAsync(int id);
    Task<PagedResult<object>> GetAllAsync(EnrollmentQueryParams query);
    Task<EnrollmentBM> CreateAsync(EnrollmentBM bm);
    Task<EnrollmentBM?> UpdateAsync(int id, EnrollmentBM bm);
    Task<bool> DeleteAsync(int id);
}
