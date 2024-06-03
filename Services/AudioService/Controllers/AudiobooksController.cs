using System.Security.Claims;
using AudioService.Attributes;
using AudioService.DTOs;
using AudioService.Models;
using AudioService.Services.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Auth;
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
        
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBooksForUser(string userId)
        {
            return Ok(await booksService.GetBooksForUser(userId));
        }

        [Authorize]
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 724288000)] //690 mb
        public async Task<IActionResult> CreateBook([FromForm] BookDTO bookDto)
        {
            if (bookDto.AudioFile == null || bookDto.AudioFile.Length == 0)
                return BadRequest("No audio file uploaded.");
            
            if(!AudioHelper.IsAudiofile(bookDto.AudioFile))
                return BadRequest("The uploaded file is not an audio file.");

            string userId = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value ?? null;
            if (userId == null)
                return BadRequest("Could not find user id in claims (sub).");
            
            Book book = new Book
            {
                OwnerId = userId,
                Name = bookDto.Name,
                Description = bookDto.Description,
                Language = bookDto.Language,
                Genre = bookDto.Genre,
                Authors = bookDto.Authors,
                Length = bookDto.Length
            };
            
            int createdId = await booksService.CreateBook(book, bookDto.AudioFile, bookDto.CoverImage);
            return Ok(createdId);
        }
        
        [Authorize]
        [OwnsBookRequirement]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await booksService.DeleteBook(id);
            return Ok();
        }
    }
}
