using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class Task
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int TaskId { get; set; }
		[Required]
		[StringLength(100)]
		public string Title { get; set; }
		[StringLength(500)]
		public string Description { get; set; }
		[Required]
		public DateTime DueDate { get; set; }
		[Required]
		public int StatusId { get; set; }
		public TaskStatus Status { get; set; }
		public string AssignedUserId { get; set; }
		public User AssignedUser { get; set; }
		[Required]
		public int ProjectId { get; set; }
		public Project Project { get; set; }
		public ICollection<Comment> Comments { get; set; }
		public ICollection<File> Files { get; set; }
		public ICollection<TaskUser> TaskUsers { get; set; }
	}
}
