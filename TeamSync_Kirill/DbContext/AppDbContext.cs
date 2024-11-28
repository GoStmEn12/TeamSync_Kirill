using Microsoft.AspNetCore.Identity;
using TeamSync_Kirill.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TeamSync_Kirill.DbContext
{
	public class AppDbContext : IdentityDbContext<User, UserRole, string>
	{
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Models.File> Files { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<ProjectUser> ProjectUsers { get; set; }
		public DbSet<Models.Task> Tasks { get; set; }
		public DbSet<Models.TaskStatus> TaskStatuses { get; set; }
		public DbSet<TaskUser> TaskUsers { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<IdentityUser>(entity => { entity.Property(e => e.Id).HasMaxLength(450); });
			modelBuilder.Entity<IdentityRole>(entity => { entity.Property(e => e.Id).HasMaxLength(450); });

			modelBuilder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.Property(e => e.UserId).HasMaxLength(450);
				entity.Property(e => e.RoleId).HasMaxLength(450);
				entity.HasKey(e => new { e.UserId, e.RoleId });

				entity.HasOne<User>()
					.WithMany()
					.HasForeignKey(e => e.UserId)
					.OnDelete(DeleteBehavior.NoAction);

				entity.HasOne<UserRole>()
					.WithMany()
					.HasForeignKey(e => e.RoleId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			modelBuilder.Entity<ProjectUser>()
				.HasKey(pu => new { pu.ProjectId, pu.UserId });

			modelBuilder.Entity<ProjectUser>()
				.HasOne(p => p.Project)
				.WithMany(pu => pu.ProjectUsers)
				.HasForeignKey(pu => pu.ProjectId);

			modelBuilder.Entity<ProjectUser>()
				.HasOne(pu => pu.User)
				.WithMany(p => p.ProjectUsers)
				.HasForeignKey(pu => pu.UserId);

			modelBuilder.Entity<Models.Task>()
				.HasOne(t => t.AssignedUser)
				.WithMany()
				.HasForeignKey(t => t.AssignedUserId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Models.Task>()
				.HasOne(t => t.Project)
				.WithMany(p => p.Tasks)
				.HasForeignKey(t => t.ProjectId);

			modelBuilder.Entity<Models.Task>()
				.HasOne(t => t.Status)
				.WithMany(ts => ts.Tasks)
				.HasForeignKey(t => t.StatusId);

			modelBuilder.Entity<User>()
				.HasIndex(e => e.Email)
				.IsUnique();

			modelBuilder.Entity<UserRole>()
				.HasMany(u => u.Users)
				.WithOne(r => r.Role)
				.HasForeignKey(r => r.RoleId);

			modelBuilder.Entity<TaskUser>()
	.HasKey(tu => new { tu.TaskId, tu.UserId }); // Составной ключ

			modelBuilder.Entity<TaskUser>()
				.HasOne(tu => tu.Task)
				.WithMany(t => t.TaskUsers)
				.HasForeignKey(tu => tu.TaskId);

			modelBuilder.Entity<TaskUser>()
				.HasOne(tu => tu.User)
				.WithMany(u => u.TaskUsers)
				.HasForeignKey(tu => tu.UserId);
		}
	}

}
