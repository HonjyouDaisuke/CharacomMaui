namespace CharacomMaui.Application.Logging;

using System.Threading.Tasks;

public interface ILogApiClient
{
  Task SendAsync(LogRequest request);
}