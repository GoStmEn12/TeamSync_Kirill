using Microsoft.AspNetCore.Identity;

namespace TeamSync_Kirill.Models
{
	public class User : IdentityUser
	{
		public string? RoleId { get; set; }
		public UserRole Role { get; set; }
		public ICollection<ProjectUser> ProjectUsers { get; set; }
		public ICollection<TaskUser> TaskUsers { get; set; }
		public ICollection<Notification> Notifications { get; set; }
		public ICollection<Comment> Comments { get; set; }
	}
}
