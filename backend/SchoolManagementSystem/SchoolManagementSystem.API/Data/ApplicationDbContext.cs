using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core Entities
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Course> Courses { get; set; }

        // Academic
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<Grade> Grades { get; set; }

        // Operations
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<TimeTable> TimeTables { get; set; }

        // Join Tables
        public DbSet<TeacherCourse> TeacherCourses { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* =========================
             * STUDENT
             * ========================= */
            builder.Entity<Student>()
                .HasKey(s => s.Id);

            builder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            /* =========================
             * TEACHER
             * ========================= */
            builder.Entity<Teacher>(entity =>
            {
                entity.HasKey(t => t.Id);

                // Make sure IdentityUserId is just a regular column, no FK
                entity.Property(t => t.IdentityUserId)
                    .IsRequired()
                    .HasMaxLength(450); // Match IdentityUser.Id length
            });

            /* =========================
             * STUDENT ↔ PARENT
            /* ========================= */
            builder.Entity<StudentParent>()
                .HasKey(sp => new { sp.StudentId, sp.ParentId });

            builder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId);

            builder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId);


            /* =========================
             * CLASS
             * ========================= */
            builder.Entity<Class>()
                .HasKey(c => c.Id);

            builder.Entity<Class>()
                .HasOne(c => c.ClassTeacher)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.ClassTeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            /* =========================
             * COURSE
             * ========================= */
            builder.Entity<Course>()
                .HasKey(c => c.Id);

            /* =========================
             * ENROLLMENT (Student ↔ Course)
             * ========================= */
            builder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            /* =========================
             * TEACHER ↔ COURSE (Many-to-Many)
             * ========================= */
            builder.Entity<TeacherCourse>()
                .HasKey(tc => new { tc.TeacherId, tc.CourseId });

            builder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Teacher)
                .WithMany(t => t.TeacherCourses)
                .HasForeignKey(tc => tc.TeacherId);

            builder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Course)
                .WithMany(c => c.TeacherCourses)
                .HasForeignKey(tc => tc.CourseId);

            /* =========================
             * EXAM
             * ========================= */
            builder.Entity<Exam>()
                .HasKey(e => e.Id);

            builder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId);

            builder.Entity<Exam>()
                .HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId);

            /* =========================
             * STUDENT ↔ EXAM
             * ========================= */
            builder.Entity<StudentExam>()
                .HasKey(se => new { se.StudentId, se.ExamId });

            builder.Entity<StudentExam>()
                .HasOne(se => se.Student)
                .WithMany(s => s.StudentExams)
                .HasForeignKey(se => se.StudentId);

            builder.Entity<StudentExam>()
                .HasOne(se => se.Exam)
                .WithMany(e => e.StudentExams)
                .HasForeignKey(se => se.ExamId);

            /* =========================
             * ATTENDANCE
             * ========================= */
            builder.Entity<Attendance>()
                .HasKey(a => a.Id);

            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            builder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId);

            builder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassId);

            /* =========================
             * GRADE
             * ========================= */
            builder.Entity<Grade>()
                .HasKey(g => g.Id);

            builder.Entity<Grade>()
                .Property(g => g.Score)
                .HasPrecision(5, 2);

            builder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId);

            builder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany()
                .HasForeignKey(g => g.CourseId);

            /* =========================
             * TIMETABLE
             * ========================= */
            builder.Entity<TimeTable>()
                .HasKey(tt => tt.Id);

            builder.Entity<TimeTable>()
                .HasOne(tt => tt.Class)
                .WithMany(c => c.TimeTables)
                .HasForeignKey(tt => tt.ClassId);

            builder.Entity<TimeTable>()
                .HasOne(tt => tt.Course)
                .WithMany(c => c.TimeTables)
                .HasForeignKey(tt => tt.CourseId);

            builder.Entity<TimeTable>()
                .HasOne(tt => tt.Teacher)
                .WithMany(t => t.TimeTables)
                .HasForeignKey(tt => tt.TeacherId);
        }
    }
}
