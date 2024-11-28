namespace TeamSync_Kirill.Models
{
	public class TaskUser
	{
		public int TaskId { get; set; }
		public string UserId { get; set; }

		public Task Task { get; set; }
		public User User { get; set; }
	}
}
