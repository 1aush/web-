using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NetTask.Models;

namespace NetTask;

public partial class NetTaskDbContext : DbContext
{
    public NetTaskDbContext(DbContextOptions<NetTaskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Department { get; set; }

    public virtual DbSet<LoginUser> LoginUser { get; set; }

    public virtual DbSet<RolePermission> RolePermission { get; set; }

    public virtual DbSet<TaskItem> TaskItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Department_Id);

            entity.Property(e => e.Department_Id).ValueGeneratedNever();
            entity.Property(e => e.Department_Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Department_Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LoginUser>(entity =>
        {
            entity.HasKey(e => e.LoginUser_Id);

            entity.Property(e => e.LoginUser_Id).ValueGeneratedNever();
            entity.Property(e => e.LoginUser_Account)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser_CreateTime).HasColumnType("datetime");
            entity.Property(e => e.LoginUser_Password).IsUnicode(false);
            entity.Property(e => e.LoginUser_Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser_Salt).IsUnicode(false);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RolePermission_Role, e.RolePermission_Permission });

            entity.Property(e => e.RolePermission_Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RolePermission_Permission)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.TaskItem_Id);

            entity.Property(e => e.TaskItem_Id).ValueGeneratedNever();
            entity.Property(e => e.TaskItem_CreateTime).HasColumnType("datetime");
            entity.Property(e => e.TaskItem_FinishTime).HasColumnType("datetime");
            entity.Property(e => e.TaskItem_State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TaskItem_Title)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
