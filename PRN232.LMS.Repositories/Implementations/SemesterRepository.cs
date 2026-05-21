using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class SemesterRepository : ISemesterRepository
{
    private readonly LmsDbContext _context;
    public SemesterRepository(LmsDbContext context) => _context = context;

    public Task<IQueryable<Semester>> GetQueryableAsync()
    {
        IQueryable<Semester> query = _context.Semesters.AsNoTracking();
        return Task.FromResult(query);
    }

    public async Task<Semester?> GetByIdAsync(int id)
        => await _context.Semesters
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.SemesterId == id);

    public async Task<Semester> CreateAsync(Semester semester)
    {
        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();
        return semester;
    }

    public async Task<Semester?> UpdateAsync(Semester semester)
    {
        var existing = await _context.Semesters.FindAsync(semester.SemesterId);
        if (existing == null) return null;
        existing.SemesterName = semester.SemesterName;
        existing.StartDate    = semester.StartDate;
        existing.EndDate      = semester.EndDate;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var semester = await _context.Semesters.FindAsync(id);
        if (semester == null) return false;
        _context.Semesters.Remove(semester);
        await _context.SaveChangesAsync();
        return true;
    }
}
