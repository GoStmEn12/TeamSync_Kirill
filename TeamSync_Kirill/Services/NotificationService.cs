using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.DbContext;
using TeamSync_Kirill.Models;

namespace TeamSync_Kirill.Services
{
	public interface INotificationService
	{
		Task<Notification?> CreateNotificationAsync(Notification notification);
		Task<IEnumerable<Notification>> GetAllNotificationsAsync();
		Task<Notification?> GetNotificationByIdAsync(int id);
		Task<Notification?> UpdateNotificationAsync(int id, Notification notification);
		Task<bool> DeleteNotificationAsync(int id);
	}

	public class NotificationService : INotificationService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<NotificationService> _logger;

		public NotificationService(AppDbContext context, ILogger<NotificationService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Notification?> CreateNotificationAsync(Notification notification)
		{
			if (notification == null)
			{
				_logger.LogWarning("Attempt to create a notification with null");
				return null;
			}
			await _context.Set<Notification>().AddAsync(notification);
			await _context.SaveChangesAsync();
			return notification;
		}

		public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
		{
			return await _context.Set<Notification>().ToListAsync();
		}

		public async Task<Notification?> GetNotificationByIdAsync(int id)
		{
			return await _context.Set<Notification>().FindAsync(id);
		}

		public async Task<Notification?> UpdateNotificationAsync(int id, Notification notification)
		{
			if (notification == null || id != notification.NotificationId)
			{
				_logger.LogWarning("Invalid notification or ID mismatch");
				return null;
			}
			try
			{
				_context.Set<Notification>().Update(notification);
				await _context.SaveChangesAsync();
				return notification;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating notification");
				return null;
			}
		}

		public async Task<bool> DeleteNotificationAsync(int id)
		{
			var notification = await _context.Set<Notification>().FindAsync(id);
			if (notification == null)
			{
				_logger.LogInformation("Notification not found");
				return false;
			}
			_context.Set<Notification>().Remove(notification);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
