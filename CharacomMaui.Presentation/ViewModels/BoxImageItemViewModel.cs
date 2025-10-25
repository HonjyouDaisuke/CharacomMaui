// CharacomMaui.Presentation/ViewModels/BoxItemViewModel.cs
using CharacomMaui.Domain.Entities;
using SkiaSharp;

namespace CharacomMaui.Presentation.ViewModels
{
    public class BoxImageItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public ImageSource? Image { get; private set; }

        public BoxImageItemViewModel(BoxImageItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Type = item.Type;

            // ImageSourceは非同期で安全に生成
            LoadImageAsync(item.Image);
        }

        private async void LoadImageAsync(byte[] imageBytes)
        {
            try
            {
                // バックグラウンドで SKBitmap → ImageSource 変換
                var img = await Task.Run(() =>
                {
                    using var ms = new MemoryStream(imageBytes);
                    var bmp = SKBitmap.Decode(ms);
                    return CharacomMaui.Presentation.Helpers.ImageSourceConverter.FromSKBitmap(bmp);
                });

                // UIスレッドで Image にセット
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Image = img;
                });
            }
            catch
            {
                // 変換失敗時は null のまま
                Image = null;
            }
        }
    }
}
