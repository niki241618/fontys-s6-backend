using AudioService.Models;

namespace AudioService.Services.Interfaces;

public interface IBooksService
{
	public Task<Book[]> GetBooks();
	public Task<Book> GetBook(int id);
	public Task<int> CreateBook(Book toBeCreated);
	public Task DeleteBook(int id);
}