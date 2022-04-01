using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public class GameSettingsService : IGameSettingsService
    {
        //TODO: persist this in a db
        public Task<GameSettings> GetGameSettingsAsync(CancellationToken cancellationToken)
            => Task.FromResult(
                new GameSettings
                {
                    NoOfRows = 5,
                    NoOfColumns = 5,
                    GameMaxDuration = TimeSpan.FromMinutes(30),
                    NoOfSwitchedOnLights = 10
                }
            );
    }
}