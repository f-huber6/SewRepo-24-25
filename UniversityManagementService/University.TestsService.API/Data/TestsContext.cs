using BlazorApp1.Entities;
using Microsoft.EntityFrameworkCore;
using University.StudentService.API.Entities;

namespace BlazorApp1.Data;

public class TestsContext : DbContext
{
    public TestsContext(DbContextOptions<TestsContext> opts) 
        : base(opts) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Test>      Tests     { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        
        mb.Entity<Test>()
            .HasOne(t => t.Student)
            .WithMany() 
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);  
        
        mb.Entity<Student>()
            .Property(s => s.Id)
            .ValueGeneratedNever(); 
    }
}