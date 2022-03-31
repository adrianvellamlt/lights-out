using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public class GameSettingsService : IGameSettingsService
    {
        public Task<GameSettings> GetGameSettingsAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}