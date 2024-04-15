namespace AudioService.Models;

public class Book
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Language { get; set; }
	public string Genre { get; set; }
	public ulong Length { get; set; } //Length in seconds
	public string[] Authors { get; set; }
	public string AudioFileName { get; set; }
	public string CoverUri { get; set; }
	public BookRatingInfo RatingInfo { get; set; }
}