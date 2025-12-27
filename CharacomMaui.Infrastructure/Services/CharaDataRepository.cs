using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;

namespace CharacomMaui.Infrastructure.Services;

public class CharaDataRepository : ICharaDataRepository
{
  private readonly HttpClient _http;

  public CharaDataRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<List<CharaData>> GetCharaDataAsync(string accessToken, string projectId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      project_id = projectId,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetProjectCharaItems, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("---------- GetProjectCharaItems server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken} projectId = {projectId} ");
    try
    {
      var result = JsonSerializer.Deserialize<CharaDataResponse>(responseBody);
      return result?.Items ?? new List<CharaData>();

    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Deserialize ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
      throw; // ← ここで投げれば MAUI の output にも出る
    }
  }

  public async Task<SimpleApiResult> UpdateSelectdCharaAsync(string accessToken, string charaId, bool isSelected)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      chara_id = charaId,
      is_selected = isSelected,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.UpdateCharaSelected, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("---------- Update CharaSelected server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken} CharaId = {charaId} is_selected = {isSelected}");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var result = JsonSerializer.Deserialize<SimpleApiResult>(responseBody);
      return result;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Deserialize ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
      throw; // ← ここで投げれば MAUI の output にも出る
    }
  }
}
