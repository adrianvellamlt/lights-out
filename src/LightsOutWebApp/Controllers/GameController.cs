using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightsOut.GameLogic;
using LightsOut.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightsOut.Web
{
    /// <summary>
    /// Allows player to start a new game and toggle cells.
    /// </summary>
    [ApiController]
    [Route("/api/game")]
    public class GameController : ControllerBase
    {
        private IGameStateService GameStateService { get; }
        private IGameSettingsService GameSettingsService { get; }
        private IGameVisualizer GameVisualizer { get; }
        private IHighScoreService HighScoreService { get; }
        private ISystemClock SystemClock { get; }
        public GameController(
            IGameStateService gameStateService,
            IGameSettingsService gameSettingsService,
            IGameVisualizer gameVisualizer,
            IHighScoreService highScoreService,
            ISystemClock systemClock
        )
        {
            GameStateService = gameStateService;
            GameSettingsService = gameSettingsService;
            GameVisualizer = gameVisualizer;
            HighScoreService = highScoreService;
            SystemClock = systemClock;
        }

        /// <summary>
        /// Creates a new game with the specified game Id
        /// </summary>
        /// <param name="gameId">Game Id - Specifies matrix size, complexity and game duration</param>
        /// <param name="model">Model - username for highscores</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns a rendered version of the game</returns>
        [HttpPost("play/{gameId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> NewGameAsync([FromRoute] ushort gameId, [FromBody] NewGameViewModel model, CancellationToken cancellationToken)
        {
            if (model == null) return BadRequest();

            var gameState = await GameStateService.InitializeGameAsync(gameId, model.Username, cancellationToken);

            HttpContext.Response.Headers.Add("X-GameStateId", gameState.Id.ToString());

            var drawing = GameVisualizer.Draw(gameState.Game);

            return Ok(drawing);
        }

        /// <summary>
        /// Renders the latest state of the specified game state id
        /// </summary>
        /// <param name="gameStateId">GameStateId - provided each time a new game is initialized</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns a rendered version of the game</returns>
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

                var timeTaken = SystemClock.UtcNow.DateTime - gameState.StartTimeUtc;

                var remainingTime = (gameState.GameMaxDuration - timeTaken).Seconds;

                if (remainingTime < 0) remainingTime = 0;

                HttpContext.Response.Headers.Add("X-IsSolved", gameState.Game.IsSolved ? "1" : "0");
                HttpContext.Response.Headers.Add("X-MoveCount", gameState.NoOfMoves.ToString());
                HttpContext.Response.Headers.Add("X-RemainingTime", remainingTime.ToString());

                return Ok(drawing);
            }
        }

        /// <summary>
        /// Toggles a cell and its adjacent cells. Checks whether the puzzle is solved or game timer is over.
        /// </summary>
        /// <param name="gameStateId">GameStateId - provided each time a new game is initialized</param>
        /// <param name="model">Model - Row x Column pointer</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns a rendered version of the game</returns>
        [HttpPost("toggle/{gameStateId}")]
        [Produces("text/plain")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleCellAsync([FromRoute] Guid gameStateId, [FromBody] ToggleCellViewModel model, CancellationToken cancellationToken)
        {
            if (model == null) return BadRequest();

            var gameState = await GameStateService.GetLastStateAsync(gameStateId, cancellationToken);

            if (gameState == null) return NotFound();

            if (!gameState.Game.IsSolved)
            {
                gameState.Game.ToggleCell(model.RowNumber, model.ColumnNumber);

                gameState.IncrementMoveCounter();

                await GameStateService.SaveStateAsync(gameState, cancellationToken);
            }

            var drawing = GameVisualizer.Draw(gameState.Game);

            var timeTaken = SystemClock.UtcNow.DateTime - gameState.StartTimeUtc;

            var remainingTime = (gameState.GameMaxDuration - timeTaken).Seconds;

            if (remainingTime < 0) remainingTime = 0;

            HttpContext.Response.Headers.Add("X-IsSolved", gameState.Game.IsSolved ? "1" : "0");
            HttpContext.Response.Headers.Add("X-MoveCount", gameState.NoOfMoves.ToString());
            HttpContext.Response.Headers.Add("X-RemainingTime", remainingTime.ToString());

            return Ok(drawing);
        }

        /// <summary>
        /// Game over :( Inserts your score in the high score table.
        /// </summary>
        /// <param name="gameStateId">GameStateId - provided each time a new game is initialized</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns Ok Status Code</returns>
        [HttpPost("surrender/{gameStateId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> SurrenderAsync([FromRoute] Guid gameStateId, CancellationToken cancellationToken)
        {
            await GameStateService.SurrenderAsync(gameStateId, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Gets an ordered list of highscores based on certain game parameters.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns a collection of highscore objects</returns>
        [HttpGet("highscores")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HighScoreViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHighScoresAsync(CancellationToken cancellationToken)
        {
            var highScores = new List<HighScoreViewModel>();

            ushort rank = 1;

            foreach (var highScore in await HighScoreService.GetBestHighScoresAsync(100, cancellationToken))
            {
                highScores.Add(new HighScoreViewModel
                (
                    rank++,
                    highScore.Username,
                    highScore.ComplexityLevel,
                    highScore.NoOfRows,
                    highScore.NoOfColumns,
                    highScore.RemainingLights,
                    (long)highScore.TimeTaken.TotalSeconds,
                    highScore.NoOfMoves
                ));
            }

            return Ok(highScores);
        }
    }
}