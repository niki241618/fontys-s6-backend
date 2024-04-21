using AudioService.Data;
using AudioService.Entities;
using AudioService.Models;
using AudioService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;

namespace AudioService.Services;

public class BooksService: IBooksService
{
	private readonly BooksContext dbContext;
	private readonly IBooksFilesService booksFilesService;

	public BooksService(BooksContext dbContext, IBooksFilesService booksFilesService)
	{
		this.dbContext = dbContext;
		this.booksFilesService = booksFilesService;
	}
	
	public async Task<Book[]> GetBooks()
	{
		List<BookEntity> bookEntities = await dbContext.Books.ToListAsync();
		
		List<Book> books = new List<Book>();
		foreach (var bookEntity in bookEntities)
		{
			BookRatingInfo ratingInfo = await GetRatingStatsForBook(bookEntity.Id);
			var book = bookEntity.Convert(ratingInfo);
			books.Add(book);
		}

		return books.ToArray();
	}

	public async Task<Book> GetBook(int id)
	{
		BookEntity book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id) ?? throw new NotFoundException($"Book with ID {id} is not found.");
		BookRatingInfo ratingInfo = await GetRatingStatsForBook(id);
		
		return book.Convert(ratingInfo) ;
	}

	public async Task<int> CreateBook(Book toBeCreated, IFormFile audioFile, IFormFile coverImage)
	{
		BookEntity bookEntity = new BookEntity
		{
			Name = toBeCreated.Name,
			Description = toBeCreated.Description,
			Language = toBeCreated.Language,
			Genre = toBeCreated.Genre,
			Authors = toBeCreated.Authors,
			Length = toBeCreated.Length,
		};
		
		string audiofileBlobName = GenerateBlobName(audioFile.FileName);
		
		await booksFilesService.UploadAudioFileAsync(audiofileBlobName, audioFile);
		bookEntity.AudioUri = audiofileBlobName;
		
		if (coverImage.Length > 0)
		{
			try
			{
				string coverImageBlobName = GenerateBlobName(coverImage.FileName);
				
				string uri = await booksFilesService.UploadCoverImageAsync(coverImageBlobName, coverImage);
				bookEntity.CoverUri = uri;
			}
			catch (Exception)
			{
				await booksFilesService.DeleteAudioFileAsync(audiofileBlobName);
				throw;
			}
		}
		
		await dbContext.Books.AddAsync(bookEntity);
		await dbContext.SaveChangesAsync();
		return bookEntity.Id;
	}

	public async Task DeleteBook(int id)
	{
		BookEntity book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id) 
		                  ?? throw new NotFoundException($"Book with ID {id} is not found.");
		
		dbContext.Books.Remove(book);
		await booksFilesService.DeleteAudioFileAsync(book.AudioUri);
		
		string coverImageBlobName = book.CoverUri.Split('/').Last();
		await booksFilesService.DeleteCoverImageAsync(coverImageBlobName);
		
		await dbContext.SaveChangesAsync();
	}

	private async Task<BookRatingInfo> GetRatingStatsForBook(int bookId)
	{
		int totalRatings = await dbContext.Reviews.Where(x => x.BookId == bookId).CountAsync();
		double averageRating = await dbContext.Reviews
			.Where(x => x.BookId == bookId)
			.AverageAsync(x => (int?)x.Rating) ?? 0;

		averageRating = Math.Round(averageRating, 2);

		return new BookRatingInfo
		{
			TotalRatings = totalRatings,
			AverageRating = averageRating
		};
	}
	
	private string GenerateBlobName(string fileName)
	{
		return $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
	}
}