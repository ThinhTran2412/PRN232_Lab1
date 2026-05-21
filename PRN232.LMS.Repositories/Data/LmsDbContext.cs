using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public class LmsDbContext : DbContext
{
    public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Table mappings
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Semester>().ToTable("Semester");
        modelBuilder.Entity<Subject>().ToTable("Subject");
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");

        // Relationships
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Semester)
            .WithMany(s => s.Courses)
            .HasForeignKey(c => c.SemesterId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);

        // ===================== SEED DATA =====================

        // 5 Semesters
        modelBuilder.Entity<Semester>().HasData(
            new Semester { SemesterId = 1, SemesterName = "Fall 2023",   StartDate = new DateTime(2023, 8,  1), EndDate = new DateTime(2023, 12, 31) },
            new Semester { SemesterId = 2, SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1,  1), EndDate = new DateTime(2024, 5,  31) },
            new Semester { SemesterId = 3, SemesterName = "Summer 2024", StartDate = new DateTime(2024, 6,  1), EndDate = new DateTime(2024, 8,  31) },
            new Semester { SemesterId = 4, SemesterName = "Fall 2024",   StartDate = new DateTime(2024, 9,  1), EndDate = new DateTime(2025, 1,  15) },
            new Semester { SemesterId = 5, SemesterName = "Spring 2025", StartDate = new DateTime(2025, 1, 20), EndDate = new DateTime(2025, 6,  15) }
        );

        // 10 Subjects
        modelBuilder.Entity<Subject>().HasData(
            new Subject { SubjectId = 1,  SubjectCode = "NET101", SubjectName = ".NET Programming",        Credit = 3 },
            new Subject { SubjectId = 2,  SubjectCode = "DB201",  SubjectName = "Database Design",          Credit = 3 },
            new Subject { SubjectId = 3,  SubjectCode = "SE301",  SubjectName = "Software Engineering",     Credit = 3 },
            new Subject { SubjectId = 4,  SubjectCode = "WEB102", SubjectName = "Web Development",          Credit = 3 },
            new Subject { SubjectId = 5,  SubjectCode = "ALG202", SubjectName = "Algorithms & DS",          Credit = 4 },
            new Subject { SubjectId = 6,  SubjectCode = "NET302", SubjectName = "ASP.NET Core REST API",    Credit = 3 },
            new Subject { SubjectId = 7,  SubjectCode = "AI401",  SubjectName = "Artificial Intelligence",  Credit = 4 },
            new Subject { SubjectId = 8,  SubjectCode = "SEC201", SubjectName = "Cybersecurity Basics",     Credit = 3 },
            new Subject { SubjectId = 9,  SubjectCode = "MOB301", SubjectName = "Mobile Development",       Credit = 3 },
            new Subject { SubjectId = 10, SubjectCode = "CLD401", SubjectName = "Cloud Computing",          Credit = 3 }
        );

        // 20 Courses (4 per semester)
        modelBuilder.Entity<Course>().HasData(
            new Course { CourseId = 1,  CourseName = ".NET Programming - K17A",     SemesterId = 1 },
            new Course { CourseId = 2,  CourseName = "Database Design - K17B",       SemesterId = 1 },
            new Course { CourseId = 3,  CourseName = "Software Engineering - K17A",  SemesterId = 1 },
            new Course { CourseId = 4,  CourseName = "Web Development - K17C",       SemesterId = 1 },
            new Course { CourseId = 5,  CourseName = "Algorithms & DS - K17A",       SemesterId = 2 },
            new Course { CourseId = 6,  CourseName = "ASP.NET Core REST API - K17B", SemesterId = 2 },
            new Course { CourseId = 7,  CourseName = "Artificial Intelligence - K17A",SemesterId = 2 },
            new Course { CourseId = 8,  CourseName = "Cybersecurity Basics - K17C",  SemesterId = 2 },
            new Course { CourseId = 9,  CourseName = ".NET Programming - K18A",      SemesterId = 3 },
            new Course { CourseId = 10, CourseName = "Mobile Development - K18B",    SemesterId = 3 },
            new Course { CourseId = 11, CourseName = "Cloud Computing - K18A",       SemesterId = 3 },
            new Course { CourseId = 12, CourseName = "Database Design - K18C",       SemesterId = 3 },
            new Course { CourseId = 13, CourseName = "Software Engineering - K18A",  SemesterId = 4 },
            new Course { CourseId = 14, CourseName = "Web Development - K18B",       SemesterId = 4 },
            new Course { CourseId = 15, CourseName = "Algorithms & DS - K18C",       SemesterId = 4 },
            new Course { CourseId = 16, CourseName = "ASP.NET Core REST API - K18A", SemesterId = 4 },
            new Course { CourseId = 17, CourseName = "Artificial Intelligence - K19A",SemesterId = 5 },
            new Course { CourseId = 18, CourseName = "Cybersecurity Basics - K19B",  SemesterId = 5 },
            new Course { CourseId = 19, CourseName = "Mobile Development - K19A",    SemesterId = 5 },
            new Course { CourseId = 20, CourseName = "Cloud Computing - K19B",       SemesterId = 5 }
        );

        // 50 Students
        var students = new[]
        {
            new Student { StudentId = 1,  FullName = "Nguyen Van An",     Email = "an.nv@fpt.edu.vn",     DateOfBirth = new DateTime(2003, 1, 15) },
            new Student { StudentId = 2,  FullName = "Le Thi Binh",       Email = "binh.lt@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 3, 22) },
            new Student { StudentId = 3,  FullName = "Tran Van Cuong",    Email = "cuong.tv@fpt.edu.vn",  DateOfBirth = new DateTime(2002, 7, 10) },
            new Student { StudentId = 4,  FullName = "Pham Thi Dung",     Email = "dung.pt@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 11, 5) },
            new Student { StudentId = 5,  FullName = "Hoang Van Em",      Email = "em.hv@fpt.edu.vn",     DateOfBirth = new DateTime(2002, 4, 18) },
            new Student { StudentId = 6,  FullName = "Vo Thi Phuong",     Email = "phuong.vt@fpt.edu.vn", DateOfBirth = new DateTime(2003, 6, 30) },
            new Student { StudentId = 7,  FullName = "Nguyen Minh Quan",  Email = "quan.nm@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 9, 14) },
            new Student { StudentId = 8,  FullName = "Bui Thi Hoa",       Email = "hoa.bt@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 2, 28) },
            new Student { StudentId = 9,  FullName = "Do Van Khoa",       Email = "khoa.dv@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 12, 3) },
            new Student { StudentId = 10, FullName = "Tran Thi Lan",      Email = "lan.tt@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 5, 20) },
            new Student { StudentId = 11, FullName = "Phan Van Manh",     Email = "manh.pv@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 8, 7)  },
            new Student { StudentId = 12, FullName = "Dinh Thi Nam",      Email = "nam.dt@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 10, 25)},
            new Student { StudentId = 13, FullName = "Le Van Oanh",       Email = "oanh.lv@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 3, 12) },
            new Student { StudentId = 14, FullName = "Nguyen Thi Phuong", Email = "phuong.nt@fpt.edu.vn", DateOfBirth = new DateTime(2003, 7, 19) },
            new Student { StudentId = 15, FullName = "Tran Van Quy",      Email = "quy.tv@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 1, 28) },
            new Student { StudentId = 16, FullName = "Hoang Thi Rang",    Email = "rang.ht@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 4, 15) },
            new Student { StudentId = 17, FullName = "Vo Van Son",        Email = "son.vv@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 6, 22) },
            new Student { StudentId = 18, FullName = "Bui Van Thanh",     Email = "thanh.bv@fpt.edu.vn",  DateOfBirth = new DateTime(2003, 9, 8)  },
            new Student { StudentId = 19, FullName = "Do Thi Uyen",       Email = "uyen.dt@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 11, 16)},
            new Student { StudentId = 20, FullName = "Pham Van Viet",     Email = "viet.pv@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 2, 4)  },
            new Student { StudentId = 21, FullName = "Nguyen Thi Xuan",   Email = "xuan.nt@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 5, 30) },
            new Student { StudentId = 22, FullName = "Le Van Yen",        Email = "yen.lv@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 8, 17) },
            new Student { StudentId = 23, FullName = "Tran Minh Zung",    Email = "zung.tm@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 10, 5) },
            new Student { StudentId = 24, FullName = "Phan Thi Anh",      Email = "anh.pt@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 12, 21)},
            new Student { StudentId = 25, FullName = "Dinh Van Bach",     Email = "bach.dv@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 2, 14) },
            new Student { StudentId = 26, FullName = "Hoang Thi Chi",     Email = "chi.ht@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 4, 1)  },
            new Student { StudentId = 27, FullName = "Vo Van Dat",        Email = "dat.vv@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 7, 27) },
            new Student { StudentId = 28, FullName = "Bui Thi Diem",      Email = "diem.bt@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 9, 13) },
            new Student { StudentId = 29, FullName = "Do Van Duc",        Email = "duc.dv@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 11, 20)},
            new Student { StudentId = 30, FullName = "Nguyen Van Gia",    Email = "gia.nv@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 1, 9)  },
            new Student { StudentId = 31, FullName = "Le Thi Ha",         Email = "ha.lt@fpt.edu.vn",     DateOfBirth = new DateTime(2002, 3, 25) },
            new Student { StudentId = 32, FullName = "Tran Van Hung",     Email = "hung.tv@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 6, 11) },
            new Student { StudentId = 33, FullName = "Pham Thi Huong",    Email = "huong.pt@fpt.edu.vn",  DateOfBirth = new DateTime(2002, 8, 18) },
            new Student { StudentId = 34, FullName = "Hoang Van Khanh",   Email = "khanh.hv@fpt.edu.vn",  DateOfBirth = new DateTime(2003, 10, 6) },
            new Student { StudentId = 35, FullName = "Vo Thi Kim",        Email = "kim.vt@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 12, 29)},
            new Student { StudentId = 36, FullName = "Bui Van Long",      Email = "long.bv@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 2, 17) },
            new Student { StudentId = 37, FullName = "Do Thi Mai",        Email = "mai.dt@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 5, 3)  },
            new Student { StudentId = 38, FullName = "Phan Van Minh",     Email = "minh.pv@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 7, 24) },
            new Student { StudentId = 39, FullName = "Dinh Thi Ngoc",     Email = "ngoc.dt@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 9, 10) },
            new Student { StudentId = 40, FullName = "Nguyen Van Phong",  Email = "phong.nv@fpt.edu.vn",  DateOfBirth = new DateTime(2003, 11, 28)},
            new Student { StudentId = 41, FullName = "Le Thi Quyen",      Email = "quyen.lt@fpt.edu.vn",  DateOfBirth = new DateTime(2002, 1, 16) },
            new Student { StudentId = 42, FullName = "Tran Van Sang",     Email = "sang.tv@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 4, 4)  },
            new Student { StudentId = 43, FullName = "Pham Thi Tam",      Email = "tam.pt@fpt.edu.vn",    DateOfBirth = new DateTime(2002, 6, 20) },
            new Student { StudentId = 44, FullName = "Hoang Van Tuan",    Email = "tuan.hv@fpt.edu.vn",   DateOfBirth = new DateTime(2003, 8, 8)  },
            new Student { StudentId = 45, FullName = "Vo Thi Tuyen",      Email = "tuyen.vt@fpt.edu.vn",  DateOfBirth = new DateTime(2002, 10, 22)},
            new Student { StudentId = 46, FullName = "Bui Van Vuong",     Email = "vuong.bv@fpt.edu.vn",  DateOfBirth = new DateTime(2003, 12, 10)},
            new Student { StudentId = 47, FullName = "Do Van Xuan",       Email = "xuan.dv@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 2, 6)  },
            new Student { StudentId = 48, FullName = "Phan Thi Yen",      Email = "yen.pt@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 5, 14) },
            new Student { StudentId = 49, FullName = "Dinh Van Phuc",     Email = "phuc.dv@fpt.edu.vn",   DateOfBirth = new DateTime(2002, 7, 31) },
            new Student { StudentId = 50, FullName = "Nguyen Thi Bao",    Email = "bao.nt@fpt.edu.vn",    DateOfBirth = new DateTime(2003, 11, 19)}
        };
        modelBuilder.Entity<Student>().HasData(students);

        // 500 Enrollments (50 students × 10 courses each)
        var statuses = new[] { "Active", "Completed", "Dropped" };
        var baseDate = new DateTime(2023, 8, 15);
        var enrollments = new List<Enrollment>();
        for (int s = 1; s <= 50; s++)
        {
            for (int j = 0; j < 10; j++)
            {
                var courseId = ((s - 1 + j) % 20) + 1;
                var enrollId = (s - 1) * 10 + j + 1;
                enrollments.Add(new Enrollment
                {
                    EnrollmentId = enrollId,
                    StudentId    = s,
                    CourseId     = courseId,
                    EnrollDate   = baseDate.AddDays(enrollId % 120),
                    Status       = statuses[enrollId % 3]
                });
            }
        }
        modelBuilder.Entity<Enrollment>().HasData(enrollments);
    }
}
