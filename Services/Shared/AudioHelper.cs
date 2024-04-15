using Microsoft.AspNetCore.Http;
using NAudio.Wave;

namespace Shared;

public static class AudioHelper
{
	public static ulong GetAudioDurationInSeconds(IFormFile audioFile)
	{
		using var reader = new Mp3FileReader(audioFile.OpenReadStream());
		return (ulong)reader.TotalTime.TotalSeconds;
	}

	public static bool IsAudiofile(IFormFile file)
	{
		if (file == null || file.Length <= 0) 
			return false; // If file is null or empty, it's not an audio file
		
		// Get the MIME type of the file
		var contentType = file.ContentType;

		// Check if the MIME type starts with "audio/"
		return contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);

	}
}