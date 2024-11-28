using Microsoft.EntityFrameworkCore;
using TeamSync_Kirill.DbContext;
using File = TeamSync_Kirill.Models.File;

namespace TeamSync_Kirill.Services
{
	public interface IFileService
	{
		Task<File?> CreateFileAsync(File file);
		Task<IEnumerable<File>> GetAllFilesAsync();
		Task<File?> GetFileByIdAsync(int id);
		Task<File?> UpdateFileAsync(int id, File file);
		Task<bool> DeleteFileAsync(int id);
	}

	public class FileService : IFileService
	{
		private readonly AppDbContext _context;
		private readonly ILogger<FileService> _logger;

		public FileService(AppDbContext context, ILogger<FileService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<File?> CreateFileAsync(File file)
		{
			if (file == null)
			{
				_logger.LogWarning("Attempt to create a file with null");
				return null;
			}
			await _context.Set<File>().AddAsync(file);
			await _context.SaveChangesAsync();
			return file;
		}

		public async Task<IEnumerable<File>> GetAllFilesAsync()
		{
			return await _context.Set<File>().ToListAsync();
		}

		public async Task<File?> GetFileByIdAsync(int id)
		{
			return await _context.Set<File>().FindAsync(id);
		}

		public async Task<File?> UpdateFileAsync(int id, File file)
		{
			if (file == null || id != file.FileId)
			{
				_logger.LogWarning("Invalid file or ID mismatch");
				return null;
			}
			try
			{
				_context.Set<File>().Update(file);
				await _context.SaveChangesAsync();
				return file;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency error while updating file");
				return null;
			}
		}

		public async Task<bool> DeleteFileAsync(int id)
		{
			var file = await _context.Set<File>().FindAsync(id);
			if (file == null)
			{
				_logger.LogInformation("File not found");
				return false;
			}
			_context.Set<File>().Remove(file);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
