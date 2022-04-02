using System;
using System.Threading;
using System.Threading.Tasks;
using LightsOut.GameLogic;
using LightsOut.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightsOut.Web
{
    [ApiController]
    [Route("/api/game")]
    public class GameController : ControllerBase
    {
        private IGameStateService GameStateService { get; }
        private IGameSettingsService GameSettingsService { get; }
        private IGameVisualizer GameVisualizer { get; }
        private ISystemClock SystemClock { get; }
        public GameController(
            IGameStateService gameStateService,
            IGameSettingsService gameSettingsService,
            IGameVisualizer gameVisualizer,
            ISystemClock systemClock
        )
        {
            GameStateService = gameStateService;
            GameSettingsService = gameSettingsService;
            GameVisualizer = gameVisualizer;
            SystemClock = systemClock;
        }

        [HttpGet("play/{gameId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> NewGameAsync([FromRoute] ushort gameId, CancellationToken cancellationToken)
        {
            var gameState = await GameStateService.InitializeGameAsync(gameId, cancellationToken);

            HttpContext.Response.Headers.Add("X-GameStateId", gameState.Id.ToString());

            var drawing = GameVisualizer.Draw(gameState.Game);

            return Ok(drawing);
        }

        [HttpGet("draw/{gameStateId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DrawAsync([FromRoute] Guid gameStateId, CancellationToken cancellationToken)
        {
            if (gameStateId == Guid.Empty) return NotFound();

            var gameState = await GameStateService.GetLastStateAsync(gameStateId, cancellationToken);

            if (gameState == null) return NotFound();

            if (gameState.SurrenderedAtUTC.HasValue)
            {
                return Ok("GameOver! The puzzle beat you.");
            }
            else
            {
                var drawing = GameVisualizer.Draw(gameState.Game);

                HttpContext.Response.Headers.Add("X-IsSolved", gameState.Game.IsSolved ? "1" : "0");
                HttpContext.Response.Headers.Add("X-MoveCount", gameState.NoOfMoves.ToString());
                HttpContext.Response.Headers.Add("X-StartTime", new DateTimeOffset(gameState.StartTimeUtc, TimeSpan.Zero).ToUnixTimeSeconds().ToString());

                return Ok(drawing);
            }
        }

        [HttpPost("toggle/{gameStateId}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleCellAsync([FromRoute] Guid gameStateId, [FromBody] ToggleCellViewModel model, CancellationToken cancellationToken)
        {
            var gameState = await GameStateService.GetLastStateAsync(gameStateId, cancellationToken);

            if (gameState == null) return NotFound();

            if (!gameState.Game.IsSolved)
            {
                gameState.Game.ToggleCell(model.RowNumber, model.ColumnNumber);

                gameState.IncrementMoveCounter();

                await GameStateService.SaveStateAsync(gameState, cancellationToken);
            }

            var drawing = GameVisualizer.Draw(gameState.Game);
            
            HttpContext.Response.Headers.Add("X-IsSolved", gameState.Game.IsSolved ? "1" : "0");
            HttpContext.Response.Headers.Add("X-MoveCount", gameState.NoOfMoves.ToString());
            HttpContext.Response.Headers.Add("X-StartTime", new DateTimeOffset(gameState.StartTimeUtc, TimeSpan.Zero).ToUnixTimeSeconds().ToString());

            return Ok(drawing);
        }

        [HttpPost("surrender/{gameStateId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> SurrenderAsync([FromRoute] Guid gameStateId, CancellationToken cancellationToken)
        {
            var gameState = await GameStateService.GetLastStateAsync(gameStateId, cancellationToken);

            if (gameState == null) return NotFound();

            if (gameState.Game.IsSolved)
            {
                return Ok("Puzzle was already solved!");
            }

            gameState.SetSurrenderedTimeStamp(SystemClock.UtcNow.DateTime);

            await GameStateService.SaveStateAsync(gameState, cancellationToken);
            
            return Ok("GameOver! The puzzle beat you.");
        }
    }
}