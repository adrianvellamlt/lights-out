using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IHighScoreService
    {
        Task AddHighScoreAsync(HighScore highScore, CancellationToken cancellationToken);

        Task<IEnumerable<HighScore>> GetBestHighScoresAsync(ushort limit, CancellationToken cancellationToken);
    }
}