using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Infrastructure.Services;

public class CharaDataRepository : ICharaDataRepository
{
  private readonly HttpClient _http;

  public CharaDataRepository(HttpClient http)
  {
    _http = http;
    _http.BaseAddress = new Uri("http://localhost:8888/CharacomMauiHP/api/");
  }

  public async Task<List<CharaData>> GetCharaDataAsync(string accessToken, string projectId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      project_id = projectId,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync("get_project_chara_items.php", content);
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
}
