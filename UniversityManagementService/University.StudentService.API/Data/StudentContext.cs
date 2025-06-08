using Microsoft.EntityFrameworkCore;
using University.StudentService.API.Entities;

namespace University.StudentService.API.Data;

public class StudentContext : DbContext
{
    public StudentContext(DbContextOptions<StudentContext> opts) : base(opts) { }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Student>().HasKey(s => s.Id);
        base.OnModelCreating(mb);
    }
}