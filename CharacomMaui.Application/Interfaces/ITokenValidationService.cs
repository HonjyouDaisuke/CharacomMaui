using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.Interfaces;

public interface ITokenValidationService
{
  Task<TokenValidationResult> ValidateAsync(string accessToken);
}