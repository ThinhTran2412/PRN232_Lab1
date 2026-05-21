using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<SubjectBM?> GetByIdAsync(int id);
    Task<PagedResult<object>> GetAllAsync(SubjectQueryParams query);
    Task<SubjectBM> CreateAsync(SubjectBM bm);
    Task<SubjectBM?> UpdateAsync(int id, SubjectBM bm);
    Task<bool> DeleteAsync(int id);
}
