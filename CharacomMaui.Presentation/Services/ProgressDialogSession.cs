using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

internal sealed class ProgressDialogSession : IProgressDialogSession
{
  private readonly ProgressDialogService _service;
  private bool _disposed;

  public ProgressDialogSession(ProgressDialogService service)
  {
    _service = service;
  }

  public Task UpdateAsync(string message, double progress)
  {
    if (_disposed) return Task.CompletedTask; // or throw new ObjectDisposedException(nameof(ProgressDialogSession));
    return _service.UpdateAsync(message, progress);
  }

  public async ValueTask DisposeAsync()
  {
    if (_disposed) return;
    _disposed = true;

    await _service.CloseAsync();
  }
}
