using ApprovalFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApprovalFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.RequesterId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RequesterName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasMany(e => e.History)
                .WithOne(h => h.ApprovalRequest)
                .HasForeignKey(h => h.ApprovalRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApprovalHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActorId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ActorName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Timestamp).IsRequired();
        });
    }
}
