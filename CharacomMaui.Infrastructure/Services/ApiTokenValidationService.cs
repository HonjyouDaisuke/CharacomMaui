
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;

namespace CharacomMaui.Infrastructure.Services;

public class ApiTokenValidationService : ITokenValidationService
{
  private readonly HttpClient _http;

  public ApiTokenValidationService(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<TokenValidationResult> ValidateAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.CheckAccessToken, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("----------server res check accessToken--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);

    return JsonSerializer.Deserialize<TokenValidationResult>(responseBody)
           ?? new TokenValidationResult { Success = false };
  }
}