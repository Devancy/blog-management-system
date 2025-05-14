using BlogManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementSystem.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Post> Posts { get; set; }
	public DbSet<Comment> Comments { get; set; }
	public DbSet<LocalRole> LocalRoles { get; set; }
	public DbSet<LocalGroup> LocalGroups { get; set; }
	public DbSet<LocalUserRole> LocalUserRoles { get; set; }
	public DbSet<LocalUserGroup> LocalUserGroups { get; set; }
	public DbSet<LocalUserIdentity> LocalUserIdentities { get; set; }
	public DbSet<LocalGroupRole> LocalGroupRoles { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Blog post relationships
		modelBuilder.Entity<Post>()
			.HasMany(p => p.Comments)
			.WithOne(c => c.Post)
			.HasForeignKey(c => c.PostId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Post>()
			.Property(p => p.Status)
			.HasConversion<string>();

		modelBuilder.Entity<Post>()
			.Property(p => p.Title)
			.IsRequired()
			.HasMaxLength(200);

		modelBuilder.Entity<Post>()
			.Property(p => p.Slug)
			.IsRequired()
			.HasMaxLength(200);

		modelBuilder.Entity<Post>()
			.HasIndex(p => p.Slug)
			.IsUnique();

		modelBuilder.Entity<Post>()
			.Property(p => p.AuthorId)
			.IsRequired();

		modelBuilder.Entity<Comment>()
			.Property(c => c.Content)
			.IsRequired()
			.HasMaxLength(1000);

		modelBuilder.Entity<Comment>()
			.Property(c => c.UserId)
			.IsRequired();

		modelBuilder.Entity<Comment>()
			.HasOne(c => c.Post)
			.WithMany(p => p.Comments)
			.HasForeignKey(c => c.PostId)
			.OnDelete(DeleteBehavior.Cascade);
			
		// Identity management relationships
		modelBuilder.Entity<LocalRole>()
			.HasIndex(r => r.Name)
			.IsUnique();
			
		modelBuilder.Entity<LocalRole>()
			.Property(r => r.Name)
			.IsRequired()
			.HasMaxLength(100);
			
		modelBuilder.Entity<LocalUserIdentity>()
			.HasKey(ui => ui.UserId);
			
		modelBuilder.Entity<LocalUserIdentity>()
			.HasIndex(ui => ui.Username)
			.IsUnique();
			
		modelBuilder.Entity<LocalUserIdentity>()
			.HasIndex(ui => ui.Email);
			
		modelBuilder.Entity<LocalUserIdentity>()
			.Property(ui => ui.Username)
			.IsRequired()
			.HasMaxLength(100);
			
		modelBuilder.Entity<LocalUserRole>()
			.HasKey(ur => new { ur.UserId, ur.RoleId });
			
		modelBuilder.Entity<LocalUserRole>()
			.HasOne(ur => ur.Role)
			.WithMany(r => r.UserRoles)
			.HasForeignKey(ur => ur.RoleId)
			.OnDelete(DeleteBehavior.Cascade);
			
		modelBuilder.Entity<LocalUserRole>()
			.HasOne(ur => ur.UserIdentity)
			.WithMany(ui => ui.UserRoles)
			.HasForeignKey(ur => ur.UserId)
			.OnDelete(DeleteBehavior.Cascade);
			
		modelBuilder.Entity<LocalGroup>()
			.HasIndex(g => g.Name);
			
		modelBuilder.Entity<LocalGroup>()
			.HasIndex(g => g.Path)
			.IsUnique();
			
		modelBuilder.Entity<LocalGroup>()
			.Property(g => g.Name)
			.IsRequired()
			.HasMaxLength(100);
			
		modelBuilder.Entity<LocalGroup>()
			.Property(g => g.Path)
			.IsRequired()
			.HasMaxLength(255);
			
		modelBuilder.Entity<LocalGroup>()
			.HasOne(g => g.ParentGroup)
			.WithMany(g => g.SubGroups)
			.HasForeignKey(g => g.ParentGroupId)
			.OnDelete(DeleteBehavior.Restrict);
			
		modelBuilder.Entity<LocalUserGroup>()
			.HasKey(ug => new { ug.UserId, ug.GroupId });
			
		modelBuilder.Entity<LocalUserGroup>()
			.HasOne(ug => ug.Group)
			.WithMany(g => g.UserGroups)
			.HasForeignKey(ug => ug.GroupId)
			.OnDelete(DeleteBehavior.Cascade);
			
		modelBuilder.Entity<LocalUserGroup>()
			.HasOne(ug => ug.UserIdentity)
			.WithMany(ui => ui.UserGroups)
			.HasForeignKey(ug => ug.UserId)
			.OnDelete(DeleteBehavior.Cascade);
			
		modelBuilder.Entity<LocalGroupRole>()
			.HasKey(gr => new { gr.GroupId, gr.RoleId });
			
		modelBuilder.Entity<LocalGroupRole>()
			.HasOne(gr => gr.Group)
			.WithMany()
			.HasForeignKey(gr => gr.GroupId)
			.OnDelete(DeleteBehavior.Cascade);
			
		modelBuilder.Entity<LocalGroupRole>()
			.HasOne(gr => gr.Role)
			.WithMany()
			.HasForeignKey(gr => gr.RoleId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}