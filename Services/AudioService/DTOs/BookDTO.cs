using Microsoft.AspNetCore.Http;

namespace AudioService.DTOs;

public class BookDTO
{
	public int? Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Language { get; set; }
	public string Genre { get; set; }
	public IFormFile CoverImage { get; set; }
	public IFormFile AudioFile { get; set; }
	public ulong Length { get; set; } = 0;
	public string[] Authors { get; set; }
	public int TotalRatings { get; set; }
	public double AverageRating { get; set; }
}