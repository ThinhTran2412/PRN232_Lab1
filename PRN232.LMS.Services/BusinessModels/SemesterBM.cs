namespace PRN232.LMS.Services.BusinessModels;

public class SemesterBM
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<CourseBM>? Courses { get; set; }
}
