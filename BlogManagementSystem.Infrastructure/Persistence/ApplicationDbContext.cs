using BlogManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementSystem.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Post> Posts { get; set; }
	public DbSet<Comment> Comments { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
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
	}
}