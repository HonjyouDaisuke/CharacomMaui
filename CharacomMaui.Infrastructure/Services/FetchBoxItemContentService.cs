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

public class FetchBoxItemContentService : IFetchBoxItemContentService
{
  private readonly HttpClient _http;

  public FetchBoxItemContentService(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<FetchImageResult> FetchImageData(string accessToken, string fileId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      file_id = fileId,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.FetchBoxItem, content);
    // Content-Type を取得
    var contentType = res.Content.Headers.ContentType?.MediaType;

    // 🔍 JSON（＝エラー）ならエラーレスポンスへ
    if (contentType == "application/json")
    {
      var jsonText = await res.Content.ReadAsStringAsync();

      return new FetchImageResult
      {
        Success = false,
        ErrorMessage = jsonText
      };
    }

    // 🔍 画像バイナリ（成功）
    var bytes = await res.Content.ReadAsByteArrayAsync();

    return new FetchImageResult
    {
      Success = true,
      ContentType = contentType,
      BinaryData = bytes
    };
  }

  public async Task<FetchImageResult> FetchThumbnailData(string accessToken, string fileId, int width, int height)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      file_id = fileId,
      width = width,
      height = height,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.FetchBoxThumbnail, content);
    // Content-Type を取得
    var contentType = res.Content.Headers.ContentType?.MediaType;

    // 🔍 JSON（＝エラー）ならエラーレスポンスへ
    if (contentType == "application/json")
    {
      var jsonText = await res.Content.ReadAsStringAsync();

      return new FetchImageResult
      {
        Success = false,
        ErrorMessage = jsonText
      };
    }

    // 🔍 画像バイナリ（成功）
    var bytes = await res.Content.ReadAsByteArrayAsync();

    return new FetchImageResult
    {
      Success = true,
      ContentType = contentType,
      BinaryData = bytes
    };
  }
}
