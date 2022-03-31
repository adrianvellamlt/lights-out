using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IGameSettingsService
    {
        Task<GameSettings> GetGameSettingsAsync(CancellationToken cancellationToken);
    }
}