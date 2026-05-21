using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;
    private readonly IMapper _mapper;

    public SubjectService(ISubjectRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<SubjectBM?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<SubjectBM>(entity);
    }

    public async Task<PagedResult<object>> GetAllAsync(SubjectQueryParams query)
    {
        var source = await _repo.GetQueryableAsync();

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var kw = query.Search.ToLower();
            source = source.Where(s =>
                s.SubjectName.ToLower().Contains(kw) ||
                s.SubjectCode.ToLower().Contains(kw));
        }

        // Sort
        source = QueryHelper.ApplySort(source, query.Sort);

        return await QueryHelper.PaginateAsync(source, query,
            e => (object)_mapper.Map<SubjectBM>(e));
    }

    public async Task<SubjectBM> CreateAsync(SubjectBM bm)
    {
        var entity  = _mapper.Map<Subject>(bm);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<SubjectBM>(created);
    }

    public async Task<SubjectBM?> UpdateAsync(int id, SubjectBM bm)
    {
        bm.SubjectId = id;
        var entity   = _mapper.Map<Subject>(bm);
        var updated  = await _repo.UpdateAsync(entity);
        return updated == null ? null : _mapper.Map<SubjectBM>(updated);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
}
