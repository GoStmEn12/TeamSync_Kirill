using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.DbContext;
using TeamSync_Kirill.Models;

namespace TeamSync_Kirill.Services
{
	public interface ICommentService
	{
		Task<Comment?> CreateCommentAsync(Comment comment);
		Task<IEnumerable<Comment>> GetAllCommentsAsync();
		Task<Comment?> GetCommentByIdAsync(int id);
		Task<Comment?> UpdateCommentAsync(int id, Comment comment);
		Task<bool> DeleteCommentAsync(int id);
	}

	public class CommentService : ICommentService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<CommentService> _logger;

		public CommentService(AppDbContext context, ILogger<CommentService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Comment?> CreateCommentAsync(Comment comment)
		{
			if (comment == null)
			{
				_logger.LogWarning("Attempt to create a comment with null data");
				return null;
			}

			await _context.Set<Comment>().AddAsync(comment);
			await _context.SaveChangesAsync();
			return comment;
		}

		public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
		{
			return await _context.Set<Comment>().Include(c => c.User).Include(c => c.Task).ToListAsync();
		}

		public async Task<Comment?> GetCommentByIdAsync(int id)
		{
			return await _context.Set<Comment>()
				.Include(c => c.User)
				.Include(c => c.Task)
				.FirstOrDefaultAsync(c => c.CommentId == id);
		}

		public async Task<Comment?> UpdateCommentAsync(int id, Comment comment)
		{
			if (comment == null || id != comment.CommentId)
			{
				_logger.LogWarning("Invalid comment or ID mismatch");
				return null;
			}

			try
			{
				_context.Set<Comment>().Update(comment);
				await _context.SaveChangesAsync();
				return comment;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating comment");
				return null;
			}
		}

		public async Task<bool> DeleteCommentAsync(int id)
		{
			var comment = await _context.Set<Comment>().FindAsync(id);
			if (comment == null)
			{
				_logger.LogInformation("Comment not found");
				return false;
			}

			_context.Set<Comment>().Remove(comment);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
