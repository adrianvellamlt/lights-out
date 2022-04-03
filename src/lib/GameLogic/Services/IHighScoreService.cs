using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IHighScoreService
    {
        Task AddHighScoreAsync(HighScore highScore, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the best high scores based on the complexity, remaining switched on lights, no of moves and time taken
        /// </summary>
        /// <param name="limit">How many scores to get</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Collection of highscores</returns>
        Task<IEnumerable<HighScore>> GetBestHighScoresAsync(ushort limit, CancellationToken cancellationToken);
    }
}