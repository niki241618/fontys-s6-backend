using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Classes;

namespace LoggingService.Entities;

public class LogEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	[Required]
	public DateTime Timestamp { get; set; }
	public Log.Type LogType { get; set; } = Log.Type.INFO;
	public string Message { get; set; }
	public string? Payload { get; set; }

	public LogEntity()
	{
	}
	
	public LogEntity(Log log)
	{
		Timestamp = log.Timestamp;
		LogType = log.LogType;
		Message = log.Message;
		Payload = log.Payload;
	}

	public Log Convert()
	{
		return new Log(Message, LogType)
		{
			Id = Id,
			Payload = Payload,
			Timestamp = Timestamp
		};
	}
}