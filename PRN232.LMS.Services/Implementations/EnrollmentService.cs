using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;
    private readonly IMapper _mapper;

    public EnrollmentService(IEnrollmentRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<EnrollmentBM?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<EnrollmentBM>(entity);
    }

    public async Task<PagedResult<object>> GetAllAsync(EnrollmentQueryParams query)
    {
        var source = await _repo.GetQueryableAsync();

        // Expand
        if (!string.IsNullOrWhiteSpace(query.Expand))
        {
            var parts = query.Expand.Split(',');
            if (parts.Contains("student", StringComparer.OrdinalIgnoreCase))
                source = source.Include(e => e.Student);
            if (parts.Contains("course", StringComparer.OrdinalIgnoreCase))
                source = source.Include(e => e.Course).ThenInclude(c => c.Semester);
        }

        // Search (by status)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var kw = query.Search.ToLower();
            source = source.Where(e => e.Status.ToLower().Contains(kw));
        }

        // Sort
        source = QueryHelper.ApplySort(source, query.Sort);

        return await QueryHelper.PaginateAsync(source, query,
            e => (object)_mapper.Map<EnrollmentBM>(e));
    }

    public async Task<EnrollmentBM> CreateAsync(EnrollmentBM bm)
    {
        var entity  = _mapper.Map<Enrollment>(bm);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<EnrollmentBM>(created);
    }

    public async Task<EnrollmentBM?> UpdateAsync(int id, EnrollmentBM bm)
    {
        bm.EnrollmentId = id;
        var entity      = _mapper.Map<Enrollment>(bm);
        var updated     = await _repo.UpdateAsync(entity);
        return updated == null ? null : _mapper.Map<EnrollmentBM>(updated);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
}
