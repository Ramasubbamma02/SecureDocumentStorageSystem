using Microsoft.EntityFrameworkCore;
using System;

public class SecureDocumentContext : DbContext
{
    public SecureDocumentContext(DbContextOptions<SecureDocumentContext> options)
        : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint on Username
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Unique constraint on (Name, Version, UserId)
        modelBuilder.Entity<Document>()
            .HasIndex(d => new { d.Name, d.Version, d.UserId })
            .IsUnique();
    }
}