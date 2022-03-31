using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IGameStateService
    {
        Task<GameState> InitializeGameAsync(CancellationToken cancellationToken);

        Task SaveStateAsync(GameState gameState, CancellationToken cancellationToken);

        Task<GameState?> GetLastStateAsync(int gameId, CancellationToken cancellationToken);

        Task SurrenderAsync(int gameId, CancellationToken cancellationToken);
    }
}