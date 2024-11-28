using Microsoft.AspNetCore.Identity;

namespace TeamSync_Kirill.Models
{
	public class UserRole : IdentityRole
	{
		public ICollection<User> Users { get; set; }
	}
}
