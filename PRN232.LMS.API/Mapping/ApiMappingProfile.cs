using AutoMapper;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // Request → BusinessModel
        CreateMap<CreateStudentRequest,  StudentBM>()
            .ForMember(d => d.StudentId,   o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());
        CreateMap<UpdateStudentRequest,  StudentBM>()
            .ForMember(d => d.StudentId,   o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());

        CreateMap<CreateCourseRequest,   CourseBM>()
            .ForMember(d => d.CourseId,  o => o.Ignore())
            .ForMember(d => d.Semester,  o => o.Ignore());
        CreateMap<UpdateCourseRequest,   CourseBM>()
            .ForMember(d => d.CourseId,  o => o.Ignore())
            .ForMember(d => d.Semester,  o => o.Ignore());

        CreateMap<CreateSemesterRequest, SemesterBM>()
            .ForMember(d => d.SemesterId, o => o.Ignore());
        CreateMap<UpdateSemesterRequest, SemesterBM>()
            .ForMember(d => d.SemesterId, o => o.Ignore());

        CreateMap<CreateSubjectRequest,  SubjectBM>()
            .ForMember(d => d.SubjectId, o => o.Ignore());
        CreateMap<UpdateSubjectRequest,  SubjectBM>()
            .ForMember(d => d.SubjectId, o => o.Ignore());

        CreateMap<CreateEnrollmentRequest, EnrollmentBM>()
            .ForMember(d => d.EnrollmentId, o => o.Ignore())
            .ForMember(d => d.Student,       o => o.Ignore())
            .ForMember(d => d.Course,        o => o.Ignore());
        CreateMap<UpdateEnrollmentRequest, EnrollmentBM>()
            .ForMember(d => d.EnrollmentId, o => o.Ignore())
            .ForMember(d => d.Student,       o => o.Ignore())
            .ForMember(d => d.Course,        o => o.Ignore());

        // BusinessModel → Response
        CreateMap<StudentBM,    StudentResponse>();
        CreateMap<CourseBM,     CourseResponse>()
            .ForMember(d => d.Semester, o => o.MapFrom(s => s.Semester));
        CreateMap<SemesterBM,   SemesterResponse>()
            .ForMember(d => d.Courses, o => o.MapFrom(s => s.Courses));
        CreateMap<SubjectBM,    SubjectResponse>();
        CreateMap<EnrollmentBM, EnrollmentResponse>()
            .ForMember(d => d.Student, o => o.MapFrom(s => s.Student))
            .ForMember(d => d.Course,  o => o.MapFrom(s => s.Course));
    }
}
