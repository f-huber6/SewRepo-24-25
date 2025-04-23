using Chat.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat.Database.Context;

public class ApplicationContext: IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FriendShip>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId);
        
        builder.Entity<FriendShip>()
            .HasOne(f => f.Friend)
            .WithMany()
            .HasForeignKey(f => f.FriendId);
        
        builder.Entity<ChatMessage>()
            .HasMany(m => m.ChatFiles)
            .WithOne(f => f.ChatMessage)
            .HasForeignKey(f => f.ChatMessageId);
    }
    
    public new DbSet<User> Users { get; set; }
    public DbSet<FriendShip> FriendShips { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    
    public DbSet<SavingsGoal> SavingsGoals { get; set; }
    
    public DbSet<ChatFile> ChatFiles { get; set; }
}