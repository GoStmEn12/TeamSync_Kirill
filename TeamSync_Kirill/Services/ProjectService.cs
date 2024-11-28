using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.DbContext;
using TeamSync_Kirill.Models;

namespace TeamSync_Kirill.Services
{
	public interface IProjectService
	{
		Task<Project?> CreateProjectAsync(Project project);
		Task<IEnumerable<Project>> GetAllProjectsAsync();
		Task<Project?> GetProjectByIdAsync(int id);
		Task<Project?> UpdateProjectAsync(int id, Project project);
		Task<bool> DeleteProjectAsync(int id);
	}

	public class ProjectService : IProjectService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<ProjectService> _logger;

		public ProjectService(AppDbContext context, ILogger<ProjectService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Project?> CreateProjectAsync(Project project)
		{
			if (project == null)
			{
				_logger.LogWarning("Attempt to create a project with null");
				return null;
			}
			await _context.Set<Project>().AddAsync(project);
			await _context.SaveChangesAsync();
			return project;
		}

		public async Task<IEnumerable<Project>> GetAllProjectsAsync()
		{
			return await _context.Set<Project>().Include(p => p.Tasks).Include(p => p.ProjectUsers).ToListAsync();
		}

		public async Task<Project?> GetProjectByIdAsync(int id)
		{
			return await _context.Set<Project>().Include(p => p.Tasks).Include(p => p.ProjectUsers).FirstOrDefaultAsync(p => p.ProjectId == id);
		}

		public async Task<Project?> UpdateProjectAsync(int id, Project project)
		{
			if (project == null || id != project.ProjectId)
			{
				_logger.LogWarning("Invalid project or ID mismatch");
				return null;
			}
			try
			{
				_context.Set<Project>().Update(project);
				await _context.SaveChangesAsync();
				return project;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating project");
				return null;
			}
		}

		public async Task<bool> DeleteProjectAsync(int id)
		{
			var project = await _context.Set<Project>().FindAsync(id);
			if (project == null)
			{
				_logger.LogInformation("Project not found");
				return false;
			}
			_context.Set<Project>().Remove(project);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
