using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class Notification
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int NotificationId { get; set; }
		[Required]
		[StringLength(500)]
		public string Message { get; set; }
		[Required]
		public bool IsRead { get; set; }
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public int UserId { get; set; }
		public User User { get; set; }

	}
}
