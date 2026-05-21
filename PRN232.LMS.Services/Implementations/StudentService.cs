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

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<StudentBM?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<StudentBM>(entity);
    }

    public async Task<PagedResult<object>> GetAllAsync(StudentQueryParams query)
    {
        var source = await _repo.GetQueryableAsync();

        // Expand
        if (!string.IsNullOrWhiteSpace(query.Expand))
        {
            var parts = query.Expand.Split(',');
            if (parts.Contains("enrollments", StringComparer.OrdinalIgnoreCase))
                source = source.Include(s => s.Enrollments).ThenInclude(e => e.Course);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var kw = query.Search.ToLower();
            source = source.Where(s =>
                s.FullName.ToLower().Contains(kw) ||
                s.Email.ToLower().Contains(kw));
        }

        // Sort
        source = QueryHelper.ApplySort(source, query.Sort);

        return await QueryHelper.PaginateAsync(source, query,
            e => (object)_mapper.Map<StudentBM>(e));
    }

    public async Task<StudentBM> CreateAsync(StudentBM bm)
    {
        var entity  = _mapper.Map<Student>(bm);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<StudentBM>(created);
    }

    public async Task<StudentBM?> UpdateAsync(int id, StudentBM bm)
    {
        bm.StudentId = id;
        var entity  = _mapper.Map<Student>(bm);
        var updated = await _repo.UpdateAsync(entity);
        return updated == null ? null : _mapper.Map<StudentBM>(updated);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
}
