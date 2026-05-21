using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly LmsDbContext _context;
    public EnrollmentRepository(LmsDbContext context) => _context = context;

    public Task<IQueryable<Enrollment>> GetQueryableAsync()
    {
        IQueryable<Enrollment> query = _context.Enrollments.AsNoTracking();
        return Task.FromResult(query);
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
        => await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment?> UpdateAsync(Enrollment enrollment)
    {
        var existing = await _context.Enrollments.FindAsync(enrollment.EnrollmentId);
        if (existing == null) return null;
        existing.StudentId  = enrollment.StudentId;
        existing.CourseId   = enrollment.CourseId;
        existing.EnrollDate = enrollment.EnrollDate;
        existing.Status     = enrollment.Status;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return false;
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }
}
