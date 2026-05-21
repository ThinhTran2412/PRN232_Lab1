namespace PRN232.LMS.Services.BusinessModels;

public class StudentBM
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
    public List<EnrollmentBM> Enrollments { get; set; } = new();
}
