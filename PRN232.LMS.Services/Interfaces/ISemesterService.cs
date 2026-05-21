using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<SemesterBM?> GetByIdAsync(int id);
    Task<PagedResult<object>> GetAllAsync(SemesterQueryParams query);
    Task<SemesterBM> CreateAsync(SemesterBM bm);
    Task<SemesterBM?> UpdateAsync(int id, SemesterBM bm);
    Task<bool> DeleteAsync(int id);
}
