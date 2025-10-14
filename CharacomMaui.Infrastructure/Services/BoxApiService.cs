using CharacomMaui.Application.Interfaces;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CharacomMaui.Infrastructure.Services;

public class BoxApiService : ICloudStorageService
{
    private readonly HttpClient _httpClient;

    public BoxApiService(string accessToken)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task UploadFileAsync(string folderId, string filePath)
    {
        using var content = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(filePath);

        // Box API に必要な attributes JSON
        content.Add(new StringContent(
            $"{{\"name\":\"{Path.GetFileName(filePath)}\",\"parent\":{{\"id\":\"{folderId}\"}}}}"
        ), "attributes");

        content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

        var response = await _httpClient.PostAsync(
            "https://upload.box.com/api/2.0/files/content", content
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> DownloadFileAsync(string fileId)
    {
        var response = await _httpClient.GetAsync($"https://api.box.com/2.0/files/{fileId}/content");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }
}
