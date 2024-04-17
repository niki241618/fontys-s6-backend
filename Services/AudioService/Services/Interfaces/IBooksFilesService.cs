namespace AudioService.Services.Interfaces;

public interface IBooksFilesService
{
	/// <summary>
	/// Uploads an audio file to the Azure Blob Storage.
	/// </summary>
	/// <param name="fileName">Name of the file</param>
	/// <param name="file">Audiofile</param>
	/// <returns>URI to the file location</returns>
	public Task<string> UploadAudioFileAsync(string fileName, IFormFile file);
	/// <summary>
	/// Upload a cover image to the Azure Blob Storage.
	/// </summary>
	/// <param name="fileName">Name of the file</param>
	/// <param name="file">Cover image</param>
	/// <returns>URI to the file location</returns>
	public Task<string> UploadCoverImageAsync(string fileName, IFormFile file);

	public Task DeleteAudioFileAsync(string audiofileBlobName);
	public Task DeleteCoverImageAsync(string bookCoverFileName);
}