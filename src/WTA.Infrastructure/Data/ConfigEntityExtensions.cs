using Microsoft.EntityFrameworkCore;
using WTA.Application.Domain;
using WTA.Application.Domain.Blogs;
using WTA.Application.Domain.Users;

namespace WTA.Infrastructure.Data;

public static class ConfigEntityExtensions
{
  public static void Config(this ModelBuilder builder)
  {
    // builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    // System
    builder.Entity<Event>().HasIndex(o => o.Entity);
    // Setting
    builder.Entity<Setting>().HasIndex(o => o.Key).IsUnique();
    // User
    builder.Entity<User>().HasMany(o => o.UserLogins).WithOne(o => o.User).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<UserLogin>().HasIndex(o => new { o.LoginProvider, o.ProviderKey }).IsUnique();
    // Role Permissions
    builder.Entity<UserRole>().HasOne(o => o.User).WithMany(o => o.UserRoles).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<UserRole>().HasOne(o => o.Role).WithMany(o => o.UserRoles).HasForeignKey(o => o.RoleId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<UserRole>().HasIndex(o => new { o.UserId, o.RoleId }).IsUnique();
    builder.Entity<RolePermission>().HasOne(o => o.Role).WithMany(o => o.RolePermissions).HasForeignKey(o => o.RoleId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<RolePermission>().HasOne(o => o.Permission).WithMany(o => o.RolePermissions).HasForeignKey(o => o.PermissionId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<RolePermission>().HasIndex(o => new { o.RoleId, o.PermissionId }).IsUnique();
    // Blog
    builder.Entity<BlogPost>().HasOne(o => o.User).WithMany(o => o.BlogPosts).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<BlogComment>().HasOne(o => o.User).WithMany(o => o.BlogComments).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<BlogComment>().HasOne(o => o.BlogPost).WithMany(o => o.BlogCommnets).HasForeignKey(o => o.BlogPostId).OnDelete(DeleteBehavior.Cascade);
    builder.Entity<BlogComment>().HasOne(o => o.Parent).WithMany(o => o.Children).HasForeignKey(o => o.ParentId).OnDelete(DeleteBehavior.SetNull);
  }
}
