using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsOut.GameLogic
{
    public interface IHighScoreService
    {
        Task AddHighScoreAsync(HightScore highScore, CancellationToken cancellationToken);
    }
}