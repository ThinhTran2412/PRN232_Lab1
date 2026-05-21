using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly LmsDbContext _context;
    public StudentRepository(LmsDbContext context) => _context = context;

    public Task<IQueryable<Student>> GetQueryableAsync()
    {
        IQueryable<Student> query = _context.Students.AsNoTracking();
        return Task.FromResult(query);
    }

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(s => s.StudentId == id);

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> UpdateAsync(Student student)
    {
        var existing = await _context.Students.FindAsync(student.StudentId);
        if (existing == null) return null;
        existing.FullName    = student.FullName;
        existing.Email       = student.Email;
        existing.DateOfBirth = student.DateOfBirth;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
