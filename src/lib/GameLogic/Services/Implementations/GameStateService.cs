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
        private IHighScoreService HighScoreService { get; }

        public GameStateService
        (
            ISystemClock systemClock,
            ICacheProviderFactory cacheProvider,
            IGameSettingsService gameSettingsService,
            IHighScoreService highScoreService
        )
        {
            SystemClock = systemClock;
            GameStateCache = cacheProvider.GetCacheInstance<GameState>(CacheType.InMemory);
            GameSettingsService = gameSettingsService;
            HighScoreService = highScoreService;
        }

        public async Task<GameState> InitializeGameAsync(ushort gameId, CancellationToken cancellationToken)
        {
            var settings = await GameSettingsService.GetGameSettingAsync(gameId, cancellationToken);

            if (settings == null) throw new ArgumentException("Game Id is not valid", nameof(gameId));

            var game = new LightsOut(settings.NoOfRows, settings.NoOfColumns, settings.NoOfSwitchedOnLights);

            var state = new GameState(Guid.NewGuid(), gameId, SystemClock.UtcNow.DateTime, settings.GameMaxDuration, game);

            await GameStateCache.SetAsync(state.Id.ToString(), state, settings.GameMaxDuration);

            return state;
        }

        public Task<GameState?> GetLastStateAsync(Guid gameStateId, CancellationToken cancellationToken)
            => GameStateCache.GetAsync(gameStateId.ToString());

        public async Task SaveStateAsync(GameState gameState, CancellationToken cancellationToken)
        {
            var timeTaken = SystemClock.UtcNow.DateTime - gameState.StartTimeUtc;

            var remainingTime = gameState.GameMaxDuration - timeTaken;

            // game is not completed yet
            if (remainingTime > TimeSpan.Zero || !gameState.Game.IsSolved)
            {
                await GameStateCache.SetAsync(gameState.Id.ToString(), gameState, remainingTime);

                return;
            }

            // game is completed therefore add result to high scores
            await GameStateCache.RemoveAsync(gameState.Id.ToString());

            var settings = await GameSettingsService.GetGameSettingAsync(gameState.GameId, cancellationToken);

            // settings have been deleted. cannot add this result to high score
            if (settings == null) return;

            byte remainingLights = 0;

            foreach (var cell in gameState.Game.Matrix)
            {
                if(cell) remainingLights++;
            }

            await HighScoreService.AddHighScoreAsync
            (
                new HightScore
                {
                    GameStateId = gameState.Id,
                    ComplexityLevel = settings.ComplexityLevel,
                    NoOfColumns = settings.NoOfColumns,
                    NoOfRows = settings.NoOfRows,
                    NoOfMoves = gameState.NoOfMoves,
                    RemainingLights = remainingLights,
                    TimeTaken = timeTaken
                },
                cancellationToken
            );
        }

        public async Task SurrenderAsync(Guid gameStateId, CancellationToken cancellationToken)
        {
            var gameState = await GameStateCache.GetAsync(gameStateId.ToString());

            if (gameState == null) return;

            await GameStateCache.RemoveAsync(gameState.Id.ToString());
            
            var settings = await GameSettingsService.GetGameSettingAsync(gameState.GameId, cancellationToken);

            // settings have been deleted. cannot add this result to high score
            if (settings == null) return;

            byte remainingLights = 0;

            foreach (var cell in gameState.Game.Matrix)
            {
                if(cell) remainingLights++;
            }

            await HighScoreService.AddHighScoreAsync
            (
                new HightScore
                {
                    GameStateId = gameState.Id,
                    ComplexityLevel = settings.ComplexityLevel,
                    NoOfColumns = settings.NoOfColumns,
                    NoOfRows = settings.NoOfRows,
                    NoOfMoves = gameState.NoOfMoves,
                    RemainingLights = remainingLights,
                    TimeTaken = SystemClock.UtcNow.DateTime - gameState.StartTimeUtc
                },
                cancellationToken
            );
        }
    }
}