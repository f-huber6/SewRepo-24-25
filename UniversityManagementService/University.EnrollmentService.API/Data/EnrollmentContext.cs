using Microsoft.EntityFrameworkCore;
using University.EnrollmentService.API.Entities;

namespace University.EnrollmentService.API.Data;

public class EnrollmentContext: DbContext
{
    public EnrollmentContext(DbContextOptions<EnrollmentContext> options): base(options) {}
    
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new
            {
                e.StudentId, e.LectureId
            });
        base.OnModelCreating(modelBuilder);
    }
}