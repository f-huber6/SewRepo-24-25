using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public class ApplicationContext: DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(s => s.Messages)
            .HasForeignKey(m => m.UserId);
    }
}