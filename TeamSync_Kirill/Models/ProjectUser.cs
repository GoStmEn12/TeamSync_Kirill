using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class ProjectUser
	{
		[Key, Column(Order = 0)]
		public int ProjectId { get; set; }
		public Project Project { get; set; }
		[Key, Column(Order = 1)]
		public string UserId { get; set; }
		public User User { get; set; }
	}
}
