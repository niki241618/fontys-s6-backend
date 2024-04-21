using AudioService.Models;

namespace AudioService.Services.Interfaces;

public interface IBooksService
{
	public Task<Book[]> GetBooks();
	public Task<Book> GetBook(int id);
	public Task<int> CreateBook(Book toBeCreated, IFormFile audioFile, IFormFile coverImage);
	public Task DeleteBook(int id);
}