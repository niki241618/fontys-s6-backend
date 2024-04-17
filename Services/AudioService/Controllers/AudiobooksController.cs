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
        public AudiobooksController(IBooksService booksService)
        {
            this.booksService = booksService;
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
            
            int createdId = await booksService.CreateBook(book, bookDto.AudioFile, bookDto.CoverImage);
            return Ok(createdId);
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await booksService.DeleteBook(id);
            return Ok();
        }
    }
}
