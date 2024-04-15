using System.Runtime.Serialization;

namespace Shared.Exceptions;

public class AudioOasisException : Exception
{
	public AudioOasisException()
	{
	}

	protected AudioOasisException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	public AudioOasisException(string? message) : base(message)
	{
	}

	public AudioOasisException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}