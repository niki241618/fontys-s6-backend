namespace AudioService.Models;

public class Review
{
	public int Id { get; set; }
	public int UserId { get; set; }
	//In rage 1 - 5
	public int Rating { get; set; }
	public string? Message { get; set; }
	public DateTime Timestamp { get; set; }
}