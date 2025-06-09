using Microsoft.EntityFrameworkCore;
namespace University.StudentService.API.Data;

public class LearnMaterialContext : DbContext
{
    public LearnMaterialContext(DbContextOptions<LearnMaterialContext> opts) : base(opts)
    {
    }

    public DbSet<LearnMaterial.Entities.LearnMaterial> LearnMaterials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<LearnMaterial.Entities.LearnMaterial>()
            .HasOne(l => l.Course)
            .WithMany(k => k.LearnMaterials)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<LearnMaterial.Entities.LearnMaterial>()
            .HasOne(l => l.Subject)
            .WithMany(s => s.LearnMaterials)
            .HasForeignKey(l => l.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
