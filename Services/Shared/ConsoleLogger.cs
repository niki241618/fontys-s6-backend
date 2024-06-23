using Shared.Classes;

namespace Shared;

public class ConsoleLogger : ILogger
{
	public void Log(Log log)
	{
		Console.WriteLine("Console Logger: " + log.Message);
	}
}