using TeamSync_Kirill.DbContext;
using TeamSync_Kirill.Models;
using Microsoft.EntityFrameworkCore;

namespace TeamSync_Kirill.Services
{
	public interface ITaskUserService
	{
		Task<TaskUser?> AssignUserToTaskAsync(TaskUser taskUser);
		Task<IEnumerable<TaskUser>> GetAllUsersByTaskIdAsync(int taskId);
		Task<bool> RemoveUserFromTaskAsync(int taskId, string userId);
	}

	public class TaskUserService : ITaskUserService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<TaskUserService> _logger;

		public TaskUserService(AppDbContext context, ILogger<TaskUserService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<TaskUser?> AssignUserToTaskAsync(TaskUser taskUser)
		{
			if (taskUser == null)
			{
				_logger.LogWarning("Attempt to assign a null user to a task");
				return null;
			}

			await _context.Set<TaskUser>().AddAsync(taskUser);
			await _context.SaveChangesAsync();
			return taskUser;
		}

		public async Task<IEnumerable<TaskUser>> GetAllUsersByTaskIdAsync(int taskId)
		{
			return await _context.Set<TaskUser>()
				.Include(tu => tu.User)
				.Where(tu => tu.TaskId == taskId)
				.ToListAsync();
		}

		public async Task<bool> RemoveUserFromTaskAsync(int taskId, string userId)
		{
			var taskUser = await _context.Set<TaskUser>()
				.FirstOrDefaultAsync(tu => tu.TaskId == taskId && tu.UserId == userId);

			if (taskUser == null)
			{
				_logger.LogInformation($"No user with ID {userId} assigned to task {taskId}");
				return false;
			}

			_context.Set<TaskUser>().Remove(taskUser);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
