
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightsOut.GameLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightsOut.Web
{
    /// <summary>
    /// Provides GameSettings CRUD operations
    /// </summary>
    [ApiController]
    [Route("/api/gamesettings")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class GameSettingsController : ControllerBase
    {
        private IGameSettingsService GameSettingsService { get; }
        public GameSettingsController(IGameSettingsService gameSettingsService)
        {
            GameSettingsService = gameSettingsService;
        }

        /// <summary>
        /// Gets game settings that are currently available.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <param name="gameId">GameId to filter result</param>
        /// <returns>Returns collection of game settings.</returns>
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<GameSettingViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGameSettingsAsync(CancellationToken cancellationToken, [FromQuery] ushort? gameId = null)
        {
            var response = new List<GameSettingViewModel>();

            static GameSettingViewModel Map(GameSettings gameSetting) => new(
                gameSetting.Id,
                gameSetting.ComplexityLevel,
                gameSetting.NoOfRows,
                gameSetting.NoOfColumns,
                gameSetting.NoOfSwitchedOnLights,
                gameSetting.GameMaxDuration  
            );

            if (gameId.HasValue)
            {
                var gameSetting = await GameSettingsService.GetGameSettingAsync(gameId.Value, cancellationToken);

                if (gameSetting != null)
                {
                    response.Add(Map(gameSetting));
                }
            }
            else
            {
                foreach (var gameSetting in await GameSettingsService.GetGameSettingsAsync(cancellationToken))
                {
                    response.Add(Map(gameSetting));
                }
            }

            return Ok(response);
        }
    
        /// <summary>
        /// Adds a new game setting
        /// </summary>
        /// <param name="model">Game setting details</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns stored game setting object.</returns>
        [HttpPost()]
        [ProducesResponseType(typeof(GameSettingViewModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddGameSettingAsync([FromBody] UpdateGameSettingViewModel model, CancellationToken cancellationToken)
        {
            var gameSetting = new GameSettings
            {
                NoOfRows = model.NoOfRows,
                NoOfColumns = model.NoOfColumns,
                NoOfSwitchedOnLights = model.NoOfSwitchedOnLights,
                ComplexityLevel = model.Complexity,
                GameMaxDurationStr = model.GameMaxDuration.ToString()
            };

            var id = await GameSettingsService.AddGameSettingAsync(gameSetting, cancellationToken);

            return Created(
                HttpContext.Request.Path + "?gameId=" + id,
                new GameSettingViewModel(id, model.Complexity, model.NoOfRows, model.NoOfColumns, model.NoOfSwitchedOnLights, model.GameMaxDuration)
            );
        }

        /// <summary>
        /// Deletes the specified game setting
        /// </summary>
        /// <param name="id">Game Setting Id</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns Ok Status Code</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteGameSettingAsync([FromRoute] ushort id, CancellationToken cancellationToken)
        {
            await GameSettingsService.DeleteGameSettingAsync(id, cancellationToken);

            return Ok();
        }
        
        /// <summary>
        /// Updates the specified game setting
        /// </summary>
        /// <param name="id">Game Setting Id</param>
        /// <param name="model">New Game Setting details</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Returns Ok Status Code</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGameSettingAsync([FromRoute] ushort id, [FromBody] UpdateGameSettingViewModel model, CancellationToken cancellationToken)
        {
            var gameSetting = new GameSettings
            {
                Id = id,
                NoOfRows = model.NoOfRows,
                NoOfColumns = model.NoOfColumns,
                NoOfSwitchedOnLights = model.NoOfSwitchedOnLights,
                ComplexityLevel = model.Complexity,
                GameMaxDurationStr = model.GameMaxDuration.ToString()
            };

            await GameSettingsService.UpdateGameSettingAsync(gameSetting, cancellationToken);

            return Ok();
        }
    }
}