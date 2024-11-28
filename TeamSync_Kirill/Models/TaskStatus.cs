using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class TaskStatus
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int TaskStatusId { get; set; }
		[Required]
		[StringLength(50)]
		public string Name { get; set; }
		public ICollection<Task> Tasks { get; set; }
	}
}
