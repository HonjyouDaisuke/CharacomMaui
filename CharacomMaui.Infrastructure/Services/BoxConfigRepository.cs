using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Infrastructure.Services;

public class BoxConfigRepository : IBoxConfigRepository
{
  private readonly HttpClient _httpClient;

  public BoxConfigRepository(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<(string ClientId, string ClientSecret)> GetBoxConfigAsync()
  {
    var url = "https://characom.sakuraweb.com/box/box-login.php";

    var result = await _httpClient.GetFromJsonAsync<BoxConfigResponse>(url);

    if (result == null)
      throw new Exception("サーバーからデータを取得できませんでした。");

    return (result.BOX_CLIENT_ID, result.BOX_CLIENT_SECRET);
  }

  private class BoxConfigResponse
  {
    public string BOX_CLIENT_ID { get; set; } = "";
    public string BOX_CLIENT_SECRET { get; set; } = "";
  }
}
