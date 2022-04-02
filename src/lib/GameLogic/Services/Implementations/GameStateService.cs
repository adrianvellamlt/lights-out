using System;
using System.Threading;
using System.Threading.Tasks;
using LightsOut.Infrastructure;

namespace LightsOut.GameLogic
{
    public class GameStateService : IGameStateService
    {
        private ISystemClock SystemClock { get; }
        private ICache<GameState> GameStateCache { get; }
        private IGameSettingsService GameSettingsService { get; }

        public GameStateService(ISystemClock systemClock, ICacheProviderFactory cacheProvider, IGameSettingsService gameSettingsService)
        {
            SystemClock = systemClock;
            GameStateCache = cacheProvider.GetCacheInstance<GameState>(CacheType.InMemory);
            GameSettingsService = gameSettingsService;
        }

        public async Task<GameState> InitializeGameAsync(ushort gameId, CancellationToken cancellationToken)
        {
            var settings = await GameSettingsService.GetGameSettingAsync(gameId, cancellationToken);

            if (settings == null) throw new ArgumentException("Game Id is not valid", nameof(gameId));

            var game = new LightsOut(settings.NoOfRows, settings.NoOfColumns, settings.NoOfSwitchedOnLights);

            var state = new GameState(Guid.NewGuid(), gameId, SystemClock.UtcNow.DateTime, game);

            await GameStateCache.SetAsync(state.Id.ToString(), state, settings.GameMaxDuration);

            return state;
        }

        public Task<GameState?> GetLastStateAsync(Guid gameId, CancellationToken cancellationToken)
            => GameStateCache.GetAsync(gameId.ToString());

        public async Task SaveStateAsync(GameState gameState, CancellationToken cancellationToken)
        {
            var settings = await GameSettingsService.GetGameSettingAsync(gameState.GameId, cancellationToken);

            if (settings == null) throw new ArgumentException("Game Id is not valid", nameof(gameState.GameId));

            var remainingTime = settings.GameMaxDuration - (SystemClock.UtcNow.DateTime - gameState.StartTimeUtc);

            if (gameState.Game.IsSolved)
            {
                gameState.SetCompletedTimeStamp(SystemClock.UtcNow.DateTime);

                //TODO: maybe persist this for some sort of high score system
            }
            else
            {
                await GameStateCache.SetAsync(gameState.Id.ToString(), gameState, remainingTime);
            }
        }

        public async Task SurrenderAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var state = await GameStateCache.GetAsync(gameId.ToString());

            if (state == null) return;

            state.SetSurrenderedTimeStamp(SystemClock.UtcNow.DateTime);

            //TODO: maybe persist this for some sort of high score system
        }
    }
}