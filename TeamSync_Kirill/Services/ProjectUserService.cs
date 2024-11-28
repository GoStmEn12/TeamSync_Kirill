using TeamSync_Kirill.DbContext;
using TeamSync_Kirill.Models;
using Microsoft.EntityFrameworkCore;

namespace TeamSync_Kirill.Services
{
	public interface IProjectUserService
	{
		Task<ProjectUser?> AddUserToProjectAsync(ProjectUser projectUser);
		Task<IEnumerable<ProjectUser>> GetAllUsersByProjectIdAsync(int projectId);
		Task<bool> RemoveUserFromProjectAsync(int projectId, string userId);
	}

	public class ProjectUserService : IProjectUserService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<ProjectUserService> _logger;

		public ProjectUserService(AppDbContext context, ILogger<ProjectUserService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ProjectUser?> AddUserToProjectAsync(ProjectUser projectUser)
		{
			if (projectUser == null)
			{
				_logger.LogWarning("Attempt to add a null user to a project");
				return null;
			}

			await _context.Set<ProjectUser>().AddAsync(projectUser);
			await _context.SaveChangesAsync();
			return projectUser;
		}

		public async Task<IEnumerable<ProjectUser>> GetAllUsersByProjectIdAsync(int projectId)
		{
			return await _context.Set<ProjectUser>()
				.Include(pu => pu.User)
				.Where(pu => pu.ProjectId == projectId)
				.ToListAsync();
		}

		public async Task<bool> RemoveUserFromProjectAsync(int projectId, string userId)
		{
			var projectUser = await _context.Set<ProjectUser>()
				.FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

			if (projectUser == null)
			{
				_logger.LogInformation($"No user with ID {userId} found in project {projectId}");
				return false;
			}

			_context.Set<ProjectUser>().Remove(projectUser);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
