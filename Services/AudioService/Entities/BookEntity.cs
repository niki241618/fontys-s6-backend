using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AudioService.Models;

namespace AudioService.Entities;

[Table("BooksInfo")]
public class BookEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	[Required]
	public string Name { get; set; }
	public string Description { get; set; }
	public string Language { get; set; }
	[Required]
	public string Genre { get; set; }
	[Required]
	public ulong Length { get; set; }
	[MinLength(1)]
	public string[] Authors { get; set; }
	[Required]
	[MaxLength(512)]
	public string AudioUri { get; set; }
	
	[MaxLength(512)]
	public string CoverUri { get; set; }

	public ICollection<ReviewEntity> Reviews { get; set; }

	public Book Convert()
	{
		return new Book()
		{
			Id = Id,
			Name = Name,
			Description = Description,
			Language = Language,
			Genre = Genre,
			Authors = Authors,
			Length = Length,
			AudioFileName = AudioUri,
			CoverUri = CoverUri
		};
	}

	public Book Convert(BookRatingInfo ratingInfo)
	{
		Book book = Convert();
		book.RatingInfo = ratingInfo;

		return book;
	}

	public static Book[] ConvertList(IEnumerable<BookEntity> books)
	{
		return books?.Select(b => b.Convert()).ToArray() ?? Array.Empty<Book>();
	}
}