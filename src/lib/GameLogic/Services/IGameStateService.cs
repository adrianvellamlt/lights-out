using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IGameStateService
    {
        Task<GameState> InitializeGameAsync(ushort gameId, CancellationToken cancellationToken);

        Task SaveStateAsync(GameState gameState, CancellationToken cancellationToken);

        Task<GameState?> GetLastStateAsync(Guid gameStateId, CancellationToken cancellationToken);

        Task SurrenderAsync(Guid gameStateId, CancellationToken cancellationToken);
    }
}