namespace PRN232.LMS.API.Models.Requests;

public class CreateCourseRequest
{
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
}

public class UpdateCourseRequest
{
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }
}
