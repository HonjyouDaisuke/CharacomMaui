
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Infrastructure.Services;

public class ApiTokenValidationService : ITokenValidationService
{
  private readonly HttpClient _httpClient;

  public ApiTokenValidationService(HttpClient httpClient)
  {
    _httpClient = httpClient;
    _httpClient.BaseAddress = new Uri("http://localhost:8888/CharacomMauiHP/api/");
  }

  public async Task<TokenValidationResult> ValidateAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _httpClient.PostAsync("check_access_token.php", content);
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("----------server res check accessToken--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    return JsonSerializer.Deserialize<TokenValidationResult>(responseBody)
           ?? new TokenValidationResult { Success = false };
  }
}