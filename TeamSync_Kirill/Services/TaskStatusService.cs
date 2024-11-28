using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.DbContext;
using TaskStatus = TeamSync_Kirill.Models.TaskStatus;

namespace TeamSync_Kirill.Services
{
	public interface ITaskStatusService
	{
		Task<TaskStatus?> CreateTaskStatusAsync(TaskStatus taskStatus);
		Task<IEnumerable<TaskStatus>> GetAllTaskStatusesAsync();
		Task<TaskStatus?> GetTaskStatusByIdAsync(int id);
		Task<TaskStatus?> UpdateTaskStatusAsync(int id, TaskStatus taskStatus);
		Task<bool> DeleteTaskStatusAsync(int id);
	}

	public class TaskStatusService : ITaskStatusService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<TaskStatusService> _logger;

		public TaskStatusService(AppDbContext context, ILogger<TaskStatusService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<TaskStatus?> CreateTaskStatusAsync(TaskStatus taskStatus)
		{
			if (taskStatus == null)
			{
				_logger.LogWarning("Attempt to create a null task status");
				return null;
			}

			await _context.Set<TaskStatus>().AddAsync(taskStatus);
			await _context.SaveChangesAsync();
			return taskStatus;
		}

		public async Task<IEnumerable<TaskStatus>> GetAllTaskStatusesAsync()
		{
			return await _context.Set<TaskStatus>().ToListAsync();
		}

		public async Task<TaskStatus?> GetTaskStatusByIdAsync(int id)
		{
			return await _context.Set<TaskStatus>().FindAsync(id);
		}

		public async Task<TaskStatus?> UpdateTaskStatusAsync(int id, TaskStatus taskStatus)
		{
			if (taskStatus == null || id != taskStatus.TaskStatusId)
			{
				_logger.LogWarning("Invalid task status or ID mismatch");
				return null;
			}

			try
			{
				_context.Set<TaskStatus>().Update(taskStatus);
				await _context.SaveChangesAsync();
				return taskStatus;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating task status");
				return null;
			}
		}

		public async Task<bool> DeleteTaskStatusAsync(int id)
		{
			var taskStatus = await _context.Set<TaskStatus>().FindAsync(id);
			if (taskStatus == null)
			{
				_logger.LogInformation($"Task status with ID {id} not found");
				return false;
			}

			_context.Set<TaskStatus>().Remove(taskStatus);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
