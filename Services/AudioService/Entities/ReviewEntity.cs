using System.ComponentModel.DataAnnotations.Schema;
using AudioService.Models;
using Microsoft.Build.Framework;

namespace AudioService.Entities;

public class ReviewEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	[Required]
	public int UserId { get; set; }
	//In rage 1 - 5
	[Required]
	public int Rating { get; set; }
	public string? Message { get; set; }
	[Required]
	public DateTime Timestamp { get; set; }
	public int BookId { get; set; }
	public BookEntity BookEntity { get; set; }

	public Review Convert()
	{
		return new Review
		{
			Id = Id,
			UserId = UserId,
			Rating = Rating,
			Message = Message,
			Timestamp = Timestamp
		};
	}

	public static Review[] ConvertList(IEnumerable<ReviewEntity> reviews)
	{
		return reviews?.Select(reviewEntity => reviewEntity.Convert()).ToArray() ?? Array.Empty<Review>();
	}
}