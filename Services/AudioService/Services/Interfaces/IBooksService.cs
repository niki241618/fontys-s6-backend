using AudioService.Models;

namespace AudioService.Services.Interfaces;

public interface IBooksService
{
	public Task<Book[]> GetBooks();
	public Task<Book[]> GetBooksForUser(string userId);
	public Task<Book> GetBook(int id);
	public Task<Book?> TryGetBook(int id);
	public Task<int> CreateBook(Book toBeCreated, IFormFile audioFile, IFormFile coverImage);
	public Task DeleteBook(int id);
	public Task DeleteBooksForUser(string userId);
}