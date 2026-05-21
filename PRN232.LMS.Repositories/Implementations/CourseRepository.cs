using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly LmsDbContext _context;
    public CourseRepository(LmsDbContext context) => _context = context;

    public Task<IQueryable<Course>> GetQueryableAsync()
    {
        IQueryable<Course> query = _context.Courses.AsNoTracking();
        return Task.FromResult(query);
    }

    public async Task<Course?> GetByIdAsync(int id)
        => await _context.Courses
            .Include(c => c.Semester)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.CourseId == id);

    public async Task<Course> CreateAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        var existing = await _context.Courses.FindAsync(course.CourseId);
        if (existing == null) return null;
        existing.CourseName = course.CourseName;
        existing.SemesterId = course.SemesterId;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }
}
