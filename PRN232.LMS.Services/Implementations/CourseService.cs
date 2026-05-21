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

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<CourseBM?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<CourseBM>(entity);
    }

    public async Task<PagedResult<object>> GetAllAsync(CourseQueryParams query)
    {
        var source = await _repo.GetQueryableAsync();

        // Expand
        if (!string.IsNullOrWhiteSpace(query.Expand))
        {
            var parts = query.Expand.Split(',');
            if (parts.Contains("semester", StringComparer.OrdinalIgnoreCase))
                source = source.Include(c => c.Semester);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var kw = query.Search.ToLower();
            source = source.Where(c => c.CourseName.ToLower().Contains(kw));
        }

        // Sort
        source = QueryHelper.ApplySort(source, query.Sort);

        return await QueryHelper.PaginateAsync(source, query,
            e => (object)_mapper.Map<CourseBM>(e));
    }

    public async Task<CourseBM> CreateAsync(CourseBM bm)
    {
        var entity  = _mapper.Map<Course>(bm);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<CourseBM>(created);
    }

    public async Task<CourseBM?> UpdateAsync(int id, CourseBM bm)
    {
        bm.CourseId = id;
        var entity  = _mapper.Map<Course>(bm);
        var updated = await _repo.UpdateAsync(entity);
        return updated == null ? null : _mapper.Map<CourseBM>(updated);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
}
