using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Classes;

namespace StreamingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamingController : ControllerBase
{
	private readonly ILogger<StreamingController> logger;
	private readonly BlobContainerClient containerClient;

	public StreamingController(IConfiguration configuration, ILogger<StreamingController> logger)
	{
		this.logger = logger;
		var connectionString = configuration["AzureBlobStorage:ConnectionString"];
		var blobServiceClient = new BlobServiceClient(connectionString);
		containerClient = blobServiceClient.GetBlobContainerClient(configuration["AzureBlobStorage:ContainerName"]);
		containerClient.CreateIfNotExists();
	}
	
	[HttpGet("{fileName}")]
	public async Task<IActionResult> GetAudiobookChunk(string fileName) //[FromQuery] long offset, [FromQuery] long length
	{
		var audiofileBlob = containerClient.GetBlobClient(fileName);
        
		if (!await audiofileBlob.ExistsAsync())
		{
			return NotFound();
		}
		
		//var properties = await audiofileBlob.GetPropertiesAsync();
		//long audiofileLength = properties.Value.ContentLength;
		//
		// if (length > audiofileLength)
		// 	length = audiofileLength;
		//
		// BlobDownloadOptions options = new BlobDownloadOptions
		// {
		// 	Range = new HttpRange(offset, length)
		// };

		//Download the whole file for now
		var response = await audiofileBlob.DownloadStreamingAsync();
		// var response = await audiofileBlob.DownloadStreamingAsync(options);
		return File(response.Value.Content, "audio/mp3");
	}
}