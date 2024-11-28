using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.DbContext;
using Task = TeamSync_Kirill.Models.Task;

namespace TeamSync_Kirill.Services
{
	public interface ITaskService
	{
		Task<Task?> CreateTaskAsync(Task task);
		Task<IEnumerable<Task>> GetAllTasksAsync();
		Task<Task?> GetTaskByIdAsync(int id);
		Task<Task?> UpdateTaskAsync(int id, Task task);
		Task<bool> DeleteTaskAsync(int id);
	}

	public class TaskService : ITaskService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<TaskService> _logger;

		public TaskService(AppDbContext context, ILogger<TaskService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Task?> CreateTaskAsync(Task task)
		{
			if (task == null)
			{
				_logger.LogWarning("Attempt to create a task with null");
				return null;
			}
			await _context.Set<Task>().AddAsync(task);
			await _context.SaveChangesAsync();
			return task;
		}

		public async Task<IEnumerable<Task>> GetAllTasksAsync()
		{
			return await _context.Set<Task>().Include(t => t.Comments).Include(t => t.Files).Include(t => t.TaskUsers).Include(t => t.Status).ToListAsync();
		}

		public async Task<Task?> GetTaskByIdAsync(int id)
		{
			return await _context.Set<Task>().Include(t => t.Comments).Include(t => t.Files).Include(t => t.TaskUsers).Include(t => t.Status).FirstOrDefaultAsync(t => t.TaskId == id);
		}

		public async Task<Task?> UpdateTaskAsync(int id, Task task)
		{
			if (task == null || id != task.TaskId)
			{
				_logger.LogWarning("Invalid task or ID mismatch");
				return null;
			}
			try
			{
				_context.Set<Task>().Update(task);
				await _context.SaveChangesAsync();
				return task;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating task");
				return null;
			}
		}

		public async Task<bool> DeleteTaskAsync(int id)
		{
			var task = await _context.Set<Task>().FindAsync(id);
			if (task == null)
			{
				_logger.LogInformation("Task not found");
				return false;
			}
			_context.Set<Task>().Remove(task);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
