using Moq;
using VR.Models;
using VR.Services;
using Microsoft.Extensions.Logging;
using VR.Interfaces;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VR.Tests.Services
{
    public class FileParserServiceTests
    {
        private readonly Mock<ILogger<FileParserService>> _loggerMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly FileParserService _fileParserService;

        public FileParserServiceTests()
        {
            _loggerMock = new Mock<ILogger<FileParserService>>();
            _validationServiceMock = new Mock<IValidationService>();
            _fileParserService = new FileParserService(_loggerMock.Object, _validationServiceMock.Object);
        }

        /// <summary>
        /// Tests that the ParseFileAsync method correctly parses a valid file.
        /// </summary>
        [Fact]
        public async Task ParseFileAsync_ShouldParseValidFile()
        {
            // Arrange
            var filePath = "testfile.txt";
            var fileContent = "HDR Supplier1 Box1\nLINE P000001661 9781473663800 10\nLINE P000001662 9781473667273 5\nHDR Supplier2 Box2\nLINE P000001663 9781473665798 20";
            await File.WriteAllTextAsync(filePath, fileContent);

            _validationServiceMock.Setup(v => v.ValidateHeader(It.IsAny<string[]>()))
                .Callback<string[]>(parts => new ValidationService().ValidateHeader(parts));
            _validationServiceMock.Setup(v => v.ValidateLine(It.IsAny<string[]>()))
                .Callback<string[]>(parts => new ValidationService().ValidateLine(parts));

            var boxes = new List<Box>();
            Func<List<Box>, Task> processBatchAsync = (batch) =>
            {
                boxes.AddRange(batch);
                return Task.CompletedTask;
            };

            // Act
            await _fileParserService.ParseFileAsync(filePath, processBatchAsync);

            // Assert
            Assert.Equal(2, boxes.Count);
            Assert.Equal("Supplier1", boxes[0].SupplierIdentifier);
            Assert.Equal("Box1", boxes[0].Identifier);
            Assert.Equal(2, boxes[0].Contents.Count);
            Assert.Equal("Supplier2", boxes[1].SupplierIdentifier);
            Assert.Equal("Box2", boxes[1].Identifier);
            Assert.Single(boxes[1].Contents);

            // Clean up
            File.Delete(filePath);
        }

        /// <summary>
        /// Tests that the ParseFileAsync method handles invalid header format correctly.
        /// </summary>
        [Fact]
        public async Task ParseFileAsync_ShouldHandleInvalidHeaderFormat()
        {
            // Arrange
            var filePath = "testfile_invalid_header.txt";
            var fileContent = "HDR Supplier1\nLINE P000001661 9781473663800 10";
            await File.WriteAllTextAsync(filePath, fileContent);

            var boxes = new List<Box>();
            Func<List<Box>, Task> processBatchAsync = (batch) =>
            {
                boxes.AddRange(batch);
                return Task.CompletedTask;
            };

            _validationServiceMock.Setup(v => v.ValidateHeader(It.IsAny<string[]>())).Throws(new FormatException());

            // Act
            await _fileParserService.ParseFileAsync(filePath, processBatchAsync);

            // Assert
            Assert.Empty(boxes);
            
            // Clean up
            File.Delete(filePath);
        }

        /// <summary>
        /// Tests that the ParseFileAsync method handles invalid line format correctly.
        /// </summary>
        [Fact]
        public async Task ParseFileAsync_ShouldHandleInvalidLineFormat()
        {
            // Arrange
            var filePath = "testfile_invalid_line.txt";
            var fileContent = "HDR Supplier1 Box1\nLINE P000001661 9781473663800";
            await File.WriteAllTextAsync(filePath, fileContent);

            var boxes = new List<Box>();
            Func<List<Box>, Task> processBatchAsync = (batch) =>
            {
                boxes.AddRange(batch);
                return Task.CompletedTask;
            };

            _validationServiceMock.Setup(v => v.ValidateLine(It.IsAny<string[]>())).Throws(new FormatException());

            // Act
            await _fileParserService.ParseFileAsync(filePath, processBatchAsync);

            // Assert
            Assert.Empty(boxes);
            
            // Clean up
            File.Delete(filePath);
        }

        /// <summary>
        /// Tests that the ParseFileAsync method logs an error when an exception occurs.
        /// </summary>
        [Fact]
        public async Task ParseFileAsync_ShouldLogErrorOnException()
        {
            // Arrange
            var filePath = "nonexistentfile.txt";

            var boxes = new List<Box>();
            Func<List<Box>, Task> processBatchAsync = (batch) =>
            {
                boxes.AddRange(batch);
                return Task.CompletedTask;
            };

            // Act
            await Assert.ThrowsAsync<FileNotFoundException>(() => _fileParserService.ParseFileAsync(filePath, processBatchAsync));

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error parsing file")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}


