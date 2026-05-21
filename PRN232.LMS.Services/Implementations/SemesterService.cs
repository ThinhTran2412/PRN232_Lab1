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

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repo;
    private readonly IMapper _mapper;

    public SemesterService(ISemesterRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<SemesterBM?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<SemesterBM>(entity);
    }

    public async Task<PagedResult<object>> GetAllAsync(SemesterQueryParams query)
    {
        var source = await _repo.GetQueryableAsync();

        // Expand
        if (!string.IsNullOrWhiteSpace(query.Expand))
        {
            var parts = query.Expand.Split(',');
            if (parts.Contains("courses", StringComparer.OrdinalIgnoreCase))
                source = source.Include(s => s.Courses);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var kw = query.Search.ToLower();
            source = source.Where(s => s.SemesterName.ToLower().Contains(kw));
        }

        // Sort
        source = QueryHelper.ApplySort(source, query.Sort);

        return await QueryHelper.PaginateAsync(source, query,
            e => (object)_mapper.Map<SemesterBM>(e));
    }

    public async Task<SemesterBM> CreateAsync(SemesterBM bm)
    {
        var entity  = _mapper.Map<Semester>(bm);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<SemesterBM>(created);
    }

    public async Task<SemesterBM?> UpdateAsync(int id, SemesterBM bm)
    {
        bm.SemesterId = id;
        var entity    = _mapper.Map<Semester>(bm);
        var updated   = await _repo.UpdateAsync(entity);
        return updated == null ? null : _mapper.Map<SemesterBM>(updated);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
}
