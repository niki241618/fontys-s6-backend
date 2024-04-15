namespace Shared.Classes;

public class Log
{
	public int Id { get; set; }
	public string Message { get; set; }
	public Type LogType { get; set; }
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	public string? Payload { get; set; }

	public Log(string message, Type logType = Type.INFO)
	{
		Message = string.IsNullOrWhiteSpace(message) ? logType.ToString() : message;
		LogType = logType;
	}

	public override string ToString()
	{
		return $"[{Timestamp}] {LogType}: {Message}";
	}

	public enum Type
	{
		INFO,
		WARNING,
		ERROR
	}
}