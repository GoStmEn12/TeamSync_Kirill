using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.Models;

namespace TeamSync_Kirill.Services
{
	public interface IUserService
	{
		Task<User?> CreateUserAsync(User user, string password);
		Task<IEnumerable<User>> GetAllUsersAsync();
		Task<User?> GetUserByIdAsync(string id);
		Task<User?> UpdateUserAsync(string id, User user);
		Task<bool> DeleteUserAsync(string id);
	}

	public class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly ILogger<UserService> _logger;

		public UserService(UserManager<User> userManager, ILogger<UserService> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		public async Task<User?> CreateUserAsync(User user, string password)
		{
			if (user == null)
			{
				_logger.LogWarning("Attempt to create a user with null data");
				return null;
			}

			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				_logger.LogInformation("User created successfully");
				return user;
			}

			_logger.LogWarning("Failed to create user: {0}", string.Join(", ", result.Errors));
			return null;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _userManager.Users.Include(u => u.Role).Include(u => u.ProjectUsers).Include(u => u.TaskUsers).Include(u => u.Notifications).Include(u => u.Comments).ToListAsync();
		}

		public async Task<User?> GetUserByIdAsync(string id)
		{
			return await _userManager.Users.Include(u => u.Role).Include(u => u.ProjectUsers).Include(u => u.TaskUsers).Include(u => u.Notifications).Include(u => u.Comments).FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<User?> UpdateUserAsync(string id, User user)
		{
			if (user == null || id != user.Id)
			{
				_logger.LogWarning("Invalid user or ID mismatch");
				return null;
			}

			try
			{
				var existingUser = await _userManager.FindByIdAsync(id);
				if (existingUser == null)
				{
					_logger.LogWarning("User not found");
					return null;
				}

				existingUser.UserName = user.UserName;
				existingUser.Email = user.Email;
				existingUser.RoleId = user.RoleId;
				existingUser.Role = user.Role;

				var result = await _userManager.UpdateAsync(existingUser);
				if (result.Succeeded)
				{
					_logger.LogInformation("User updated successfully");
					return existingUser;
				}
				else
				{
					_logger.LogWarning("Failed to update user: {0}", string.Join(", ", result.Errors));
					return null;
				}
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating user");
				return null;
			}
		}

		public async Task<bool> DeleteUserAsync(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				_logger.LogInformation("User not found");
				return false;
			}

			var result = await _userManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				_logger.LogInformation("User deleted successfully");
				return true;
			}
			else
			{
				_logger.LogWarning("Failed to delete user: {0}", string.Join(", ", result.Errors));
				return false;
			}
		}
	}
}
