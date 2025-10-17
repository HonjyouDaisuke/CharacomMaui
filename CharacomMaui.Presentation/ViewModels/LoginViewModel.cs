using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;

namespace CharacomMaui.Presentation.ViewModels;

public class LoginViewModel
{
  private readonly LoginToBoxUseCase _loginUseCase;
  private readonly GetBoxConfigUseCase _getBoxConfigUseCase;
  private readonly ITokenStorageService _tokenStorage;

  public LoginViewModel(
      LoginToBoxUseCase loginUseCase,
      GetBoxConfigUseCase getBoxConfigUseCase,
      ITokenStorageService tokenStorage)
  {
    _loginUseCase = loginUseCase;
    _getBoxConfigUseCase = getBoxConfigUseCase;
    _tokenStorage = tokenStorage;
  }

  public async Task LoginAsync()
  {

    var (clientId, clientSecret) = await _getBoxConfigUseCase.ExecuteAsync();

    var authUrl = _loginUseCase.GetAuthorizationUrl(clientId, clientSecret);
    var callbackUrl = new Uri("myapp://callback");
    var result = await WebAuthenticator.AuthenticateAsync(new Uri(authUrl), callbackUrl);
    if (!result.Properties.TryGetValue("code", out var code))
      throw new Exception("認可コードが取得できませんでした。");

    var tokens = await _loginUseCase.LoginWithCodeAsync(code, callbackUrl.ToString());

    await _tokenStorage.SaveTokensAsync(tokens);
    return;
  }
}
