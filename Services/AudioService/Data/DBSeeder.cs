using AudioService.Entities;
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
		dbContext.Books.AddRange(new BookEntity
		{
			Id = 1,
			Name = "Be Useful: Seven Tools for Life",
			Authors = new []{ "Arnold Schwarzenegger" },
			Description = "The seven rules to follow to realize your true purpose in life—distilled by Arnold Schwarzenegger from his own journey of ceaseless reinvention and extraordinary achievement, and available for absolutely anyone.",
			Language = "English",
			Genre = "Motivation",
			Length = 2132,
			AudioUri = "4c83c2c9-8307-4ee8-a2fe-32b578bd2d0d.mp3",
			CoverUri = "https://audiooasisaudiobookstest.blob.core.windows.net/coverimages/4as1a21-2s3seo2d-212a.png"
		}, 
		new BookEntity
		{
			Id = 2,
			Name = "The Black Veil",
			Authors = new []{ "Boz" },
			Description = "It is the year 1800, or thereabouts, and a young doctor has recently opened his own surgery.  All he is waiting for is his first patient.  However, none seem to be forthcoming.",
			Language = "English",
			Genre = "Horror",
			Length = 1234,
			AudioUri = "21af8a8c-a10a-46dd-8f21-041fa267299e.mp3",
			CoverUri = "https://audiooasisaudiobookstest.blob.core.windows.net/coverimages/cb97895a-c052-40d9-9347-b5d8e99353a0.png"
		},
		new BookEntity
		{
			Id = 3,
			Name = "The 3 Questions",
			Authors = new []{ "Leo Tolstoy" },
			Description = "It once occurred to a certain king that if he always knew the right time to begin everything.",
			Language = "English",
			Genre = "Parable",
			Length = 1324,
			AudioUri = "4c83c2c9-8307-4ee8-a2fe-32b578bd2d0d.mp3",
			CoverUri = "https://audiooasisaudiobookstest.blob.core.windows.net/coverimages/c52017ca-8ebc-4e3c-8d65-532cc5cf715b.jpg"
		});
		dbContext.SaveChanges();
	}
}