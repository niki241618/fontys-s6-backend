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

	public async Task<Book[]> GetBooksForUser(string userId)
	{
		return await dbContext.Books
			.Where(b => b.OwnerId == userId)
			.Select(b => b.Convert())
			.ToArrayAsync();
	}

	public async Task<Book> GetBook(int id)
	{
		BookEntity book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id) ?? throw new NotFoundException($"Book with ID {id} is not found.");
		BookRatingInfo ratingInfo = await GetRatingStatsForBook(id);
		
		return book.Convert(ratingInfo) ;
	}

	public async Task<Book?> TryGetBook(int id)
	{
		BookEntity? book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
		if(book == null)
			return null;
		
		BookRatingInfo ratingInfo = await GetRatingStatsForBook(id);
		
		return book.Convert(ratingInfo);
	}

	public async Task<int> CreateBook(Book toBeCreated, IFormFile audioFile, IFormFile coverImage)
	{
		BookEntity bookEntity = new BookEntity
		{
			Name = toBeCreated.Name,
			OwnerId = toBeCreated.OwnerId,
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
		// Fetch the book entity
		var book = await dbContext.Books.SingleOrDefaultAsync(b => b.Id == id);
    
		// Check if the book was found
		if (book == null)
		{
			throw new NotFoundException($"Book with ID {id} is not found.");
		}

		// Begin a transaction to ensure atomic operations
		await using var transaction = await dbContext.Database.BeginTransactionAsync();
		try
		{
			// Remove the book entity from the database
			dbContext.Books.Remove(book);

			// Delete associated files asynchronously
			await Task.WhenAll(
				booksFilesService.DeleteAudioFileAsync(book.AudioUri),
				booksFilesService.DeleteCoverImageAsync(book.CoverUri.Split('/').Last())
			);

			// Save changes in the database
			await dbContext.SaveChangesAsync();

			// Commit the transaction
			await transaction.CommitAsync();
		}
		catch (Exception)
		{
			// Rollback the transaction in case of an error
			await transaction.RollbackAsync();
			
			throw; // Rethrow the exception to handle it further up the call stack if necessary
		}
	}

	public async Task DeleteBooksForUser(string userId)
	{
		var booksToDelete = dbContext.Books.Where(b => b.OwnerId == userId);
		if (booksToDelete.Any())
		{
			dbContext.Books.RemoveRange(booksToDelete);
			await dbContext.SaveChangesAsync();
		}
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