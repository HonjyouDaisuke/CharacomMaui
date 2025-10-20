using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;

namespace CharacomMaui.Tests.Application.UseCases;

public class ProcessImageFromBoxUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_ShouldCallDownloadFileAsync()
  {
    // Arrange
    var mockCloudStorage = new Mock<ICloudStorageService>();
    var mockImageProcessing = new Mock<IImageProcessingService>();

    var fileId = "file123";
    var folderId = "folder456";

    // Streamを返すように修正
    var fakeStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });

    mockCloudStorage.Setup(c => c.DownloadFileAsync(fileId))
                    .ReturnsAsync(fakeStream);

    var useCase = new ProcessImageFromBoxUseCase(mockCloudStorage.Object, mockImageProcessing.Object);

    // Act
    await useCase.ExecuteAsync(fileId, folderId);

    // Assert
    mockCloudStorage.Verify(c => c.DownloadFileAsync(fileId), Times.Once);
    mockImageProcessing.VerifyNoOtherCalls();
  }
}

