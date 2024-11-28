using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class Project
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ProjectId { get; set; }
		[Required]
		[StringLength(100)]
		public string Name { get; set; }
		[StringLength(500)]
		public string Description { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public ICollection<Task> Tasks { get; set; }
		public ICollection<ProjectUser> ProjectUsers { get; set; }
	}
}
