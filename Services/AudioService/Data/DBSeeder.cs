using AudioService.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.SharedMiddleware;

namespace AudioService.Data;

public class DbSeeder
{
	private readonly BooksContext dbContext;

	public DbSeeder(BooksContext booksContext)
	{
		this.dbContext = booksContext;
	}

	public void Seed()
	{
		dbContext.Database.EnsureCreated();
		
		UpsertBook(new BookEntity
		{
			Id = 1,
			OwnerId = "auth0|665733a439111cabc30c4b36", //Admin User-ID
			Name = "Be Useful: Seven Tools for Life",
			Authors = new[] { "Arnold Schwarzenegger" },
			Description = "The seven rules to follow to realize your true purpose in life—distilled by Arnold Schwarzenegger from his own journey of ceaseless reinvention and extraordinary achievement, and available for absolutely anyone.",
			Language = "English",
			Genre = "Motivation",
			Length = 2132,
			AudioUri = "4c83c2c9-8307-4ee8-a2fe-32b578bd2d0d.mp3",
			CoverUri = "https://audiooasisaudiobookstest.blob.core.windows.net/coverimages/4as1a21-2s3seo2d-212a.png"
		});
		
		UpsertBook(new BookEntity
		{
			Id = 2,
			OwnerId = "auth0|6657334ebdc93dff06695e43", //User User-ID
			Name = "The Black Veil",
			Authors = new []{ "Boz" },
			Description = "It is the year 1800, or thereabouts, and a young doctor has recently opened his own surgery.  All he is waiting for is his first patient.  However, none seem to be forthcoming.",
			Language = "English",
			Genre = "Horror",
			Length = 1234,
			AudioUri = "21af8a8c-a10a-46dd-8f21-041fa267299e.mp3",
			CoverUri = "https://audiooasisaudiobookstest.blob.core.windows.net/coverimages/cb97895a-c052-40d9-9347-b5d8e99353a0.png"
		});
		
		UpsertBook(new BookEntity
		{
			Id = 3,
			OwnerId = "auth0|a652134eb7c93rff06695e5a", //Random ID
			Name = "The 3 Questions",
			Authors = new []{ "Leo Tolstoy" },
			Description = "It once occurred to a certain king that if he always knew the right time to begin everything.",
			Language = "English",
			Genre = "Parable",
			Length = 1324,
			AudioUri = "4c83c2c9-8307-4ee8-a2fe-32b578bd2d0d.mp3",
			CoverUri = "https://audiooasisaudiobookstest.blob.core.windows.net/coverimages/c52017ca-8ebc-4e3c-8d65-532cc5cf715b.jpg"
		});

		using var transaction = dbContext.Database.BeginTransaction();
		try
		{
			dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BooksInfo ON");
			dbContext.SaveChanges();
			dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BooksInfo OFF");
			transaction.Commit();
		}
		catch (Exception)
		{
			transaction.Rollback();
		}
	}
	
	private void UpsertBook(BookEntity book)
	{
		var existingBook = dbContext.Books.FirstOrDefault(x => x.Id == book.Id);
		if (existingBook == null)
		{
			Console.WriteLine("Book with ID {0} not found. Adding new book.", book.Id);
			dbContext.Books.Add(book);
		}
		else
		{
			Console.WriteLine("Book with ID {0} found. Updating book.", book.Id);
			existingBook.Name = book.Name;
			existingBook.OwnerId = book.OwnerId;
			existingBook.Authors = book.Authors;
			existingBook.Description = book.Description;
			existingBook.Language = book.Language;
			existingBook.Genre = book.Genre;
			existingBook.Length = book.Length;
			existingBook.AudioUri = book.AudioUri;
			existingBook.CoverUri = book.CoverUri;
		}
	}
}