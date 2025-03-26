using Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public class ApplicationContext: IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
    }

    public new DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<FriendConnection> FriendConnections { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany(s => s.Messages)
            .HasForeignKey(m => m.UserId);
        
        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.LoginProvider, p.ProviderKey });
        
        modelBuilder.Entity<FriendConnection>()
            .HasOne(fc => fc.To)
            .WithMany()
            .HasForeignKey(f => f.ToId);
        
        modelBuilder.Entity<FriendConnection>()
            .HasOne(fc => fc.From)
            .WithMany()
            .HasForeignKey(f => f.FromId);
    }
}