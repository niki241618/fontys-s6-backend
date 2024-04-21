using System.Text;
using AudioService.Controllers;
using AudioService.DTOs;
using AudioService.Models;
using AudioService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NAudio.Wave;

namespace AudioOasisTests.ControllerTests;

public class AudiobooksControllerTests
{
	private readonly Mock<IBooksService> mockBooksService;
    private readonly AudiobooksController controller;

    public AudiobooksControllerTests()
    {
        mockBooksService = new Mock<IBooksService>();
        controller = new AudiobooksController(mockBooksService.Object);
    }

    [Fact]
    public async Task GetBooks_ReturnsOk()
    {
        // Arrange
        mockBooksService.Setup(service => service.GetBooks()).ReturnsAsync(new[] { new Book() });

        // Act
        var result = await controller.GetBooks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetBook_WithValidId_ReturnsOk()
    {
        // Arrange
        const int bookId = 1;
        mockBooksService.Setup(service => service.GetBook(bookId)).ReturnsAsync(new Book());

        // Act
        var result = await controller.GetBook(bookId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    // Add more tests for other controller methods...

    [Fact]
    public async Task CreateBook_WithoutAudioFile_ReturnsBadRequest()
    {
        // Arrange
        var bookDto = new BookDTO
        {
            Name = "Test Book",
            Description = "Test Description",
            Language = "English",
            Genre = "Fiction",
            Authors = new [] {"Test Author"},
        };

        // Act
        var result = await controller.CreateBook(bookDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateBook_WithInvalidAudioFile_ReturnsBadRequest()
    {
        // Arrange
        var bookDto = new BookDTO
        {
            Name = "Test Book",
            Description = "Test Description",
            Language = "English",
            Genre = "Fiction",
            Authors = new [] {"Test Author"},
            // Fill in with valid data for testing
            AudioFile = new FormFile(Stream.Null, 0, 0, "audioFile", "testAudio.txt") // Not an audio file
        };

        // Act
        var result = await controller.CreateBook(bookDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    //ToDo inject the class that will be responsible for getting the duration of the audio file.
    
    // [Fact]
    // public async Task CreateBook_WithValidData_ReturnsOk()
    // {
    //     // Arrange
    //     var bookDto = new BookDTO
    //     {
    //         // Fill in with valid data for testing
    //         Name = "Test Book",
    //         Description = "Test Description",
    //         Language = "English",
    //         Genre = "Fiction",
    //         Authors = new [] {"Test Author"},
    //     };
    //
    //     bookDto.AudioFile = Mp3AudioGenerator.GenerateDummyMp3File();
    //
    //     mockBooksService.Setup(service => service.CreateBook(It.IsAny<Book>(), bookDto.AudioFile, null))
    //         .ReturnsAsync(1);
    //
    //     // Act
    //     var result = await controller.CreateBook(bookDto);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.Equal(1, okResult.Value);
    // }

    [Fact]
    public async Task DeleteBook_WithValidId_ReturnsOk()
    {
        // Arrange
        const int bookId = 1;
        mockBooksService.Setup(service => service.DeleteBook(bookId)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteBook(bookId);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
   
}