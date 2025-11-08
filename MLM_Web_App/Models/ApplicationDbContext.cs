using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MLM_Web_App.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwTeamLevel> VwTeamLevels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Audit__3214EC0748E8B3CB");

            entity.ToTable("Audit");

            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CE211C14");

            entity.HasIndex(e => e.UserCode, "UQ__Users__1DF52D0C1B1B2142").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344550D8C9").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.SponsorUserCode).HasMaxLength(20);
            entity.Property(e => e.UserCode).HasMaxLength(20);

            entity.HasOne(d => d.Sponsor).WithMany(p => p.InverseSponsor)
                .HasForeignKey(d => d.SponsorId)
                .HasConstraintName("FK_Users_Sponsor");
        });

        modelBuilder.Entity<VwTeamLevel>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TeamLevels");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
