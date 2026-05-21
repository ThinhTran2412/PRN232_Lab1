using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<CourseBM?> GetByIdAsync(int id);
    Task<PagedResult<object>> GetAllAsync(CourseQueryParams query);
    Task<CourseBM> CreateAsync(CourseBM bm);
    Task<CourseBM?> UpdateAsync(int id, CourseBM bm);
    Task<bool> DeleteAsync(int id);
}
