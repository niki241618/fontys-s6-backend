using AudioService.DTOs;
using AudioService.Models;
using AudioService.Services.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Classes;
using Shared.Exceptions;

namespace AudioService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudiobooksController : ControllerBase
    {
        private readonly IBooksService booksService;
        private readonly BlobContainerClient audioFilesContainerClient;
        private readonly BlobContainerClient coverImagesContainerClient;

        public AudiobooksController(IBooksService booksService, IConfiguration configuration)
        {
            this.booksService = booksService;
            
            // Create a BlobServiceClient object which will be used to create a container client
            string connectionString = configuration["AzureBlobStorage:ConnectionString"] ?? throw new NotFoundException("Couldn't find AzureBlobStorage:ConnectionString in appsettings.json");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            
            // Get a reference to a container
            string audioFilesContainerName = configuration["AzureBlobStorage:AudioFilesContainerName"] ?? throw new NotFoundException("Couldn't find AzureBlobStorage:ContainerName in appsettings.json");
            audioFilesContainerClient = blobServiceClient.GetBlobContainerClient(audioFilesContainerName);
            
            string coverImagesContainerName = configuration["AzureBlobStorage:CoverImagesContainerName"] ?? throw new NotFoundException("Couldn't find AzureBlobStorage:CoverImagesContainerName in appsettings.json");
            coverImagesContainerClient = blobServiceClient.GetBlobContainerClient(coverImagesContainerName);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            return Ok(await booksService.GetBooks());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBook(int id)
        {
            return Ok(await booksService.GetBook(id));
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 724288000)] //690 mb
        public async Task<IActionResult> CreateBook([FromForm] BookDTO bookDto)
        {
            if (bookDto.AudioFile.Length == 0)
                return BadRequest("No audio file uploaded.");
            
            if(!AudioHelper.IsAudiofile(bookDto.AudioFile))
                return BadRequest("The uploaded file is not an audio file.");
            
            Book book = new Book
            {
                Name = bookDto.Name,
                Description = bookDto.Description,
                Language = bookDto.Language,
                Genre = bookDto.Genre,
                Authors = bookDto.Authors,
                Length = AudioHelper.GetAudioDurationInSeconds(bookDto.AudioFile)
            };
            
            // Create the container if it doesn't exist
            await audioFilesContainerClient.CreateIfNotExistsAsync();

            // Get a unique name for the blob
            string audiofileBlobName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.AudioFile.FileName)}";

            // Get a reference to a blob
            BlobClient audiofileBlobClient = audioFilesContainerClient.GetBlobClient(audiofileBlobName);

            // Upload the file to blob storage
            await using (var stream = bookDto.AudioFile.OpenReadStream())
            {
                await audiofileBlobClient.UploadAsync(stream, true);
            }
            
            book.AudioFileName = audiofileBlobName;

            //If a cover image was uploaded
            if (bookDto.CoverImage.Length > 0)
            {
                await coverImagesContainerClient.CreateIfNotExistsAsync();
                // Get a unique name for the blob
                string coverImageBlobName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.CoverImage.FileName)}";
                
                // Get a reference to a blob
                BlobClient coverImageBlobClient = coverImagesContainerClient.GetBlobClient(coverImageBlobName);

                // Upload the file to blob storage
                await using var stream = bookDto.CoverImage.OpenReadStream();
                await coverImageBlobClient.UploadAsync(stream, true);
                
                book.CoverUri = coverImageBlobClient.Uri.ToString();
            }

            try
            {
                int createdId = await booksService.CreateBook(book);
                return Ok(createdId);
            }
            catch (Exception)
            {
                //Delete the audiofile in case of an error.
                await audiofileBlobClient.DeleteIfExistsAsync();
                throw;
            }
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            Book book = await booksService.GetBook(id);
            
            BlobClient audiofileBlobClient = audioFilesContainerClient.GetBlobClient(book.AudioFileName);
            await audiofileBlobClient.DeleteIfExistsAsync();
            
            string coverImageBlobName = book.CoverUri.Split('/').Last();
            BlobClient coverImageBlobClient = coverImagesContainerClient.GetBlobClient(coverImageBlobName);
            await coverImageBlobClient.DeleteIfExistsAsync();
            
            await booksService.DeleteBook(id);
            
            return Ok();
        }
    }
}
