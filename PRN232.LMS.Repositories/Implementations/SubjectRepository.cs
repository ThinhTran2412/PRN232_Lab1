using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SubjectRepository : ISubjectRepository
{
    private readonly LmsDbContext _context;
    public SubjectRepository(LmsDbContext context) => _context = context;

    public Task<IQueryable<Subject>> GetQueryableAsync()
    {
        IQueryable<Subject> query = _context.Subjects.AsNoTracking();
        return Task.FromResult(query);
    }

    public async Task<Subject?> GetByIdAsync(int id)
        => await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id);

    public async Task<Subject> CreateAsync(Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task<Subject?> UpdateAsync(Subject subject)
    {
        var existing = await _context.Subjects.FindAsync(subject.SubjectId);
        if (existing == null) return null;
        existing.SubjectCode = subject.SubjectCode;
        existing.SubjectName = subject.SubjectName;
        existing.Credit      = subject.Credit;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return false;
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return true;
    }
}
