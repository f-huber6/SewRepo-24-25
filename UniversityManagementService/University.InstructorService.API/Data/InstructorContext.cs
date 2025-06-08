using Microsoft.EntityFrameworkCore;
using University.InstructorService.API.Entities;

namespace University.InstructorService.API.Data;

public class InstructorContext : DbContext
{
    public InstructorContext(DbContextOptions<InstructorContext> opts) 
        : base(opts) { }

    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Course>      Courses     { get; set; }
    public DbSet<Lecture>     Lectures    { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Instructor>().HasKey(i => i.Id);

        mb.Entity<Course>()
            .HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId);

        mb.Entity<Lecture>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lectures)
            .HasForeignKey(l => l.CourseId);

        base.OnModelCreating(mb);
    }
}