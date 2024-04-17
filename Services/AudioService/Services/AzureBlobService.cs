using AudioService.Services.Interfaces;
using Azure.Storage.Blobs;
using Shared.Exceptions;

namespace AudioService.Services;

public class AzureBlobService: IBooksFilesService
{
	private readonly BlobContainerClient audioFilesContainerClient;
	private readonly BlobContainerClient coverImagesContainerClient;

	public AzureBlobService(IConfiguration configuration)
	{
		// Create a BlobServiceClient object which will be used to create a container client
		string connectionString = configuration["AzureBlobStorage:ConnectionString"] ?? throw new NotFoundException("Couldn't find AzureBlobStorage:ConnectionString in appsettings.json");
		BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            
		// Get a reference to a container
		string audioFilesContainerName = configuration["AzureBlobStorage:AudioFilesContainerName"] ?? throw new NotFoundException("Couldn't find AzureBlobStorage:ContainerName in appsettings.json");
		audioFilesContainerClient = blobServiceClient.GetBlobContainerClient(audioFilesContainerName);
            
		string coverImagesContainerName = configuration["AzureBlobStorage:CoverImagesContainerName"] ?? throw new NotFoundException("Couldn't find AzureBlobStorage:CoverImagesContainerName in appsettings.json");
		coverImagesContainerClient = blobServiceClient.GetBlobContainerClient(coverImagesContainerName);
	}

	public async Task<string> UploadAudioFileAsync(string fileName, IFormFile file)
	{
		return await UploadFile(audioFilesContainerClient, fileName, file);
	}

	public async Task<string> UploadCoverImageAsync(string fileName, IFormFile file)
	{
		return await UploadFile(coverImagesContainerClient, fileName, file);
	}

	public async Task DeleteAudioFileAsync(string audiofileBlobName)
	{
		await audioFilesContainerClient.GetBlobClient(audiofileBlobName).DeleteIfExistsAsync();
	}

	public async Task DeleteCoverImageAsync(string bookCoverFileName)
	{
		await coverImagesContainerClient.GetBlobClient(bookCoverFileName).DeleteIfExistsAsync();
	}

	private async Task<string> UploadFile(BlobContainerClient containerClient, string fileName, IFormFile file)
	{
		//Create the container if it doesn't exist
		await containerClient.CreateIfNotExistsAsync();
		BlobClient blobClient = containerClient.GetBlobClient(fileName);
		await using var stream = file.OpenReadStream();
		await blobClient.UploadAsync(stream, true);
		return blobClient.Uri.AbsoluteUri;
	}
}