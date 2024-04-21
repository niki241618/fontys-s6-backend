using Microsoft.AspNetCore.Http;

namespace AudioOasisTests;

public class Mp3AudioGenerator
{
	public static IFormFile GenerateDummyMp3File(string fileName = "dummy.mp3")
	{
		// Create dummy audio data
		byte[] dummyAudioData = GenerateDummyAudioData();

		// Create a MemoryStream from the dummy audio data
		var memoryStream = new MemoryStream(dummyAudioData);
		return new FormFile(memoryStream, 0, dummyAudioData.Length, "audioFile", fileName)
		{
			Headers = new HeaderDictionary(),
			ContentType = "audio/mpeg"
		};
	}

	// Here, I'm just creating a simple dummy MP3 with some arbitrary bytes.
	private static byte[] GenerateDummyAudioData()
	{
		const int fileSizeInBytes = 1024; // Size of the dummy audio file in bytes
		byte[] dummyAudioData = new byte[fileSizeInBytes];
		
		// Let's just fill the array with some arbitrary data
		for (int i = 0; i < fileSizeInBytes; i++)
		{
			dummyAudioData[i] = (byte)(i % 256);
		}

		return dummyAudioData;
	}
}