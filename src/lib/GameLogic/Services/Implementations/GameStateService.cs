using System.Threading;
using System.Threading.Tasks;
using LightsOut.Infrastructure;

namespace LightsOut.GameLogic
{
    public class GameStateService : IGameStateService
    {
        private ICache<byte[]> GameStateCache { get; }

        public GameStateService(ICacheProviderFactory cacheProvider)
        {
            GameStateCache = cacheProvider.GetCacheInstance<byte[]>(CacheType.InMemory);
        }

        public Task<GameState?> GetLastStateAsync(int gameId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<GameState> InitializeGameAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveStateAsync(GameState gameState, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SurrenderAsync(int gameId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}