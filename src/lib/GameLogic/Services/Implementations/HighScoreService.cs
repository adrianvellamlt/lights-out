using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LightsOut.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace LightsOut.GameLogic
{
    public class HighScoreService : IHighScoreService
    {
        private IOptionsMonitor<ConnectionStringOptions> ConnectionStrings { get; }
        public HighScoreService(IOptionsMonitor<ConnectionStringOptions> connectionStrings)
            => ConnectionStrings = connectionStrings;

        public async Task AddHighScoreAsync(HightScore highScore, CancellationToken cancellationToken)
        {
            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrEmpty(connStr.SQLite)) return;

            using var conn = new SqliteConnection(connStr.SQLite);

            await conn.OpenAsync(cancellationToken);

            await conn.ExecuteAsync
            (
                new CommandDefinition
                (
                    @"INSERT INTO [HighScores]
                    (
                          [GameStateId]
                        , [ComplexityLevel]
                        , [NoOfRows]
                        , [NoOfColumns]
                        , [RemainingLights]
                        , [TimeTaken]
                        , [NoOfMoves]
                    )
                    VALUES
                    (
                          @GameStateId
                        , @ComplexityLevel
                        , @NoOfRows
                        , @NoOfColumns
                        , @RemainingLights
                        , @TimeTaken
                        , @NoOfMoves
                    )",
                    highScore,
                    cancellationToken: cancellationToken
                )
            );
        }
    }
}