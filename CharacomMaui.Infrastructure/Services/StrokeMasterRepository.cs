using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;

namespace CharacomMaui.Infrastructure.Services;

public class StrokeMasterRepository : IStrokeMasterRepository
{
  private readonly HttpClient _http;
  public StrokeMasterRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<SimpleApiResult> UpdateStrokeMasterAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.CreateStrokeMaster, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------stroke master server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken}  ");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        System.Diagnostics.Debug.WriteLine("こっち");
        return new SimpleApiResult
        {
          Success = true,
          Message = "Success Create Project...",
        };
      }


      var message = root.GetProperty("message").GetString();
      System.Diagnostics.Debug.WriteLine($"サーバーエラー: {message}");
      return new SimpleApiResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      return new SimpleApiResult
      {
        Success = false,
        Message = $"想定外のエラーが発生しました。。。{ex.Message}",
      };
    }

  }
  public async Task<string> GetStrokeFileIdAsync(string accessToken, string charaName)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      chara_name = charaName,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.GetStrokeFileId, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------stroke master get file id server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken}  ");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        System.Diagnostics.Debug.WriteLine("こっち");
        return root.GetProperty("file_id").GetString();
      }
    }
    catch (Exception)
    {
      return null;
    }
    return null;
  }

}