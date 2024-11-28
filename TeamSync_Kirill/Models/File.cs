using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeamSync_Kirill.Models
{
	public class File
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int FileId { get; set; }
		[Required]
		[StringLength(255)]
		public string FileName { get; set; }
		[Required]
		[StringLength(500)]
		public string FilePath { get; set; }
		[Required]
		public DateTime UploadedAt { get; set; }
		[Required]
		public int TaskId { get; set; }
		public Task Task { get; set; }
	}
}
