namespace AudioService.Models;

public struct BookRatingInfo
{
	public int TotalRatings { get; set; }
	public double AverageRating { get; set; }

	public BookRatingInfo(int totalRatings, double averageRating)
	{
		TotalRatings = totalRatings;
		AverageRating = averageRating;
	}
}