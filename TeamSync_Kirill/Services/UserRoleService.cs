using Microsoft.AspNetCore.Identity;
using TeamSync_Kirill.Models;
using Microsoft.EntityFrameworkCore;


namespace TeamSync_Kirill.Services
{
	public interface IUserRoleService
	{
		Task<UserRole?> CreateUserRoleAsync(UserRole userRole);
		Task<IEnumerable<UserRole>> GetAllUserRolesAsync();
		Task<UserRole?> GetUserRoleByIdAsync(string id);
		Task<UserRole?> UpdateUserRoleAsync(string id, UserRole userRole);
		Task<bool> DeleteUserRoleAsync(string id);
	}

	public class UserRoleService : IUserRoleService
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<UserRole> _roleManager;
		private readonly ILogger<UserRoleService> _logger;

		public UserRoleService(
			UserManager<User> userManager,
			RoleManager<UserRole> roleManager,
			ILogger<UserRoleService> logger)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_logger = logger;
		}

		public async Task<UserRole?> CreateUserRoleAsync(UserRole userRole)
		{
			if (userRole == null)
			{
				_logger.LogWarning("Attempt to create a user role with null");
				return null;
			}

			var result = await _roleManager.CreateAsync(userRole);
			if (result.Succeeded)
			{
				return userRole;
			}

			_logger.LogWarning($"Failed to create user role: {string.Join(", ", result.Errors)}");
			return null;
		}

		public async Task<IEnumerable<UserRole>> GetAllUserRolesAsync()
		{
			return await _roleManager.Roles.ToListAsync();
		}

		public async Task<UserRole?> GetUserRoleByIdAsync(string id)
		{
			return await _roleManager.FindByIdAsync(id);
		}

		public async Task<UserRole?> UpdateUserRoleAsync(string id, UserRole userRole)
		{
			if (userRole == null || id != userRole.Id)
			{
				_logger.LogWarning("Invalid user role or ID mismatch");
				return null;
			}

			var existingRole = await _roleManager.FindByIdAsync(id);
			if (existingRole == null)
			{
				_logger.LogInformation("User role not found");
				return null;
			}

			existingRole.Name = userRole.Name;

			var result = await _roleManager.UpdateAsync(existingRole);
			if (result.Succeeded)
			{
				return existingRole;
			}

			_logger.LogWarning($"Failed to update user role: {string.Join(", ", result.Errors)}");
			return null;
		}

		public async Task<bool> DeleteUserRoleAsync(string id)
		{
			var userRole = await _roleManager.FindByIdAsync(id);
			if (userRole == null)
			{
				_logger.LogInformation("User role not found");
				return false;
			}

			var result = await _roleManager.DeleteAsync(userRole);
			if (result.Succeeded)
			{
				return true;
			}

			_logger.LogWarning($"Failed to delete user role: {string.Join(", ", result.Errors)}");
			return false;
		}
	}
}
