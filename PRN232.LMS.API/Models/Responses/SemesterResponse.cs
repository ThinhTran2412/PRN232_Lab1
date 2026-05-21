namespace PRN232.LMS.API.Models.Responses;

public class SemesterResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<CourseResponse>? Courses { get; set; }
}
