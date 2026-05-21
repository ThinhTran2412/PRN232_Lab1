using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → BusinessModel
        CreateMap<Student,    StudentBM>()
            .ForMember(d => d.Enrollments, o => o.MapFrom(s => s.Enrollments));
        CreateMap<Course,     CourseBM>()
            .ForMember(d => d.Semester, o => o.MapFrom(s => s.Semester));
        CreateMap<Semester,   SemesterBM>()
            .ForMember(d => d.Courses, o => o.MapFrom(s => s.Courses));
        CreateMap<Subject,    SubjectBM>();
        CreateMap<Enrollment, EnrollmentBM>()
            .ForMember(d => d.Student, o => o.MapFrom(s => s.Student))
            .ForMember(d => d.Course,  o => o.MapFrom(s => s.Course));

        // BusinessModel → Entity (for create/update via Request in Service)
        CreateMap<StudentBM,    Student>();
        CreateMap<CourseBM,     Course>();
        CreateMap<SemesterBM,   Semester>();
        CreateMap<SubjectBM,    Subject>();
        CreateMap<EnrollmentBM, Enrollment>();
    }
}
