using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IGameStateService
    {
        Task<GameState> InitializeGameAsync(CancellationToken cancellationToken);

        Task SaveStateAsync(GameState gameState, CancellationToken cancellationToken);

        Task<GameState?> GetLastStateAsync(Guid gameId, CancellationToken cancellationToken);

        Task SurrenderAsync(Guid gameId, CancellationToken cancellationToken);
    }
}