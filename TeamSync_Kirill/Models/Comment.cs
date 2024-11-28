using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class Comment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CommentId { get; set; }
		[Required]
		[StringLength(1000)]
		public string Content { get; set; }
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public int UserId { get; set; }
		public User User { get; set; }
		[Required]
		public int TaskId { get; set; }
		public Task Task { get; set; }


	}
}
