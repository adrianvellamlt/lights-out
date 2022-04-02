using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IGameSettingsService
    {
        Task<IEnumerable<GameSettings>> GetGameSettingsAsync(CancellationToken cancellationToken);

        Task<GameSettings?> GetGameSettingAsync(ushort gameId, CancellationToken cancellationToken);

        Task UpdateGameSettingAsync(GameSettings gameSettings, CancellationToken cancellationToken);

        Task DeleteGameSettingAsync(ushort gameId, CancellationToken cancellationToken);

        Task<ushort> AddGameSettingAsync(GameSettings gameSettings, CancellationToken cancellationToken);
    }
}