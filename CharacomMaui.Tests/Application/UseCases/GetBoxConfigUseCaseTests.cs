using System.Threading.Tasks;
using Xunit;
using Moq;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;

namespace CharacomMaui.Tests.Application.UseCases;

public class GetBoxConfigUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_Returns_ClientId_And_ClientSecret_FromRepository()
  {
    // Arrange
    var expectedClientId = "test-client-id";
    var expectedClientSecret = "test-client-secret";

    var mockRepo = new Mock<IBoxConfigRepository>();
    mockRepo
        .Setup(r => r.GetBoxConfigAsync())
        .ReturnsAsync((expectedClientId, expectedClientSecret));

    var useCase = new GetBoxConfigUseCase(mockRepo.Object);

    // Act
    var (clientId, clientSecret) = await useCase.ExecuteAsync();

    // Assert
    Assert.Equal(expectedClientId, clientId);
    Assert.Equal(expectedClientSecret, clientSecret);

    mockRepo.Verify(r => r.GetBoxConfigAsync(), Times.Once);
  }
}
