namespace PRN232.LMS.Services.BusinessModels;

public class EnrollmentBM
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;
    public StudentBM? Student { get; set; }
    public CourseBM? Course { get; set; }
}
