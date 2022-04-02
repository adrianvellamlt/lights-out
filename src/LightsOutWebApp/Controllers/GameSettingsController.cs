
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightsOut.GameLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightsOut.Web
{
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteGameSettingAsync([FromRoute] ushort id, CancellationToken cancellationToken)
        {
            await GameSettingsService.DeleteGameSettingAsync(id, cancellationToken);

            return Ok();
        }
        
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