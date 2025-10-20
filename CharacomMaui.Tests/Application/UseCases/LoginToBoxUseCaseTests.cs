using System.Threading.Tasks;
using Xunit;
using Moq;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using CharacomMaui.Application.UseCases;

namespace CharacomMaui.Tests.Application.UseCases;

public class LoginToBoxUseCaseTests
{
  [Fact]
  public void GetAuthorizationUrl_ShouldReturnUrl()
  {
    // Arrange
    var mockService = new Mock<IBoxApiAuthService>();
    var expectedUrl = "https://example.com/auth";
    mockService.Setup(s => s.GetAuthorizationUrl("id", "secret"))
               .Returns(expectedUrl);

    var useCase = new LoginToBoxUseCase(mockService.Object);

    // Act
    var result = useCase.GetAuthorizationUrl("id", "secret");

    // Assert
    Assert.Equal(expectedUrl, result);
  }

  [Fact]
  public async Task LoginWithCodeAsync_ShouldReturnAuthResult()
  {
    // Arrange
    var mockService = new Mock<IBoxApiAuthService>();
    var expected = new BoxAuthResult
    {
      AccessToken = "access",
      RefreshToken = "refresh",
      ExpiresAt = 3600 // int型で指定
    };

    mockService.Setup(s => s.ExchangeCodeForTokenAsync("code123", "https://redirect"))
               .ReturnsAsync(expected);

    var useCase = new LoginToBoxUseCase(mockService.Object);

    // Act
    var result = await useCase.LoginWithCodeAsync("code123", "https://redirect");

    // Assert
    Assert.Equal(expected.AccessToken, result.AccessToken);
    Assert.Equal(expected.RefreshToken, result.RefreshToken);
    Assert.Equal(expected.ExpiresAt, result.ExpiresAt);
  }

  [Fact]
  public async Task RefreshTokenAsync_ShouldReturnRefreshedTokens()
  {
    // Arrange
    var mockService = new Mock<IBoxApiAuthService>();
    var expected = new BoxAuthResult
    {
      AccessToken = "new_access",
      RefreshToken = "new_refresh",
      ExpiresAt = 7200
    };

    mockService.Setup(s => s.RefreshTokenAsync("refresh_token"))
               .ReturnsAsync(expected);

    var useCase = new LoginToBoxUseCase(mockService.Object);

    // Act
    var result = await useCase.RefreshTokenAsync("refresh_token");

    // Assert
    Assert.Equal("new_access", result.AccessToken);
    Assert.Equal("new_refresh", result.RefreshToken);
    Assert.Equal(7200, result.ExpiresAt);
  }
}
