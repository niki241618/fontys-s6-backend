using System.Runtime.Serialization;

namespace Shared.Exceptions;

public class NotFoundException: AudioOasisException
{
	public NotFoundException()
	{
	}

	protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	public NotFoundException(string? message) : base(message)
	{
	}

	public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}